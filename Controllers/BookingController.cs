
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trainning.Common.Mappers;
using Trainning.Data;
using Trainning.DTO.Creates;
using Trainning.DTO.Updates;
using Trainning.Entities;
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
                var bookings = (await _context.Bookings.Include(r => r.User).Include(r => r.Room).Include(r => r.Payments).ToListAsync()).Select(x => x.ToBookingDTO());
                return Results.Ok(new
                {
                    message = "Get All Bookings",
                    data = bookings
                });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { ex });
                throw;
            }
        }
        [HttpGet]
        public async Task<IResult> GetBooking([FromQuery] int id)
        {
            try
            {
                var booking = await _context.Bookings.Include(r => r.User).Include(r => r.Room).Include(r => r.Payments).FirstOrDefaultAsync(x => x.Id == id);
                if (booking == null)
                {
                    return Results.NotFound(new { message = "Booking Not Found" });
                }
                return Results.Ok(new
                {
                    message = "Get Booking Successfully",
                    data = booking
                });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { ex });
                throw;
            }
        }
        [HttpPost]
        public async Task<IResult> CreateBooking([FromBody] CreateBookingDTO dto)
        {
            try
            {
               
                var room = await _context.Rooms.FirstOrDefaultAsync(x => x.Id == dto.RoomId);
                if (room == null)
                {
                    return Results.NotFound(new { message = "Room Not Found" });
                }
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == dto.UserId);
                if (user == null)
                {
                    return Results.NotFound(new { message = "User Not Found" });
                }

                var newBooking = new Booking
                {
                    Room = room,
                    User = user,
                    CheckinDate = dto.CheckinDate,
                    CheckoutDate = dto.CheckoutDate,
                    TotalPrice = dto.TotalPrice,
                };
                var inserted = await _context.Bookings.AddAsync(newBooking);
                await _context.SaveChangesAsync();
                return Results.Ok(new
                {
                    message = "Created new booking successfully!",
                    data = newBooking.ToBookingDTO()
                });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { ex });
                throw;
            }
        }
        [HttpPut]
        public async Task<IResult> UpdateBooking([FromQuery] int id, [FromBody] UpdateBookingDTO dto)
        {
            try
            {
                var booking = await _context.Bookings.Include(r => r.User).Include(r => r.Room).Include(r => r.Payments).FirstOrDefaultAsync(x => x.Id == id);
                if (booking == null)
                {
                    return Results.NotFound(new { message = "Booking Not Found" });
                }
                if (dto.RoomId != null)
                {
                    var room = await _context.Rooms.FirstOrDefaultAsync(x => x.Id == dto.RoomId);
                    if (room == null)
                    {
                        return Results.NotFound(new { message = "Room Not Found" });
                    }
                    else
                    {
                        booking.Room = room;
                    }
                }
                if (dto.UserId != null)
                {
                    var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == dto.UserId);
                    if (user == null)
                    {
                        return Results.NotFound(new { message = "User Not Found" });
                    }
                    else
                    {
                        booking.User = user;
                    }
                }
                booking.CheckinDate = dto.CheckinDate ?? booking.CheckinDate;
                booking.CheckoutDate = dto.CheckoutDate ?? booking.CheckoutDate;
                booking.TotalPrice = dto.TotalPrice ?? booking.TotalPrice;
                var updated = _context.Bookings.Update(booking);
                await _context.SaveChangesAsync();
                return Results.Ok(new
                {
                    Message = "Update Booking Successfully",
                    Data = booking.ToBookingDTO()
                });
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