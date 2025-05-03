namespace RealEstateAPI.DTOs.PriceEstimateDTO
{
    public class PriceEstimateDTO
    {
        public Guid Id { get; set; }
        public Guid PropertyId { get; set; }
        public decimal EstimatedPrice { get; set; }
        public DateTime EstimatedAt { get; set; }
    }
}
