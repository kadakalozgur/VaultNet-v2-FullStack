using Microsoft.EntityFrameworkCore;
using VaultNetAPIv2.Models;

namespace VaultNetAPIv2.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }

    }
}
