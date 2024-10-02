
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trainning.Entities;

namespace Trainning.Data.Configurations
{
    public class UserRolesConfigurations : IEntityTypeConfiguration<UserRoles>
    {
        public void Configure(EntityTypeBuilder<UserRoles> builder)
        {   // Define composite primary key
            builder.HasKey(ur => new { ur.UserId, ur.RoleId });

            // Define relationships
            builder.HasOne(ur => ur.User)
                   .WithMany(u => u.UserRoles)
                   .HasForeignKey(ur => ur.UserId).OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(ur => ur.Role)
                   .WithMany(r => r.UserRoles)
                   .HasForeignKey(ur => ur.RoleId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}