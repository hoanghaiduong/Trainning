using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Trainning.Data.Configurations;
using Trainning.Entities;


namespace Trainning.Data
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
           
             builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly()); // Apply
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RoomType> RoomTypes { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<UserRoles> UserRoles { get; set; }
    }
}