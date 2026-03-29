using Microsoft.EntityFrameworkCore;
using PhoneStore.Data.Seeds;

namespace PhoneStore.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Component> Components { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Sku> Skus { get; set; } = null!;
        public DbSet<ProductComponent> ProductComponents { get; set; } = null!;

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(SeedData.CreateUsers());

            modelBuilder.Entity<Component>()
                .Property(e => e.DataType)
                .HasConversion<string>();

            modelBuilder.Entity<ProductComponent>()
                .Property(e => e.Value)
                .HasColumnType("jsonb");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=phone_store;Username=postgres;Password=1234");


        }
    }
}
