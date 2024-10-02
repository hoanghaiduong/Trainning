
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trainning.Data;
using Trainning.Interfaces;

namespace Trainning.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class BookingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }
        [HttpGet("all")]
        public async Task<IResult> GetBookings()
        {
            try
            {
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { ex });
                throw;
            }
        }
        [HttpGet]
        public async Task<IResult> GetBooking()
        {
            try
            {
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { ex });
                throw;
            }
        }
        [HttpPost]
        public async Task<IResult> CreateBooking()
        {
            try
            {
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { ex });
                throw;
            }
        }
        [HttpPut]
        public async Task<IResult> UpdateBooking()
        {
            try
            {
                return Results.Ok();
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { ex });
                throw;
            }
        }
        [HttpDelete]
        public async Task<IResult> DeleteBooking([FromQuery] int id)
        {
            try
            {
                var booking = await _context.Bookings.FirstOrDefaultAsync(x => x.Id == id);
                if (booking == null)
                {
                    return Results.NotFound();
                }
                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
                return Results.Ok(new { message = "Delete Booking Successfully" });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { ex });
                throw;
            }
        }
    }
}