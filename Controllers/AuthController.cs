using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Trainning.Models;
using Trainning.Services;

namespace Trainning.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtServices _jwtServices;
        private readonly List<UserModel> _users = [];
        public AuthController(JwtServices jwtServices)
        {
            _jwtServices = jwtServices;
            _users.Add(new UserModel
            {
                Username = "string",
                Password = "string",
                Roles = new List<string> { "User" },
            });
            _users.Add(new UserModel
            {
                Username = "admin",
                Password = "admin",
                Roles = new List<string> { "Admin", "User" }
            });
        }

        // Endpoint để login và cấp access token + refresh token
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserModel model)
        {
            var user = _users.FirstOrDefault(u => u.Username == model.Username && u.Password == model.Password);
            // Kiểm tra thông tin đăng nhập (ở đây là kiểm tra mẫu, bạn nên thay bằng cách xác thực thực tế)
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, model.Username),

                };
                // Add each role as a separate claim
                foreach (var role in user.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                // Tạo access token và refresh token
                var accessToken = _jwtServices.GenerateAccessToken(claims);
                var refreshToken = _jwtServices.GenerateRefreshToken();
                return Ok(new TokenModel
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken
                });

            }

            return Unauthorized();
        }

        // Endpoint để làm mới access token bằng refresh token
        [HttpPost("refresh-token")]
        public IActionResult RefreshToken([FromBody] string refresh)
        {
            // Xác thực refresh token
            var principal = _jwtServices.ValidateRefreshToken(refresh);
            if (principal == null)
            {
                return Unauthorized("Invalid refresh token.");
            }

            // Tạo mới access token từ các claim của refresh token
            var newAccessToken = _jwtServices.GenerateAccessToken(principal.Claims);
            var newRefreshToken = _jwtServices.GenerateRefreshToken();
            return Ok(new
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            });
        }
        [HttpGet("Account"), Authorize]
        public async Task<IActionResult> GetAccount()
        {
            var username = User.FindFirstValue(ClaimTypes.Name); // User ID

            var user = _users.FirstOrDefault(x => x.Username == username);

            if (user == null)
            {
                return Unauthorized();
            }
            return Ok(new { user });
        }
        [HttpGet("check-user"), Authorize(Roles = "User")]
        public async Task<IActionResult> CheckUserRole()
        {
            var role = User.FindFirstValue(ClaimTypes.Role);

            return Ok(new { Message = "Successfully " + role });
        }
        [HttpGet("check-admin"), Authorize(Roles = "Admin")] // Allow both User and Admin roles
        public async Task<IActionResult> CheckAdminRole()
        {
            var role = User.FindFirstValue(ClaimTypes.Role);
            return Ok(new { Message = "Successfully " + role });
        }
        [HttpGet("check-roles"), Authorize(Roles = "Admin,User")] // Allow both User and Admin roles
        public async Task<IActionResult> CheckRoles()
        {
            var roles = User.FindAll(ClaimTypes.Role).Select(roleClaim => roleClaim.Value).ToList();
            return Ok(new
            {
                Message = "Successfully ",
                roles
            });
        }
    }

}