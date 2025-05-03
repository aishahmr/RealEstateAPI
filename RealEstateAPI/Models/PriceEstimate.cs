using System;

namespace RealEstateAPI.Models
{
    public class PriceEstimate
    {
        public Guid Id { get; set; }  // Primary Key
        public Guid PropertyId { get; set; }  // Foreign Key (Property)
        public decimal EstimatedPrice { get; set; }
        public DateTime EstimatedAt { get; set; }

        // Navigation Property
        public Property Property { get; set; }
    }
}
