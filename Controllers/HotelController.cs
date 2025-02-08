
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trainning.Common.Mappers;
using Trainning.Data;
using Trainning.DTO;
using Trainning.DTO.Updates;
using Trainning.Entities;
using Trainning.Interfaces;
using System.IO;
namespace Trainning.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class HotelController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileUploadService _fileUploadService;

        public HotelController(ApplicationDbContext context, IFileUploadService fileUploadService)
        {
            _context = context;
            _fileUploadService = fileUploadService;
        }
        [HttpGet("all")]
        public async Task<IResult> GetHotels()
        {
            try
            {
                var hotels = (await _context.Hotels.Include(r=>r.Users).Include(r => r.Rooms).ToListAsync()).Select(s => s.ToHotelDTO());
             

                return Results.Ok(new { data = hotels });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { ex });
                throw;
            }
        }
        [HttpGet]
        public async Task<IResult> GetHotel([FromQuery] int id)
        {
            try
            {
                var hotel = (await _context.Hotels.Include(r=>r.Users).Include(r => r.Rooms).FirstOrDefaultAsync(x => x.Id == id))?.ToHotelDTO();

                return Results.Ok(new { data = hotel });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { ex });
                throw;
            }
        }
        [HttpPost]
        public async Task<IResult> CreateHotel([FromForm] CreateHotelDTO dto)
        {
            var thumbnail = string.Empty;
            List<string> images = [];
            try
            {

                if (dto.Thumbnail != null && dto.Images.Count > 0)
                {
                    thumbnail = await _fileUploadService.UploadSingleFile(["uploads", "images", "hotels", "thumbnail"], dto.Thumbnail);
                    images = await _fileUploadService.UploadMultipleFiles(["uploads", "images", "hotels", "images"], dto.Images);
                }
                var newHotel = new Hotel
                {
                    Name = dto.Name,
                    Address = dto.Address,
                    Phone = dto.Phone,
                    Email = dto.Email,
                    Thumbnail = thumbnail,
                    Images = images,
                    Stars = dto.Stars,
                    CheckinTime = dto.CheckinTime,
                    CheckoutTime = dto.CheckoutTime,
                };
                await _context.Hotels.AddAsync(newHotel);
                await _context.SaveChangesAsync();
                return Results.Ok(new { data = newHotel.ToHotelDTO() });
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(thumbnail))
                {
                    _fileUploadService.DeleteSingleFile(thumbnail);
                }
                if (images.Count > 0)
                {
                    _fileUploadService.DeleteMultipleFiles(images);
                }
                var errorResponse = new
                {
                    Error = "An error occurred while creating the hotel",
                    Details = ex.Message
                };
                return Results.BadRequest(errorResponse);
            }
        }
        //     [HttpPut]
        //     public async Task<IResult> UpdateHotel([FromQuery] int id, [FromForm] CreateHotelDTO dto, CancellationToken cancellationToken)
        //     {
        //         try
        //         {
        //             var hotel = await _context.Hotels.FirstOrDefaultAsync(s => s.Id == id, cancellationToken: cancellationToken);
        //             if (hotel == null) return Results.NotFound();

        //             // Delete old thumbnail and images
        //             if (!string.IsNullOrEmpty(hotel.Thumbnail))
        //             {
        //                 _fileUploadService.DeleteSingleFile(hotel.Thumbnail);
        //             }

        //             if (hotel.Images != null && hotel.Images.Count > 0)
        //             {
        //                 _fileUploadService.DeleteMultipleFiles(hotel.Images);
        //             }

        //             // Upload new files if they exist
        //             string newThumbnail = null!;
        //             List<string> newImages = [];

        //             if (dto.Thumbnail != null)
        //             {
        //                 newThumbnail = await _fileUploadService.UploadSingleFile(["uploads", "images", "hotels", "thumbnail"], dto.Thumbnail);
        //             }

        //             if (dto.Images != null && dto.Images.Count > 0)
        //             {
        //                 newImages = await _fileUploadService.UploadMultipleFiles(["uploads", "images", "hotels", "images"], dto.Images);
        //             }

        //             // Update hotel entity
        //             hotel.Name = dto.Name;
        //             hotel.Address = dto.Address;
        //             hotel.Phone = dto.Phone;
        //             hotel.Email = dto.Email;
        //             hotel.Thumbnail = newThumbnail;
        //             hotel.Images = newImages;
        //             hotel.Stars = dto.Stars;
        //             hotel.CheckinTime = dto.CheckinTime;
        //             hotel.CheckoutTime = dto.CheckoutTime;

        //             _context.Hotels.Update(hotel);
        //             await _context.SaveChangesAsync(cancellationToken);

        //             return Results.Ok(new { data = hotel.ToHotelDTO() });
        //         }
        //         catch (Exception ex)
        //         {
        //             return Results.BadRequest(new { ex });
        //             throw;
        //         }
        //     }


        [HttpPut]
        public async Task<IResult> UpdateHotel([FromQuery] int id, [FromForm] UpdateHotelDTO dto, CancellationToken cancellationToken)
        {
            // Handle images
            var newImages = new List<string>();
            var uploadedImages = new List<string>();
            var thumbnail = string.Empty;
            try
            {
                var hotel = await _context.Hotels.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
                if (hotel == null) return Results.NotFound();

                // Handle thumbnail
                if (!string.IsNullOrEmpty(dto.ExistsThumbnail) && dto.Thumbnail == null)
                {
                    // Keep the existing thumbnail
                    thumbnail = dto.ExistsThumbnail;
                }
                else if (dto.Thumbnail != null)
                {
                    // Delete old thumbnail if it exists and a new one is uploaded
                    if (!string.IsNullOrEmpty(hotel.Thumbnail))
                    {
                        _fileUploadService.DeleteSingleFile(hotel.Thumbnail);
                    }

                    // Upload new thumbnail
                    thumbnail = await _fileUploadService.UploadSingleFile(new[] { "uploads", "images", "hotels", "thumbnail" }, dto.Thumbnail);
                }


                // Add existing images if they were not updated
                if (dto.ExistingImages != null && dto.ExistingImages.Count > 0)
                {
                    newImages.AddRange(dto.ExistingImages);
                }

                // Upload new images if any were provided
                if (hotel.Images != null && hotel.Images.Count > 0)
                {
                    var imagesToDelete = hotel.Images.Except(newImages).ToList();
                    if (imagesToDelete.Count > 0)
                    {
                        _fileUploadService.DeleteMultipleFiles(imagesToDelete);
                    }
                }
                // Upload new images if any were provided
                if (dto.Images != null && dto.Images.Count > 0)
                {
                    uploadedImages = await _fileUploadService.UploadMultipleFiles(new[] { "uploads", "images", "hotels", "images" }, dto.Images);
                    if (uploadedImages.Count > 0)
                    {
                        newImages.AddRange(uploadedImages);
                    }
                }


                // Update hotel entity with new data
                hotel.Name = !string.IsNullOrEmpty(dto.Name) ? dto.Name : hotel.Name;
                hotel.Thumbnail = thumbnail;
                hotel.Address = !string.IsNullOrEmpty(dto.Address) ? dto.Address : hotel.Address;
                hotel.Phone = !string.IsNullOrEmpty(dto.Phone)?dto.Phone : hotel.Phone;
                hotel.Email =  !string.IsNullOrEmpty(dto.Email)?dto.Email: hotel.Email;
                hotel.Images = newImages;
                hotel.Stars = dto.Stars;
                hotel.CheckinTime = dto.CheckinTime;
                hotel.CheckoutTime = dto.CheckoutTime;

                // Save changes
                _context.Hotels.Update(hotel);
                await _context.SaveChangesAsync();

                return Results.Ok(new { data = hotel.ToHotelDTO() });
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(thumbnail))
                {
                    _fileUploadService.DeleteSingleFile(thumbnail);
                }
                if (newImages.Count > 0)
                {
                    _fileUploadService.DeleteMultipleFiles(uploadedImages);
                }
                return Results.BadRequest(new { ex });
            }
        }

        [HttpDelete]
        public async Task<IResult> DeleteHotel([FromQuery] int id, CancellationToken cancellationToken)
        {
            try
            {
                var hotel = await _context.Hotels.FirstOrDefaultAsync(x => x.Id == id);
                if (hotel == null) return Results.NotFound();
                if (!string.IsNullOrEmpty(hotel.Thumbnail))
                {
                    _fileUploadService.DeleteSingleFile(hotel.Thumbnail);
                }
                if (hotel.Images.Count > 0)
                {
                    _fileUploadService.DeleteMultipleFiles(hotel.Images);
                }

                _context.Hotels.Remove(hotel);
                await _context.SaveChangesAsync(cancellationToken);
                return Results.Ok(new
                {
                    Message = @"Delete Hotel with id :{0} successfully !"
                });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { ex });
            }
        }
    }
}