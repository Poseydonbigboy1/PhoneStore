using Microsoft.EntityFrameworkCore;
using PhoneStore.Data.Seeds;

namespace PhoneStore.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;

        public ApplicationContext() 
        { 
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) 
        { 
            modelBuilder.Entity<User>().HasData(SeedData.CreateUsers());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=phone_store;Username=postgres;Password=1234");

            
        }
    }
}
