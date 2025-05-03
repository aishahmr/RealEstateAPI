using RealEstateAPI.DTOs.UserDTos.Register;
using RealEstateAPI.DTOs.UserDTos.Login;
using RealEstateAPI.DTOs.UserDTos.Administration;
using RealEstateAPI.DTOs.UserDTos.ResetPassword;
using RealEstateAPI.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RealEstateAPI.Service.IServices
{
    public interface IAuthService
    {
        Task<RegisterResultDTO> RegisterAsync(RegisterDTO model);
        Task<LoginResponseDTO> Login(LoginDTO model);
        Task<bool> SendPasswordResetOTP(PhoneOnlyDTO model);
        Task<bool> ValidateOTP(VerifyOTPDTO model);
        Task<bool> ResetPassword(ResetPasswordRequestDTO passwordModel);
        Task<List<ListUsers>> ListOfUsers();
        Task<string> UserActivation(UserActivation activation);
        Task<ApplicationUser?> GetUserById(string userId);
    }
}
