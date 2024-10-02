

using Trainning.DTO;
using Trainning.Entities;

namespace Trainning.Common.Mappers
{
    public static class RoleMapper
    {
        public static RoleDTO ToRoleDTO(this Role role){
            return new RoleDTO{
                Id=role.Id,
                Name = role.Name,
                Description = role.Description,
                UserRoles = role.UserRoles
            };
        }
    }
}