namespace RealEstateAPI.DTOs.UserDTos.Login
{
    public class LoginResponseDTO
    {
        public string Email { get; set; }  
        public string UserId { get; set; }
        public string Role { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public DateTime Expier { get; set; }
    }
}
