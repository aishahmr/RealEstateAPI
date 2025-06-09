namespace RealEstateAPI.DTOs.PropertyDTOs
{
    public class UpdatePropertyDTO : AddPropertyDTO
    {
        public Guid Id { get; set; }

    }

    public class UpdatePropertyImagesDTO
    {
        public List<IFormFile>? NewImages { get; set; }
        public List<string>? ImagesToRemove { get; set; }
    }


}
