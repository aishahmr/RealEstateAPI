namespace RealEstateAPI.DTOs.UserDTos.UserManagement
{
    public class UpdateUserDTO
    {
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string? ImageUrl { get; set; }
    }
}
