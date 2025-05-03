using System.ComponentModel.DataAnnotations;


namespace RealEstateAPI.DTOs.UserDTos.ResetPassword
{
    public class VerifyOTPDTO
    {
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "OTP must be exactly 4 digits.")]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "OTP must contain only numbers.")]
        public string OTP { get; set; }
    }
}
   


