using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Trainning.Common;
using Trainning.Common.Extensions;
using Trainning.Common.Mappers;
using Trainning.Data;
using Trainning.DTO;
using Trainning.DTO.Updates;
using Trainning.Entities;
using Trainning.Interfaces;
using Trainning.Models;
using Trainning.Services;

namespace Trainning.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileUploadService _fileUploadService;
        public UserController(IFileUploadService fileUploadService, ApplicationDbContext context)
        {

            _fileUploadService = fileUploadService;
            _context = context;
        }
        [HttpGet("all")]
        public async Task<IResult> GetUsers([FromQuery] PaginationModel paginate)
        {
            try
            {
                var users = _context.Users.Include(x => x.UserRoles).ThenInclude(x => x.Role).Include(x => x.Bookings).AsQueryable();

                // Apply search filtering if provided, before projecting to DTO
                if (!string.IsNullOrEmpty(paginate.Search))
                {
                    users = users.Where(x =>
                 x.FirstName.Contains(paginate.Search) ||
                 x.LastName.Contains(paginate.Search) ||
                 x.Email.Contains(paginate.Search) ||
                 x.Phone.Contains(paginate.Search)||
                 x.Username.Contains(paginate.Search)
                 );
                }

                // Project to DTOs after filtering
                var usersDTOQuery = users.Select(x => x.ToUserDTO());


                // Áp dụng phân trang trên IQueryable trước khi gọi ToListAsync
                var usersPaginated = await usersDTOQuery.ToPaginatedResultAsync(paginate);

                return Results.Ok(new { Message = "Get User Success", Data = usersPaginated });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { ex });
            }
        }
        [HttpGet]
        public async Task<IResult> GetUser([FromQuery] int id)
        {
            try
            {
                var user = await _context.Users.Include(x => x.UserRoles).ThenInclude(x => x.Role).Include(x => x.Bookings).FirstOrDefaultAsync(x => x.Id == id);
                if (user == null) return Results.NotFound();
                return Results.Ok(new { Message = "Get User Success", Data = user.ToUserDTO() });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { ex });
            }
        }
        [HttpPost]
        public async Task<IResult> CreateUser([FromForm] CreateUserDTO dto)
        {
            var avatar = string.Empty;
            Hotel hotel = null!;
            try
            {
                if (dto.HotelId != null)
                {
                    var hotelFind = await _context.Hotels.FirstOrDefaultAsync(x => x.Id == dto.HotelId);
                    if (hotelFind == null) return Results.NotFound(new { Message = "Hotel not found" });
                    hotel = hotelFind;
                }
                if (dto.Avatar != null)
                {
                    avatar = await _fileUploadService.UploadSingleFile(new[] { "uploads", "images", "users", "avatars" }, dto.Avatar);
                }
                var hashPassword = HashService.EnhancedHashPassword(dto.Password);
                var newUser = new User
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Username = dto.Username,
                    Password = hashPassword,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    Avatar = avatar,
                    Hotel = hotel
                };
                // Save the new user to the database to generate the User ID
                await _context.Users.AddAsync(newUser);

                await _context.SaveChangesAsync();
                if (dto.RoleIDs != null && dto.RoleIDs.Count > 0)
                {
                    var roles = await _context.Roles.Where(x => dto.RoleIDs.Contains(x.Id)).ToListAsync();
                    // Find missing RoleIDs
                    var missingRoleIds = dto.RoleIDs.Except(roles.Select(r => r.Id)).ToList();

                    // If there are missing RoleIDs, throw an exception
                    if (missingRoleIds.Any())
                    {
                        return Results.NotFound(new { Message = $"The following RoleIDs were not found: {string.Join(", ", missingRoleIds)}" });
                    }

                    if (roles.Count > 0)
                    {
                        var userRoles = new List<UserRoles>();

                        foreach (var role in roles)
                        {

                            userRoles.Add(new UserRoles
                            {
                                User = newUser,
                                Role = role,
                                RoleId = role.Id,
                                UserId = newUser.Id
                            });
                        }
                        await _context.UserRoles.AddRangeAsync(userRoles);
                        await _context.SaveChangesAsync();
                    }
                    // Add the roles to the user

                }

                // Return success with user data
                return Results.Ok(new
                {
                    Message = "Create new User successfully",
                    Data = newUser.ToUserDTO()
                });

            }
            catch (Exception ex)
            {

                if (!string.IsNullOrEmpty(avatar))
                {
                    _fileUploadService.DeleteSingleFile(avatar);
                }
                return Results.BadRequest(new { ex.Message, detail = ex.InnerException.Message });
            }


        }
        [HttpPut]
        public async Task<IResult> UpdateUser([FromQuery] int id, [FromForm] UpdateUserDTO dto)
        {
            var newAvatar = string.Empty;

            try
            {

                var user = await _context.Users.Include(r => r.Hotel).Include(r => r.Bookings).Include(r => r.UserRoles).ThenInclude(r => r.Role).FirstOrDefaultAsync(u => u.Id == id);
                if (user == null) return Results.NotFound();

                if (dto.HotelId != null)
                {
                    var hotel = await _context.Hotels.FirstOrDefaultAsync(x => x.Id == dto.HotelId);
                    if (hotel == null) return Results.NotFound(new { Message = "Hotel not found" });
                    user.Hotel = hotel;
                }
                if (!string.IsNullOrEmpty(user.Avatar) && dto.Avatar != null)
                {
                    //có ảnh cũ và có nhập ảnh mới thì xoá ảnh cũ di
                    _fileUploadService.DeleteSingleFile(user.Avatar);
                    //cập nhật ẩnh mới
                    newAvatar = await _fileUploadService.UploadSingleFile(["uploads", "images", "users", "avatars"], dto.Avatar);
                    if (!string.IsNullOrEmpty(newAvatar))
                    {
                        user.Avatar = newAvatar;
                    }
                }
                user.FirstName = dto.FirstName;
                user.LastName = dto.LastName;
                user.Username = dto.Username;
                user.Phone = dto.Phone;

                if (dto.RoleIDs != null && dto.RoleIDs.Count > 0)
                {
                    var roles = await _context.Roles.Where(x => dto.RoleIDs.Contains(x.Id)).ToListAsync();
                    // Find missing RoleIDs
                    var missingRoleIds = dto.RoleIDs.Except(roles.Select(r => r.Id)).ToList();
                    if (missingRoleIds.Count != 0)
                    {
                        return Results.NotFound(new { Message = $"The following RoleIDs were not found: {string.Join(", ", missingRoleIds)}" });
                    }
                    if (roles != null && roles.Count > 0)
                    {
                        var userRoles = new List<UserRoles>();

                        foreach (var role in roles)
                        {

                            userRoles.Add(new UserRoles
                            {
                                User = user,
                                Role = role,
                            });
                        }
                        _context.UserRoles.UpdateRange(userRoles);
                        await _context.SaveChangesAsync();
                    }
                }
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return Results.Ok(new
                {
                    Message = "Update user successfully",
                    user = user.ToUserDTO()
                });

            }
            catch (Exception ex)
            {

                if (!string.IsNullOrEmpty(newAvatar))
                {
                    _fileUploadService.DeleteSingleFile(newAvatar);
                }
                return Results.BadRequest(new { ex.Message, detail = ex.InnerException.Message });
            }

        }
        [HttpDelete]
        public async Task<IResult> DeleteUser([FromQuery] int id)
        {
            try
            {
                var user = await _context.Users.Include(x => x.UserRoles).Include(x => x.Bookings).FirstOrDefaultAsync(x => x.Id == id);
                if (user == null) return Results.NotFound();
                if (!string.IsNullOrEmpty(user.Avatar))
                {
                    _fileUploadService.DeleteSingleFile(user.Avatar!);
                }
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return Results.Ok(new
                {
                    Message = "Delete user successfully"
                });
            }
            catch (Exception ex)
            {
                return Results.BadRequest(new { ex.Message, detail = ex.InnerException.Message });
            }
        }
    }
}