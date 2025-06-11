using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

public class PricePredictionService
{
    private readonly HttpClient _httpClient;

    public PricePredictionService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("http://localhost:5000/");
    }

    public async Task<int> PredictPriceAsync(PropertyData data)
    {
        var request = new
        {
           
            price2025 = data.Price2025,
            area = data.Area,
            bedrooms = data.Bedrooms,
            bathrooms = data.Bathrooms,
            propertyType = data.PropertyType,
            amenities = data.Amenities,
            nearbyFacilities = data.NearbyFacilities
        };

        var response = await _httpClient.PostAsJsonAsync("api/predict", request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<JsonElement>();
        return result.GetProperty("predicted_price").GetInt32();
    }
}

public class PropertyData
{
    
    public decimal Price2025 { get; set; }
    public int Area { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public string PropertyType { get; set; } // "Apartment" or "Villa"
    public string Amenities { get; set; }
    public string NearbyFacilities { get; set; }
}