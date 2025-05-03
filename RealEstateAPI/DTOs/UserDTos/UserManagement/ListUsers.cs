namespace RealEstateAPI.DTOs.UserDTos.UserManagement
{
    public class ListUsers
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? Address { get; set; }
        public string? Region { get; set; }
        public string? Gender { get; set; }
        public string? Role { get; set; }
        public string? ImageUrl { get; set; }
    }
}
