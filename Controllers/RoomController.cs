using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trainning.Common.Enums;
using Trainning.Common.Mappers;
using Trainning.Data;
using Trainning.DTO;
using Trainning.DTO.Creates;
using Trainning.DTO.Updates;
using Trainning.Entities;
using Trainning.Interfaces;

namespace Trainning.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class RoomController : ControllerBase
    {

        private readonly ApplicationDbContext _context;
        private readonly IFileUploadService _fileUploadService;
        public RoomController(ApplicationDbContext context, IFileUploadService fileUploadService)
        {

            _context = context;
            _fileUploadService = fileUploadService;
        }
        [HttpGet("all")]
        public async Task<IResult> GetRooms()
        {
            try
            {
                var rooms = await _context.Rooms.Include(i => i.Hotel).Include(i => i.RoomType).ToListAsync();
                var roomMapper = rooms.Select(s => s.ToRoomDTO());

                return Results.Ok(new { roomMapper });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { ex });

            }
        }
        [HttpGet]
        public async Task<IResult> GetRoom([FromQuery] int id)
        {
            try
            {
                var room = await _context.Rooms.FirstOrDefaultAsync(x => x.Id == id);
                if (room == null) return Results.NotFound();
                var roomResult = room.ToRoomDTO();
                return Results.Ok(new { data = roomResult });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { ex });
            }
        }
        [HttpPost]
        public async Task<IResult> CreateRoom([FromForm] CreateRoomDTO dto)
        {
            var thumbnail = string.Empty;
            List<string> images = [];
            try
            {


                var hotel = await _context.Hotels.FirstOrDefaultAsync(x => x.Id == dto.HotelID);
                if (hotel == null) return Results.NotFound();
                var roomType = await _context.RoomTypes.FirstOrDefaultAsync(x => x.Id == dto.RoomTypeId);
                if (roomType == null) return Results.NotFound();
                if (dto.Thumbnail != null)
                {
                    thumbnail = await _fileUploadService.UploadSingleFile(["uploads", "images", "rooms", "thumbnail"], dto.Thumbnail);
                }
                if (dto.Images != null && dto.Images.Count > 0)
                {
                    images = await _fileUploadService.UploadMultipleFiles(["uploads", "images", "rooms", "images"], dto.Images);
                }

                var newRoom = new Room
                {
                    RoomNumber = dto.RoomNumber,
                    Thumbnail = thumbnail,
                    Images = images,
                    Hotel = hotel,
                    RoomType = roomType,
                    Status = dto.Status
                };
                await _context.Rooms.AddAsync(newRoom);
                await _context.SaveChangesAsync();
                return Results.Ok(new { newRoom });
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
                return Results.BadRequest(new { ex });
            }
        }
        [HttpPut]
        public async Task<IResult> UpdateRoom([FromQuery] int id, [FromForm] UpdateRoomDTO dto)
        {
            var thumbnail = string.Empty;
            var newImages = new List<string>();
            var addImages = new List<string>();
            try
            {


                var room = await _context.Rooms.FirstOrDefaultAsync(x => x.Id == id);
                if (room == null) return Results.NotFound();
                if (dto.HotelID != room.HotelID)
                {
                    var hotel = await _context.Hotels.FirstOrDefaultAsync(x => x.Id == dto.HotelID);
                    if (hotel == null) return Results.NotFound();
                    room.Hotel = hotel;
                }
                if (dto.RoomTypeId != room.RoomTypeId)
                {
                    var roomType = await _context.RoomTypes.FirstOrDefaultAsync(x => x.Id == dto.RoomTypeId);
                    if (roomType == null) return Results.NotFound();
                    room.RoomType = roomType;
                }
                //mặc định khi cập nhật chắc chắn có ảnh //exists thumbnail and images
                //Kiểm tra nếu có ảnh (thumbnail) cũ và không upload ảnh mới thì giữ lại, không có => xoá
                if (!string.IsNullOrEmpty(dto.ExistsThumbnail) && dto.Thumbnail == null)
                {
                    room.Thumbnail = dto.ExistsThumbnail;
                }
                //còn nếu có ảnh cũ và truyền ảnh mới thì xoá ảnh cũ đi thêm ảnh mới vào
                else if (!string.IsNullOrEmpty(dto.ExistsThumbnail) && dto.Thumbnail != null)
                {
                    //xoá ảnh cũ đi
                    _fileUploadService.DeleteSingleFile(room.Thumbnail);
                    //thêm ảnh mới
                    thumbnail = await _fileUploadService.UploadSingleFile(new[] { "uploads", "images", "rooms", "thumbnail" }, dto.Thumbnail);

                }

                //room.images=["url1","url2","url3"];//ảnh hiện có trong csdl
                //dto.images= new Files{img1,img2}; // ảnh trong request
                //dto.ExistsImages=["url1","url2","url3"] //ảnh mặc định nếu không cập nhật gì về mảng ảnh này
                //giả sử xoá ảnh là url2 => [url1,url3] thì dto.ExistsImages= [url1,url3]
                //kiểm tra nếu có images cũ thì giữ lại bằng cách so sánh 2 mảng , không có cái nào xoá;
                if (dto.ExistsImages != null && dto.ExistsImages.Count > 0) //nếu có thay đổi ví dụ xoá url2 thì truyền 1 và 3 vào tức là sẽ chạy vào đây
                {
                    //lọc những hình ảnh vô lý thêm sai ví dụ string; => [url1,url2,url3,string]
                    //[url1,url2,url3]
                    var notExists = room.Images.Intersect(dto.ExistsImages).ToList();
                    if (notExists.Count > 0)
                    {
                        newImages.AddRange(notExists);
                    }
                    //thêm các ảnh đã có trong csdl vào mảng 1
                    //Add url1 và url3 vào 

                    //hiện tại mảng newImages =[url1,url3]
                }
                //Kiểm tra nếu có mảng url ảnh cũ trong csdl và so sánh với mảng url đã lọc 

                if (room.Images != null && room.Images.Count > 0)//kiểm tra xem mảng ảnh cũ trong csdl có giá trị
                {
                    //chạy vào đây 
                    //mảng hiện tại 
                    //room.Images=["url1","url2","url3"]
                    //newImages=[url1,url3]
                    //Except so sánh => ví dụ datasource 1 =[1,2,3,4,5,6]
                    //                        datasource 2= [1,3,5,8,9,10]
                    //                        outsource =[2,4,6]
                    //suy ra có trong mảng 1 mà khong có trong mảng 2 thì lấy 
                    //áp dụng so sánh ta có "url2"
                    //xoá ảnh cũ đi
                    var imagesToDelete = room.Images.Except(newImages).ToList(); //[url2]
                    if (imagesToDelete.Count > 0)
                    {
                        _fileUploadService.DeleteMultipleFiles(imagesToDelete);//delete
                    }
                }
                //tới đây thì chỉ còn newImages [url1,url3]
                //Kiểm tra xem có upload thêm ảnh hay 
                //giả sử upload thêm 1 cái gọi là url4 thì lúc này  newImages [url1,url3,url4]
                if (dto.Images != null && dto.Images.Count > 0)
                {
                    addImages = await _fileUploadService.UploadMultipleFiles(new[] { "uploads", "images", "rooms", "images" }, dto.Images);
                    newImages.AddRange(addImages);// [url1,url3,url4]
                }
                room.RoomNumber = dto.RoomNumber!;
                room.Status = dto.Status;
                room.Images = newImages;
                room.Thumbnail = thumbnail;
                _context.Update(room);
                await _context.SaveChangesAsync();
                var newRoom = room.ToRoomDTO();
                return Results.Ok(new
                {
                    Message = "Update room successfully",
                    data = newRoom
                });
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(thumbnail))
                {
                    _fileUploadService.DeleteSingleFile(thumbnail);
                }
                if (addImages.Count > 0)
                {
                    _fileUploadService.DeleteMultipleFiles(addImages);
                }
                return Results.BadRequest(new { ex });
            }
        }
        [HttpDelete]
        public async Task<IResult> DeleteRoom([FromQuery] int id)
        {
            try
            {
                var room = await _context.Rooms.FirstOrDefaultAsync(x => x.Id == id);

                if (room == null) return Results.NotFound();
                if (!string.IsNullOrEmpty(room.Thumbnail))
                {
                    _fileUploadService.DeleteSingleFile(room.Thumbnail);
                }
                if (room.Images.Count > 0)
                {
                    _fileUploadService.DeleteMultipleFiles(room.Images);
                }

                _context.Rooms.Remove(room);
                await _context.SaveChangesAsync();
                return Results.Ok(new { Message = "Delete room Successfully" });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { ex });
            }
        }
    }
}