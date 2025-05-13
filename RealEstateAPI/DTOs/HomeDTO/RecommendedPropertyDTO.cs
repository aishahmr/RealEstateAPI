public class RecommendedPropertyDTO
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    //public string Location { get; set; } // "8 Stonepot Road, NY 070"
    public string City { get; set; }
    public string PriceFormatted { get; set; } // "$20,000"
    public string AreaFormatted { get; set; } // "2360 sqft"
    public string ImageUrl { get; set; }
}