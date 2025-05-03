using System.ComponentModel.DataAnnotations;

namespace RealEstateAPI.DTOs.UserDTos.ResetPassword
{
    public class ResetPasswordRequestDTO
    {
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "OTP must be exactly 4 digits.")]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "OTP must contain only numbers.")]
        public string OTP { get; set; }

    }
}
