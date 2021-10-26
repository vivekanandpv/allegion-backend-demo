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
            if (!Exists(viewModel.Username)) {
                throw new LoginFailedException(
                    $"Cannot find the username: {viewModel.Username}. Login attempt prevented.");
            }

            var appUserDb = await _context.AppUsers
                .Include(u => u.AppUserRoles)
                .ThenInclude(ur => ur.AppRole)
                .FirstOrDefaultAsync(u => u.Email == viewModel.Username);

            var loginResult = VerifyPassword(viewModel.Password, appUserDb.PasswordHash, appUserDb.PasswordSalt);

            if (!loginResult) {
                throw new LoginFailedException(
                    $"Passwords do not match for the username: {viewModel.Username}. Login attempt prevented.");
            }

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

        private void CreatePasswordHash(string rawPassword, out byte[] passwordHash, out byte[] passwordSalt) {
            using (var hmac = new HMACSHA512()) {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(GetBytes(rawPassword));
            }
        }

        private bool VerifyPassword(string rawPassword, byte[] passwordHash, byte[] passwordSalt) {
            using (var hmac = new HMACSHA512(passwordSalt)) {
               var newHash =  hmac.ComputeHash(GetBytes(rawPassword));
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
    }
}