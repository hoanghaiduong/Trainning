using Microsoft.EntityFrameworkCore;
using Trainning.Entities;

namespace Trainning.Data
{
    public class ApplicationDbContext : DbContext
    {
        
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Book> Books { get; set; }
    }
}