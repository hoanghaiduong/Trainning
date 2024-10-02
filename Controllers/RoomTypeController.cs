
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trainning.Common.Mappers;
using Trainning.Data;
using Trainning.DTO;
using Trainning.DTO.Updates;
using Trainning.Entities;


namespace Trainning.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class RoomTypeController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public RoomTypeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("all")]
        public async Task<IResult> GetRoomTypes()
        {
            try
            {
                var roomTypes = (await _context.RoomTypes.Include(r => r.Rooms).ToListAsync()).Select(x => x.ToRoomTypeDTO());
                return Results.Ok(new { data = roomTypes });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { ex });
                throw;
            }
        }
        [HttpGet]
        public async Task<IResult> GetRoomType([FromQuery] int id)
        {
            try
            {
                var roomType = await _context.RoomTypes.Include(x => x.Rooms).FirstOrDefaultAsync(x => x.Id == id);
                return roomType != null ? Results.Ok(new { data = roomType.ToRoomTypeDTO() }) : Results.NotFound(new { Message = "Room type not found" });

            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { ex });

            }
        }


        [HttpPost]
        public async Task<IResult> CreateRoomType([FromBody] CreateRoomTypeDTO dto)
        {
            try
            {
                var newRoom = new RoomType
                {
                    Name = dto.Name,
                    Description = dto.Description,
                    PricePerNight = dto.PricePerNight,
                    Capacity = dto.Capacity,

                };
                await _context.RoomTypes.AddAsync(newRoom);
                await _context.SaveChangesAsync();
                return Results.Ok(new { data = newRoom.ToRoomTypeDTO() });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { ex });

            }
        }
        [HttpPut]
        public async Task<IResult> UpdateRoomType([FromQuery] int id, [FromBody] UpdateRoomTypeDTO dto)
        {
            try
            {
                var roomType = await _context.RoomTypes.FirstOrDefaultAsync(x => x.Id == id);
                if (roomType == null) return Results.NotFound();
                roomType.Name = dto.Name;
                roomType.Description = dto.Description;
                roomType.PricePerNight = dto.PricePerNight;
                roomType.Capacity = dto.Capacity;
                _context.RoomTypes.Update(roomType);
                await _context.SaveChangesAsync();
                return Results.Ok(new { data = roomType.ToRoomTypeDTO() });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { ex });

            }
        }
        [HttpDelete]
        public async Task<IResult> DeleteRoomType([FromQuery] int id)
        {
            try
            {
                var roomType = await _context.RoomTypes.FirstOrDefaultAsync(x => x.Id == id);
                if (roomType == null) return Results.NotFound();
                _context.RoomTypes.Remove(roomType);
                await _context.SaveChangesAsync();
                return Results.Ok(new { message = "Delete room type successfully" });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { ex });
                throw;
            }
        }
    }


}