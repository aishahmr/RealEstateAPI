using System.ComponentModel.DataAnnotations;

public class UploadProfilePictureDTO
{
    [Required]
    public IFormFile File { get; set; }
}

