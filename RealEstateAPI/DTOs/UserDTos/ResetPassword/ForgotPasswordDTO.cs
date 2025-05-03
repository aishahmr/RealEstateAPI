namespace RealEstateAPI.DTOs.UserDTos.ResetPassword
{
    public class ForgotPasswordDTO
    {
        public string PhoneNumber { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
