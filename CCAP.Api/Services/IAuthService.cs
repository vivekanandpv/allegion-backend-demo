using System.Threading.Tasks;
using CCAP.Api.ViewModels;

namespace CCAP.Api.Services {
    public interface IAuthService {
        Task Register(GeneralUserRegisterViewModel viewModel);
        Task Register(StaffUserRegisterViewModel viewModel);
        Task<TokenViewModel> Login(LoginViewModel viewModel);
        bool Exists(string email);
    }
}