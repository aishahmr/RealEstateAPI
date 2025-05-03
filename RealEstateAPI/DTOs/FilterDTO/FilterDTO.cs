namespace RealEstateAPI.DTOs.FilterDTO
{
    public class FilterDTO
    {
        public string UserId { get; set; }
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public string Location { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public string PropertyType { get; set; } // apartment, studio, villa
    }
}
