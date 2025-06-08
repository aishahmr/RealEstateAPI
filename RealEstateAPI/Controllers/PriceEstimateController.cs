using Microsoft.AspNetCore.Mvc;
using RealEstateAPI.DTOs.PriceEstimateDTO;
using  RealEstateAPI.DTOs;
using RealEstateAPI.Services;

// PriceEstimateController.cs
[ApiController]
[Route("api/[controller]")]
public class PriceEstimateController : ControllerBase
{
    private readonly PricePredictionService _predictionService;

    public PriceEstimateController(PricePredictionService predictionService)
    {
        _predictionService = predictionService;
    }

    [HttpPost("predict")]
    public async Task<IActionResult> Predict([FromBody] PropertyData data)
    {
        try
        {
            var predictedPrice = await _predictionService.PredictPriceAsync(data);
            return Ok(new { PredictedPrice = predictedPrice });
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Prediction error: {ex.Message}");
        }
    }
}