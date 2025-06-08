namespace RealEstateAPI.DTOs.PriceEstimateDTO
{
    public class PriceEstimateDTO
    {
        public decimal Price2023 { get; set; }       
        public decimal Price2024 { get; set; }       
        public decimal Price2025 { get; set; }       
        public int AreaSqm { get; set; }            
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public string PropertyType { get; set; }     
        public string Amenities { get; set; }        
        public string NearbyFacility { get; set; }

        
        public Guid? Id { get; set; }
        public Guid? PropertyId { get; set; }
        public decimal? EstimatedPrice { get; set; } // Will be populated by the model
        public DateTime? EstimatedAt { get; set; }
    }
}