namespace RealEstateAPI.DTOs.UserDTos.Register
{
    public class RegisterResultDTO
    {
        public string Message { get; set; }  // e.g., "Existing", "Password", "Image", or "Success"
        public string? Error { get; set; }     // Optional: Details of the error if registration fails
    }
}
