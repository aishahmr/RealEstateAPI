namespace RealEstateAPI.DTOs.FavoriteDTO
{
    public class FavoriteDTO
    {
        public string UserId { get; set; }
        public Guid PropertyId { get; set; }
    }

    public class FavoritePropertyDTO
    {
        public string UserId { get; set; }
        public Guid PropertyId { get; set; }
        public string Title { get; set; }           // → Property name in card
        public string Address { get; set; }         // → Address line
        public decimal Price { get; set; }
        public string FormattedPrice { get; set; }  // Formatted for display
                                                    // → Formatted as currency
        public int Area { get; set; }               // → Shown as "XXX sqft"
        public string MainImageUrl { get; set; }    

    }
}
