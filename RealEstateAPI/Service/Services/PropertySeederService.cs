using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.Configuration.Attributes;
using Microsoft.EntityFrameworkCore;
using RealEstateAPI.Models;
using RealEstateAPI.Models.Data;

namespace RealEstateAPI.Service.Services
{
    public class PropertySeederService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;

        public PropertySeederService(AppDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task SeedProperties(bool forceReseed = false)
        {
            // Prevent duplicate seeding
            if (!forceReseed && await _context.Properties.AnyAsync())
            {
                return;
            }

            // Clear existing data if forcing reseed
            if (forceReseed)
            {
                _context.Properties.RemoveRange(_context.Properties);
                await _context.SaveChangesAsync();
            }

            var csvPath = Path.Combine(_env.ContentRootPath, "SeedData", "FinalDataset.csv");

            if (!File.Exists(csvPath))
                throw new FileNotFoundException($"CSV file not found at: {csvPath}");

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                HasHeaderRecord = true,
                IgnoreBlankLines = true,
                BadDataFound = null,
                PrepareHeaderForMatch = args => args.Header
                    .Replace(" ", "")
                    .Replace("(EGP)", "")
                    .Trim()
            };

            using var reader = new StreamReader(csvPath);
            using var csv = new CsvReader(reader, config);

            List<PropertyCSVRow> records;
            try
            {
                records = csv.GetRecords<PropertyCSVRow>().ToList();
            }
            catch (CsvHelper.HeaderValidationException ex)
            {
                throw new Exception("CSV header validation failed", ex);
            }

            var userId = _context.Users.First().Id;

            foreach (var row in records)
            {
                _context.Properties.Add(new Property
                {
                    Id = Guid.NewGuid(),
                    Title = $"{row.Type} in {row.City}",
                    Description = GenerateDescription(row),
                    Price2025 = row.Price2025,
                    Price2024 = row.Price2024,
                    Price2023 = row.Price2023,
                    AddressLine1 = row.City,
                    City = DeriveCityFromLocation(row.City),
                    Governorate = DeriveGovernorateFromLocation(row.City),
                    Size = row.Size,
                    Bedrooms = row.Bedrooms,
                    Bathrooms = row.Bathrooms,
                    FloorLevel = row.FloorLevel,
                    Type = row.Type,
                    FurnishingStatus = row.FurnishingStatus,
                    Amenities = row.Amenities,
                    NearbyFacility = row.NearbyFacility,
                    CreatedAt = DateTime.UtcNow,
                    VerificationStatus = "Approved",
                    UserId = userId,
                    DocumentUrl = "/documents/default.pdf",
                    IsFeatured = ShouldFeatureProperty(row.City),
                    yourName = "RealEstate Admin",
                    MobilePhone = "+201234567890"
                });
            }

            await _context.SaveChangesAsync();
        }

        private string GenerateDescription(PropertyCSVRow row)
        {
            return $"Beautiful {row.Type} in {row.City}. " +
                   $"{row.Size} sqm with {row.Bedrooms} bedrooms and {row.Bathrooms} bathrooms. " +
                   $"Built {row.BuildingAge} years ago. Nearby: {row.NearbyFacility}. " +
                   $"Amenities include: {row.Amenities}";
        }

        private bool ShouldFeatureProperty(string location)
        {
            return location.Contains("Sheikh Zayed") || location.Contains("New Cairo");
        }

        private string DeriveCityFromLocation(string location)
        {
            return location.Split(',').First().Trim();
        }

        private string DeriveGovernorateFromLocation(string location)
        {
            if (location.Contains("6th of October")) return "Giza";
            if (location.Contains("Sheikh Zayed")) return "Giza";
            if (location.Contains("Nasr City")) return "Cairo";
            return "Cairo";
        }
    }
}