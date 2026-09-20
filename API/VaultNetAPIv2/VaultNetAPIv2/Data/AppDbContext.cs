using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using VaultNetAPIv2.Models;

namespace VaultNetAPIv2.Data
{
    public class AppDbContext : DbContext, IDataProtectionKeyContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }

        public DbSet<VaultItem> VaultItems { get; set; }

        public DbSet<Category> Categories { get; set; }

        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; }

    }
}
