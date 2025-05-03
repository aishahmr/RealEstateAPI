using System;
using System.Threading.Tasks;

namespace RealEstateAPI.Services
{
    public interface IBookingService
    {
        Task<bool> BookPropertyAsync(Guid propertyId, string userId, DateTime checkIn, DateTime checkOut, string? note);
    }
}
