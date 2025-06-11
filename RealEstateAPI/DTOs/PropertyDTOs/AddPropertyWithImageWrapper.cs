using Microsoft.AspNetCore.Mvc;

namespace RealEstateAPI.DTOs.PropertyDTOs

{
    public class AddPropertyWithImageWrapper
    {
        public string PropertyJson { get; set; } = string.Empty;

       [FromForm(Name = "files")]
    public List<IFormFile> Files { get; set; }
    }
}
