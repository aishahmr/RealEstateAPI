namespace RealEstateAPI.DTOs.UserDTos.UserManagement
{
    public class GetUserDetailsResponse
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public bool IsBlocked { get; set; }
    }
}
