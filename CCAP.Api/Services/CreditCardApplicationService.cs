using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CCAP.Api.DataAccess;
using CCAP.Api.Exceptions;
using CCAP.Api.Models;
using CCAP.Api.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace CCAP.Api.Services {
    public class CreditCardApplicationService : ICreditCardApplicationService {
        private readonly CCAPContext _context;

        public CreditCardApplicationService(CCAPContext context) {
            _context = context;
        }

        public async Task Register(CreditCardApplicationCreateViewModel viewModel) {
            await UpdateUserProfile(viewModel);
            var applicationDb = await CreateNewCreditCardApplication(viewModel);
            //  create the submit status (and save)
            var changeStatusViewModel = new CreditCardApplicationStatusChangeViewModel {
                CreditCardApplicationId = applicationDb.Id,
                Remarks = "Registered by the customer",
                Username = viewModel.Username
            };

            await ChangeApplicationStatus(changeStatusViewModel, StatusType.Submitted, (id) => Task.FromResult(true));
        }

        public async Task<IEnumerable<CreditCardApplicationViewModel>> GetPendingApproval() {
            var list = await _context
                .CreditCardApplications
                .Include(a => a.CreditCard)
                .Include(a => a.StatusList)
                .ThenInclude(s => s.AppUser)
                .ToListAsync();

            return list.Where(IsFreshApplication)
                .Select(a => new CreditCardApplicationViewModel {
                    CardCode = a.CreditCard.CardCode,
                    CardDescription = $"{a.CreditCard.Category}/{a.CreditCard.SubType}",
                    CreditCardId = a.CreditCard.Id,
                    CustomerId = GetCustomerId(a),
                    DateOfApplication = a.DateOfApplication,
                    FullName = GetCustomerFullName(a),
                    Status = StatusType.Submitted,
                    Id = a.Id,
                    ImageUrl = a.CreditCard.ImageUrl,
                    LimitRequired = a.LimitRequired,
                });
        }

        public async Task<IEnumerable<CreditCardApplicationViewModel>> GetPendingIssuance() {
            var list = await _context
                .CreditCardApplications
                .Include(a => a.CreditCard)
                .Include(a => a.StatusList)
                .ThenInclude(s => s.AppUser)
                .ToListAsync();

            return list
                .Where(CanBeIssued)
                .Select(a => new CreditCardApplicationViewModel {
                    CardCode = a.CreditCard.CardCode,
                    CardDescription = $"{a.CreditCard.Category}/{a.CreditCard.SubType}",
                    CreditCardId = a.CreditCard.Id,
                    CustomerId = GetCustomerId(a),
                    DateOfApplication = a.DateOfApplication,
                    FullName = GetCustomerFullName(a),
                    Status = StatusType.Approved,
                    Id = a.Id,
                    ImageUrl = a.CreditCard.ImageUrl,
                    LimitRequired = a.LimitRequired
                });
        }

        public async Task<CreditCardApplicationReportViewModel> GetApplication(int id) {
            var applicationDb = await GetOriginalApplication(id);
            var customerDb = GetCustomer(applicationDb);
            var communicationAddress = GetCommunicationAddress(customerDb);

            //  create the view model and return
            return new CreditCardApplicationReportViewModel {
                CustomerId = customerDb.Id,
                FirstName = customerDb.FirstName,
                LastName = customerDb.LastName,
                Username = customerDb.Email,
                MiddleName = customerDb.MiddleName,
                DateOfBirth = customerDb.DateOfBirth,
                Gender = customerDb.Gender,
                PAN = customerDb.PAN,
                PhoneNumber = customerDb.PhoneNumber,
                SecondaryPhoneNumber = customerDb.SecondaryPhoneNumber,
                Qualification = customerDb.Qualification,
                AnnualIncome = applicationDb.AnnualIncome,
                EmploymentStatus = applicationDb.EmploymentStatus,
                ApplicationId = applicationDb.Id,
                DateOfApplication = applicationDb.DateOfApplication,
                CreditCardId = applicationDb.CreditCard.Id,
                CreditCardCode = applicationDb.CreditCard.CardCode,
                CreditCardDetails = $"{applicationDb.CreditCard.Category}/{applicationDb.CreditCard.SubType}",
                LimitRequired = applicationDb.LimitRequired,
                AddressLine1 = communicationAddress.AddressLine1,
                AddressLine2 = communicationAddress.AddressLine2,
                AddressLine3 = communicationAddress.AddressLine3,
                City = communicationAddress.City,
                State = communicationAddress.State,
                PIN = communicationAddress.PIN,
            };
        }

        public async Task Approve(CreditCardApplicationStatusChangeViewModel viewModel) {
            await ChangeApplicationStatus(viewModel, StatusType.Approved, IsFreshApplication);
        }

        public async Task Reject(CreditCardApplicationStatusChangeViewModel viewModel) {
            await ChangeApplicationStatus(viewModel, StatusType.Rejected, IsFreshApplication);
        }

        public async Task IssueCard(CreditCardApplicationStatusChangeViewModel viewModel) {
            await ChangeApplicationStatus(viewModel, StatusType.Issued, CanBeIssued);
        }

        public async Task<int> GetTotalApplications() {
            return await _context.CreditCardApplications.CountAsync();
        }

        public async Task<int> GetTotalPendingApproval() {
            return await GetTotalApplications() - (await GetTotalApproved() + await GetTotalRejected());
        }

        public async Task<int> GetTotalPendingIssuance() {
            return await GetTotalApproved() - await GetTotalIssued();
        }

        public async Task<int> GetTotalApproved() {
            return await _context.ApplicationStatusList.CountAsync(a => a.Status == StatusType.Approved);
        }

        public async Task<int> GetTotalRejected() {
            return await _context.ApplicationStatusList.CountAsync(a => a.Status == StatusType.Rejected);
        }

        public async Task<int> GetTotalIssued() {
            return await _context.ApplicationStatusList.CountAsync(a => a.Status == StatusType.Issued);
        }

        public async Task<ApproverStatisticsViewModel> GetApproverStatistics() {
            return new ApproverStatisticsViewModel {
                TotalApplications = await GetTotalApplications(),
                TotalApproved = await GetTotalApproved(),
                TotalPendingApproval = await GetTotalPendingApproval(),
                TotalRejected = await GetTotalRejected()
            };
        }

        public async Task<IssuerStaticsViewModel> GetIssuerStatistics() {
            return new IssuerStaticsViewModel {
                TotalIssued = await GetTotalIssued(),
                TotalPendingIssuance = await GetTotalPendingIssuance()
            };
        }

        //  implementation details
        private async Task<bool> IsFreshApplication(int id) {
            //  make sure the application exists
            var applicationDb = await GetOriginalApplication(id);

            var approveRejectCount =
                applicationDb.StatusList.Count(s => s.Status == StatusType.Approved || s.Status == StatusType.Rejected);

            return approveRejectCount == 0;
        }

        private bool IsFreshApplication(CreditCardApplication applicationDb) {
            //  make sure the application exists
            var approveRejectCount =
                applicationDb.StatusList.Count(s => s.Status == StatusType.Approved || s.Status == StatusType.Rejected);

            return approveRejectCount == 0;
        }

        private async Task<bool> CanBeIssued(int id) {
            //  make sure the application exists
            var applicationDb = await GetOriginalApplication(id);
            return IsApproved(applicationDb) && !IsIssued(applicationDb);
        }

        private bool CanBeIssued(CreditCardApplication applicationDb) {
            //  make sure the application exists
            return IsApproved(applicationDb) && !IsIssued(applicationDb);
        }

        private bool IsApproved(CreditCardApplication applicationDb) {
            return applicationDb.StatusList.Count(s => s.Status == StatusType.Approved) == 1;
        }

        private bool IsIssued(CreditCardApplication applicationDb) {
            return applicationDb.StatusList.Count(s => s.Status == StatusType.Issued) == 1;
        }

        private async Task<CreditCardApplication> GetOriginalApplication(int id) {
            var applicationDb = await _context
                .CreditCardApplications
                .Include(a => a.CreditCard)
                .Include(a => a.StatusList)
                .ThenInclude(s => s.AppUser)
                .ThenInclude(u => u.Addresses)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (applicationDb == null) {
                throw new RecordNotFoundException($"Application with id: {id} is not found");
            }

            return applicationDb;
        }

        private async Task SaveChanges() {
            await _context.SaveChangesAsync();
        }

        private async Task<AppUser> GetUser(string username) {
            var userDb = await _context.AppUsers
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.Email == username);

            if (userDb == null) {
                throw new RecordNotFoundException($"User: {username} is not found");
            }

            return userDb;
        }

        private ApplicationStatus GetNewStatus(AppUser userDb, CreditCardApplication applicationDb,
            StatusType statusType, string remarks) {
            return new ApplicationStatus {
                Status = statusType,
                AppUser = userDb,
                AppUserId = userDb.Id,
                CreditCardApplication = applicationDb,
                CreditCardApplicationId = applicationDb.Id,
                DateOfProcessing = DateTime.UtcNow,
                Remarks = remarks
            };
        }

        private async Task ChangeApplicationStatus(CreditCardApplicationStatusChangeViewModel viewModel,
            StatusType statusType, Func<int, Task<bool>> predicate) {
            var canBeProcessed = await predicate(viewModel.CreditCardApplicationId);

            if (!canBeProcessed) {
                throw new DomainValidationException($"Application cannot be {statusType} as it is already processed");
            }

            var applicationDb = await GetOriginalApplication(viewModel.CreditCardApplicationId);

            var userDb = await GetUser(viewModel.Username);

            //  then put the new state as approved
            var newStatus = GetNewStatus(userDb, applicationDb, statusType, viewModel.Remarks);

            //  attach the status to the application
            applicationDb.StatusList.Add(newStatus);

            //  save
            await SaveChanges();
        }

        private int GetCustomerId(CreditCardApplication applicationDb) {
            var submitStatusForm = applicationDb.StatusList.FirstOrDefault(s => s.Status == StatusType.Submitted);

            if (submitStatusForm == null) {
                throw new RecordNotFoundException(
                    $"Application {applicationDb.Id} doesn't have the submit status form");
            }

            return submitStatusForm.AppUserId;
        }

        private AppUser GetCustomer(CreditCardApplication applicationDb) {
            var submitStatusForm = applicationDb.StatusList.FirstOrDefault(s => s.Status == StatusType.Submitted);

            if (submitStatusForm == null) {
                throw new RecordNotFoundException(
                    $"Application {applicationDb.Id} doesn't have the submit status form");
            }

            return submitStatusForm.AppUser;
        }


        private string GetCustomerFullName(CreditCardApplication applicationDb) {
            var submitStatusForm = applicationDb.StatusList.FirstOrDefault(s => s.Status == StatusType.Submitted);

            if (submitStatusForm == null) {
                throw new RecordNotFoundException(
                    $"Application {applicationDb.Id} doesn't have the submit status form");
            }

            if (submitStatusForm.AppUser == null) {
                throw new DomainValidationException(
                    $"AppUser is not included with the credit card application status list");
            }

            return $"{submitStatusForm.AppUser.FirstName} {submitStatusForm.AppUser.LastName}";
        }


        private async Task UpdateUserProfile(CreditCardApplicationCreateViewModel viewModel) {
            //  make sure the username exists
            var userDb = await _context.AppUsers
                .Include(u => u.Addresses)
                .FirstOrDefaultAsync(u => u.Email == viewModel.Username);

            if (userDb == null) {
                throw new RecordNotFoundException(
                    $"Could not find the user {viewModel.Username} for application registration");
            }

            userDb.MiddleName = viewModel.MiddleName;
            userDb.Gender = viewModel.Gender;
            userDb.Qualification = viewModel.Qualification;
            userDb.PhoneNumber = viewModel.PhoneNumber;
            userDb.SecondaryPhoneNumber = viewModel.SecondaryPhoneNumber;
            userDb.DateOfBirth = viewModel.DateOfBirth;
            userDb.PAN = viewModel.PAN;

            //  deal with addresses
            var newPermanentAddress = new Address {
                AddressLine1 = viewModel.PAddressLine1,
                AddressLine2 = viewModel.PAddressLine2,
                AddressLine3 = viewModel.PAddressLine3,
                AppUser = userDb,
                AppUserId = userDb.Id,
                City = viewModel.PCity,
                IsPermanent = true,
                PIN = viewModel.PPIN,
                State = viewModel.PState,
                UseForCommunication = viewModel.UsePermanentAddressForCommunication
            };

            userDb.Addresses.Add(newPermanentAddress);

            if (!viewModel.UsePermanentAddressForCommunication) {
                var newCommunicationAddress = new Address {
                    AddressLine1 = viewModel.CAddressLine1,
                    AddressLine2 = viewModel.CAddressLine2,
                    AddressLine3 = viewModel.CAddressLine3,
                    AppUser = userDb,
                    AppUserId = userDb.Id,
                    City = viewModel.CCity,
                    IsPermanent = false,
                    PIN = viewModel.CPIN,
                    State = viewModel.CState,
                    UseForCommunication = true
                };

                userDb.Addresses.Add(newCommunicationAddress);
            }

            await SaveChanges();
        }

        private async Task<CreditCard> GetCreditCard(int id) {
            var cardDb = await _context.CreditCards.FirstOrDefaultAsync(c => c.Id == id);

            if (cardDb == null) {
                throw new RecordNotFoundException($"Credit card id {id} not found");
            }

            return cardDb;
        }

        private async Task<CreditCardApplication> CreateNewCreditCardApplication(
            CreditCardApplicationCreateViewModel viewModel) {
            var application = new CreditCardApplication {
                AnnualIncome = viewModel.AnnualIncome,
                CreditCard = await GetCreditCard(viewModel.CreditCardId),
                CreditCardId = viewModel.CreditCardId,
                DateOfApplication = DateTime.UtcNow,
                EmploymentStatus = viewModel.EmploymentStatus,
                LimitRequired = viewModel.LimitRequired,
            };

            await _context.AddAsync(application);
            await SaveChanges();
            return application;
        }

        private Address GetCommunicationAddress(AppUser customer) {
            if (customer.Addresses.Count == 0) {
                throw new DomainValidationException($"Customer addresses are not included");
            }

            return customer.Addresses.FirstOrDefault(a => a.UseForCommunication);
        }
    }
}