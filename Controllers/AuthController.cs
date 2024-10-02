
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Trainning.Common.Mappers;
using Trainning.Data;
using Trainning.DTO;
using Trainning.Helpers;
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

        public AuthController(ApplicationDbContext context)
        {
            _context = context;
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
        [HttpPost("sign-in")]
        public async Task<IResult> SignIn([FromBody] AuthDTO dto)
        {
            try
            {
                var user = _context.Users.Include(r => r.Bookings).Include(r => r.UserRoles).ThenInclude(r => r.Role).FirstOrDefault(x => x.Email == dto.Email || x.Username == dto.Username);
                if (user == null) return Results.NotFound();
                // Kiểm tra các trường cần thiết
                var check = HashService.VerifyPassword(dto.Password, user.Password);
                if (!check) return Results.BadRequest(new { Message = "Password Invalid" });
                //generate token here
                //access_token
                //refresh_token
                return Results.Ok(new
                {
                    Message = "Login Successfully",
                    Data = user.ToUserDTO()
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