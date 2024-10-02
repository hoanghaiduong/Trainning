
using Trainning.DTO;
using Trainning.Entities;

namespace Trainning.Common.Mappers
{
    public static class UserRolesMapper
    {
        public static UserRolesDTO ToUserRoles(this UserRoles userRoles){
            return new UserRolesDTO{
                UserId = userRoles.UserId,
                RoleId = userRoles.RoleId,
                User=userRoles.User,
                Role=userRoles.Role
            };
        }
    }
}