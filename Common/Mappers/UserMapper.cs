using Trainning.DTO;
using Trainning.Entities;

namespace Trainning.Common.Mappers
{
    public static class UserMapper
    {
        public static UserDTO ToUserDTO(this User user)
        {
            var roles=user.UserRoles.Select(x=>x.Role).ToList();
            return new UserDTO
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Username = user.Username,
                Password = user.Password,
                Email = user.Email,
                EmailVerified = user.EmailVerified,
                Phone = user.Phone,
                Avatar = user.Avatar,
                RefreshToken = user.RefreshToken,
                LastLogin = user.LastLogin,
                Roles=roles,
                Hotel=user.Hotel,
                Bookings = user.Bookings
            };
        }
    }
}