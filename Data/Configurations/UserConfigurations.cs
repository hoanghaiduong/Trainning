
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Trainning.Entities;

namespace Trainning.Data.Configurations
{
    public class UserConfigurations : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // Chỉ mục unique cho cột Email
            builder.HasIndex(x => x.Email).IsUnique();

            // Chỉ mục unique cho cột Username
            builder.HasIndex(x => x.Username).IsUnique();

            // Chỉ mục unique cho cột Phone
            builder.HasIndex(x => x.Phone).IsUnique();
        }
    }
}