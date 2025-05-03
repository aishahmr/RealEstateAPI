using Microsoft.AspNetCore.Mvc;
using RealEstateAPI.DTOs.UserDTos.Login;
using RealEstateAPI.DTOs.UserDTos.Register;
using RealEstateAPI.DTOs.UserDTos.ResetPassword;
using RealEstateAPI.Service.IServices;
using System.Threading.Tasks;

namespace RealEstateAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] RegisterDTO dto)
        {
            var result = await _authService.RegisterAsync(dto);
            if (result.Message == "Existing") return BadRequest(new { code = result.Message, message = result.Error });
            if (result.Message == "Password") return BadRequest(new { code = result.Message, message = result.Error });
            if (result.Message == "Image") return BadRequest(new { code = result.Message, message = result.Error });
            return Ok();
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var result = await _authService.Login(dto);
            if (!result.IsAuthenticated) return BadRequest(result.Message);
            return Ok(result);
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] PhoneOnlyDTO dto)
        {
            var result = await _authService.SendPasswordResetOTP(dto);
            if (!result)
                return BadRequest("Failed to send OTP. please enter the correct phone mumber");
            return Ok("OTP sent successfully.");
        }

        [HttpPost("VerifyOTP")]
        public async Task<IActionResult> VerifyOTP([FromBody] VerifyOTPDTO dto)
        {
            var isValid = await _authService.ValidateOTP(dto);
            if (!isValid)
                return BadRequest("Invalid OTP.");
            return Ok("OTP verified. You can now reset your password.");
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDTO dto)
        {
            var result = await _authService.ResetPassword(dto);
            if (!result)
                return BadRequest("Failed to reset password. Please try again.");
            return Ok("Password reset successfully.");
        }
    }
}
