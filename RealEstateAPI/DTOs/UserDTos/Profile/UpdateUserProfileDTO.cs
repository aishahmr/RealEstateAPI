namespace RealEstateAPI.DTOs.UserDTos.Profile
{
    public class UpdateUserProfileDTO
    {
        public string? Username { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Bio { get; set; }
        public string? Gender { get; set; }
        public string? City { get; set; }
        public string? Governorate { get; set; }
    }
}
