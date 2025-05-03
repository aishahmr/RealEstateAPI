using System;
using System.Threading.Tasks;
using RealEstateAPI.Models;
using RealEstateAPI.Models.Data;

namespace RealEstateAPI.Services
{
    public class BookingService : IBookingService
    {
        private readonly AppDbContext _context;

        public BookingService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> BookPropertyAsync(Guid propertyId, string userId, DateTime checkIn, DateTime checkOut, string? note)
        {
            var booking = new Booking
            {
                Id = Guid.NewGuid(),
                PropertyId = propertyId,
                UserId = userId,
                CheckIn = checkIn,
                CheckOut = checkOut,
                NoteToOwner = note,
                CreatedAt = DateTime.UtcNow
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
