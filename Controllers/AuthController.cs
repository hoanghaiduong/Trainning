
using System.Data;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Trainning.Common.Mappers;
using Trainning.Data;
using Trainning.DTO;
using Trainning.Helpers;
using Trainning.Models;
using Trainning.Services;

namespace Trainning.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class AuthController : ControllerBase
    {
        // private readonly SQLHelperNoContext _sql;
        private readonly ApplicationDbContext _context;
        private readonly JwtService _jwtService;

        public AuthController(ApplicationDbContext context, JwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // public AuthController(SQLHelperNoContext sql)
        // {
        //     _sql = sql;
        // }

        // [HttpPost("sign-up")]
        // public async Task<IActionResult> SignUp([FromBody] AuthDTO dto)
        // {
        //     try
        //     {
        //         // Kiểm tra các trường cần thiết
        //         if (string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Password) || string.IsNullOrEmpty(dto.Email))
        //         {
        //             return BadRequest("Username, Password, and Email are required.");
        //         }
        //         // Chuẩn bị các tham số cho stored procedure
        //         var parameters = new[]
        //         {
        //             new SqlParameter("@Username", dto.Username),
        //             new SqlParameter("@Password", dto.Password), // Mật khẩu ở đây là plaintext, sẽ được mã hóa trong stored procedure
        //             new SqlParameter("@Email", dto.Email),
        //         };
        //         var result = await _sql.ExecuteQueryAsync("sp_CreateUser", CommandType.StoredProcedure, parameters);
        //         return Ok(new { result });
        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest(new
        //         {
        //             Error = ex.Message
        //         });
        //     }
        // }

        // [HttpPost("sign-in")]
        // public async Task<IActionResult> SignIn([FromBody] AuthDTO dto)
        // {
        //     try
        //     {
        //         // Kiểm tra các trường cần thiết
        //         if (string.IsNullOrEmpty(dto.Password) || string.IsNullOrEmpty(dto.Email))
        //         {
        //             return BadRequest(new { error = "Email and Password are required." });
        //         }
        //         // Chuẩn bị các tham số cho stored procedure
        //         var parameters = new[]
        //         {
        //             new SqlParameter("@Username", dto.Username),
        //             new SqlParameter("@Password", dto.Password), // Mật khẩu ở đây là plaintext, sẽ được mã hóa trong stored procedure
        //             new SqlParameter("@Email", dto.Email),
        //             new SqlParameter("@UserID", SqlDbType.UniqueIdentifier) { Direction = ParameterDirection.Output }, // Output parameter
        //         };
        //         var result = await _sql.ExecuteQueryAsync("sp_Check_Login", CommandType.StoredProcedure, parameters);
        //         var userId = parameters.FirstOrDefault(p => p.ParameterName == "@UserID")?.Value;
        //         return Ok(new { result, userId });

        //     }
        //     catch (Exception ex)
        //     {
        //         return BadRequest(new
        //         {
        //             Error = ex.Message
        //         });
        //     }
        // }
        // [HttpPost("test")]
        // [Authorize]
        // public async Task<IResult> TestValidateAsync()
        // {
        //     try
        //     {

        //         var username = User.FindFirstValue(ClaimTypes.Name);
        //         var email = User.FindFirstValue(ClaimTypes.Email);
        //         var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //         var roles = User.FindAll(ClaimTypes.Role).Select(roleClaim => roleClaim.Value).ToList();
        //         // var roles=User.FindAll(s=>s.Type==ClaimTypes.Role).ToList();
        //         return Results.Ok(new { username, email, id, roles });
        //     }
        //     catch (System.Exception ex)
        //     {

        //         return Results.BadRequest(ex.Message);
        //     }
        // }
        [HttpPost("refresh-token"), Authorize]
        public async Task<IResult> RefreshToken([FromBody] RefreshTokenModel dto)
        {
            // Fetch the user ID from the claims
            var uid = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Find the user in the database
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == int.Parse(uid!));
            if (user == null)
            {
                return Results.NotFound(new { message = "User not found" });
            }

            // Check if the provided refresh token matches the one in the database
            if (dto.RefreshToken != user.RefreshToken)
            {
                return Results.BadRequest(new { message = "Refresh Token Not Valid" });
            }

            // Validate the refresh token
            var principal = _jwtService.ValidateRefreshToken(dto.RefreshToken);
            if (principal == null)
            {
                return Results.Unauthorized();
            }

            // Generate new access token
            var newAccessToken = _jwtService.GenerateAccessToken(principal.Claims);

            // Optional: Generate new refresh token (to invalidate the old one)
            var newRefreshToken = _jwtService.GenerateRefreshToken();

            // Store the new refresh token in the database (optional)
            user.RefreshToken = newRefreshToken;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Results.Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }

        [HttpPost("sign-in")]
        public async Task<IResult> SignIn([FromBody] AuthDTO dto)
        {
            try
            {
                var user = _context.Users.Include(r => r.Bookings).Include(r => r.UserRoles).ThenInclude(r => r.Role).FirstOrDefault(x => x.Email == dto.Email || x.Username == dto.Username);
                if (user == null) return Results.NotFound(new
                {
                    message = "Email Or Username Invalid",
                });
                // Kiểm tra các trường cần thiết
                var check = HashService.VerifyPassword(dto.Password, user.Password);
                if (!check) return Results.BadRequest(new { Message = "Password Invalid" });
                //generate token here
                //access_token
                // Generate claims
                var claims = new List<Claim>
                {
                    new (ClaimTypes.NameIdentifier, user.Id.ToString()),   // User ID
                    new (ClaimTypes.Name, user.Username),                 // Username
                    new (ClaimTypes.Email, user.Email),                   // Email
                    new (ClaimTypes.Role, string.Join(",", user.UserRoles.Select(ur => ur.Role.Name))) // Roles (if multiple)
                };
                var access_token = _jwtService.GenerateAccessToken(claims);
                // Generate refresh token (optional)
                var refreshToken = _jwtService.GenerateRefreshToken();
                user.RefreshToken = refreshToken;
                await _context.SaveChangesAsync();
                //refresh_token
                return Results.Ok(new
                {
                    Message = "Login Successfully",
                    Data = user.ToUserDTO(),
                    access_token,
                    refreshToken
                });

            }
            catch (Exception ex)
            {
                return Results.BadRequest(new
                {
                    Error = ex.Message
                });
            }
        }
    }


}