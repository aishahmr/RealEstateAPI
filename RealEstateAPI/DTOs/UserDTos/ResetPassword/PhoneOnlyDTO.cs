using System.ComponentModel.DataAnnotations;

namespace RealEstateAPI.DTOs.UserDTos.ResetPassword
{
    public class PhoneOnlyDTO
    {
        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
    }
}

