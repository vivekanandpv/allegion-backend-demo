using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using CCAP.Api.DataAccess;
using CCAP.Api.Exceptions;
using CCAP.Api.Models;
using CCAP.Api.Utils;
using CCAP.Api.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace CCAP.Api.Services {
    public class AuthService : IAuthService {
        private readonly CCAPContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(CCAPContext context, IConfiguration configuration) {
            _context = context;
            _configuration = configuration;
        }

        public async Task Register(GeneralUserRegisterViewModel viewModel) {
            var appUser = await ApplyRoles(new string[] { "user" }, Preregister(viewModel, false));
            await _context.AppUsers.AddAsync(appUser);
            await SaveChanges();
        }

        public async Task Register(StaffUserRegisterViewModel viewModel) {
            var appUser = await ApplyRoles(viewModel.Roles, Preregister(viewModel, true));
            await _context.AppUsers.AddAsync(appUser);
            await SaveChanges();
        }

        public async Task<TokenViewModel> Login(LoginViewModel viewModel) {
            //  Does the user exist?
            if (!await IsAllowedForLogin(viewModel.Username)) {
                throw new LoginFailedException(
                    $"Cannot allow the username: {viewModel.Username} to login.");
            }

            var appUserDb = await _context.AppUsers
                .Include(u => u.AppUserRoles)
                .ThenInclude(ur => ur.AppRole)
                .FirstOrDefaultAsync(u => u.Email == viewModel.Username);

            var loginResult = VerifyPassword(viewModel.Password, appUserDb.PasswordHash, appUserDb.PasswordSalt);

            if (!loginResult) {
                await RegisterWrongLogin(appUserDb);
                throw new LoginFailedException(
                    $"Passwords do not match for the username: {viewModel.Username}. Login attempt prevented.");
            }

            await RegisterCorrectLogin(appUserDb);

            return GetToken(appUserDb);
        }

        private AppUser Preregister(GeneralUserRegisterViewModel viewModel, bool isStaff) {
            //  duplicate registration should be avoided
            if (Exists(viewModel.Email)) {
                throw new DuplicateUserRegistrationException(
                    $"User with email: {viewModel.Email} already exists. Registration prevented.");
            }

            //  create a domain/model object
            var appUser = new AppUser {
                FirstName = viewModel.FirstName,
                LastName = viewModel.LastName,
                Email = viewModel.Email,
                IsStaff = isStaff,
                CreatedOn = DateTime.UtcNow
            };

            CreatePasswordHash(viewModel.Password, out var passwordHash, out var passwordSalt);

            appUser.PasswordHash = passwordHash;
            appUser.PasswordSalt = passwordSalt;

            return appUser;
        }

        private async Task<AppUser> ApplyRoles(string[] roles, AppUser appUser) {
            foreach (var role in roles) {
                var appRole = await GetRole(role);

                if (appRole == null) {
                    throw new RoleAbsentException(
                        $"Role: {role} doesn't exist. Registration of {appUser.Email} is halted."
                    );
                }

                appUser.AppUserRoles.Add(new AppUserRole {
                    AppUser = appUser,
                    AppRole = appRole,
                    AppRoleId = appRole.Id
                });
            }

            return appUser;
        }

        private TokenViewModel GetToken(AppUser appUser) {
            var claims = new List<Claim> {
                new Claim(ClaimTypes.Name, $"{appUser.FirstName} {appUser.LastName}")
            };

            foreach (var userRole in appUser.AppUserRoles) {
                claims.Add(new Claim("Roles", userRole.AppRole.RoleName));
            }

            var key = new SymmetricSecurityKey(GetBytes(_configuration.GetSection("AuthConfig:ServerSecret").Value));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(2),
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();

            return new TokenViewModel {
                Jwt = tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor))
            };
        }

        public bool Exists(string email) {
            return _context.AppUsers.Any(u => u.Email == email);
        }

        public async Task ChangePassword(ChangePasswordViewModel viewModel) {
            //  make sure the user exists
            if (!Exists(viewModel.Username)) {
                throw new LoginFailedException(
                    $"Change password prevented for the non-existing user: {viewModel.Username}");
            }

            //  make sure new password and confirm new passwords are the same
            if (viewModel.NewPassword != viewModel.ConfirmNewPassword) {
                throw new DomainValidationException($"Change password prevented as new passwords do not match");
            }

            var userDb = await _context.AppUsers.FirstOrDefaultAsync(u => u.Email == viewModel.Username);

            //  make sure the current-password matches
            var currentPasswordResult =
                VerifyPassword(viewModel.CurrentPassword, userDb.PasswordHash, userDb.PasswordSalt);

            if (!currentPasswordResult) {
                throw new LoginFailedException(
                    $"Change password prevented for the user: {viewModel.Username}, as the current password doesn't match");
            }

            //  hash the new password and replace it in the user data
            CreatePasswordHash(viewModel.NewPassword, out var newPasswordHash, out var newPasswordSalt);
            userDb.PasswordHash = newPasswordHash;
            userDb.PasswordSalt = newPasswordSalt;

            //  save
            await SaveChanges();
        }

        public async Task<ResetKeyViewModel> ResetForUser(string username) {
            //  Check if the user exists
            if (!Exists(username)) {
                throw new DomainValidationException($"Cannot reset the password for non-existent user {username}");
            }

            //  Generate the new random password
            var randomPassword = StaticProvider.GetRandomPassword(8);

            var userDb = await _context.AppUsers.FirstOrDefaultAsync(u => u.Email == username);

            //  Force the user to reset (set the property)
            userDb.IsForcedToResetPassword = true;
            userDb.NWrongAttempts = 0;
            userDb.IsLocked = false;

            //  Replace the hash and salt
            CreatePasswordHash(randomPassword, out var passwordHash, out var passwordSalt);
            userDb.PasswordHash = passwordHash;
            userDb.PasswordSalt = passwordSalt;

            //  Save
            await SaveChanges();

            //  return the random password as reset key
            return new ResetKeyViewModel {
                ResetKey = randomPassword
            };
        }

        public async Task ResetPassword(ResetPasswordViewModel viewModel) {
            //  check if the user exists
            if (!Exists(viewModel.Username)) {
                throw new LoginFailedException($"Cannot process reset for the non-existent user: {viewModel.Username}");
            }

            //  check if the reset key is right
            var userDb = await _context.AppUsers.FirstOrDefaultAsync(u => u.Email == viewModel.Username);

            if (!userDb.IsForcedToResetPassword) {
                throw new LoginFailedException(
                    $"User {viewModel.Username} is unnecessarily attempting to reset the password");
            }

            var passwordMatch = VerifyPassword(viewModel.ResetKey, userDb.PasswordHash, userDb.PasswordSalt);

            if (!passwordMatch) {
                await RegisterWrongLogin(userDb);
                throw new LoginFailedException(
                    $"Cannot reset for user {viewModel.Username} as the reset key doesn't match");
            }

            await RegisterCorrectLogin(userDb);

            if (viewModel.NewPassword != viewModel.ConfirmNewPassword) {
                throw new DomainValidationException(
                    $"New passwords do not match for the reset password of user {viewModel.Username}");
            }
            
            //  replace the hash and salt
            CreatePasswordHash(viewModel.NewPassword, out var passwordHash, out var passwordSalt);
            userDb.PasswordHash = passwordHash;
            userDb.PasswordSalt = passwordSalt;
            
            //  remove the forced to reset flag
            userDb.IsForcedToResetPassword = false;
            
            //  save
            await SaveChanges();
        }

        private void CreatePasswordHash(string rawPassword, out byte[] passwordHash, out byte[] passwordSalt) {
            using (var hmac = new HMACSHA512()) {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(GetBytes(rawPassword));
            }
        }

        private bool VerifyPassword(string rawPassword, byte[] passwordHash, byte[] passwordSalt) {
            using (var hmac = new HMACSHA512(passwordSalt)) {
                var newHash = hmac.ComputeHash(GetBytes(rawPassword));
                for (int i = 0; i < newHash.Length; ++i) {
                    if (newHash[i] != passwordHash[i]) {
                        return false;
                    }
                }

                return true;
            }
        }

        private byte[] GetBytes(string inputString) {
            return System.Text.Encoding.Default.GetBytes(inputString);
        }

        private string GetString(byte[] inputBytes) {
            return System.Text.Encoding.Default.GetString(inputBytes);
        }

        private async Task<AppRole> GetRole(string roleName) {
            return await _context.AppRoles.FirstOrDefaultAsync(r => r.RoleName == roleName);
        }

        private async Task SaveChanges() {
            await _context.SaveChangesAsync();
        }

        private async Task<bool> IsAllowedForLogin(string username) {
            if (!Exists(username)) {
                Console.WriteLine($"Non-existent login attempt with username: {username}");
                return false;
            }

            var userDb = await _context.AppUsers.FirstOrDefaultAsync(u => u.Email == username);

            if (userDb.IsForcedToResetPassword) {
                Console.WriteLine($"Cannot allow the user: {username} as password reset is pending");
                return false;
            }

            Console.WriteLine($"Cannot allow the user: {username} as the user is locked");
            return !userDb.IsLocked;
        }

        private async Task RegisterWrongLogin(AppUser appUser) {
            if (appUser.NWrongAttempts == StaticProvider.MaxWrongAttempts) {
                Console.WriteLine($"User {appUser.Email} is locked after max login attempts");
                appUser.IsLocked = true;
            }
            else {
                ++appUser.NWrongAttempts;
                Console.WriteLine($"Registering a wrong login attempt #{appUser.NWrongAttempts} for user {appUser.Email}");
            }

            await SaveChanges();
        }

        private async Task RegisterCorrectLogin(AppUser appUser) {
            appUser.IsLocked = false;
            appUser.NWrongAttempts = 0;
            await SaveChanges();
            Console.WriteLine($"Registering a successful login attempt for user {appUser.Email}");
        }
    }
}