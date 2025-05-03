namespace RealEstateAPI.DTOs.UserDTos.Administration
{
    public class ListUsers
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string? ImageUrl { get; set; }
    }
}

