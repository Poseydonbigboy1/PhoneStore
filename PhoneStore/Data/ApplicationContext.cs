using Microsoft.EntityFrameworkCore;
using System.Text.Json;
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
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var users = SeedData.CreateUsers();
            var components = SeedData.CreateComponents();
            var products = SeedData.CreateProducts();
            var skus = SeedData.CreateSkus(products);
            var productComponents = SeedData.CreateProductComponents(products, skus, components);
            var orders = SeedData.CreateOrders(users, skus);

            modelBuilder.Entity<User>().HasData(users);
            modelBuilder.Entity<Component>().HasData(components);
            modelBuilder.Entity<Product>().HasData(products);
            modelBuilder.Entity<Sku>().HasData(skus);
            modelBuilder.Entity<ProductComponent>().HasData(productComponents);

            var orderItems = orders.SelectMany(o => o.OrderItems).ToList();
            orders.ForEach(o => o.OrderItems = new List<OrderItem>());
            
            modelBuilder.Entity<Order>().HasData(orders);
            modelBuilder.Entity<OrderItem>().HasData(orderItems);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.User)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.UserId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderId);

            modelBuilder.Entity<OrderItem>()
                .HasOne(oi => oi.Sku)
                .WithMany() 
                .HasForeignKey(oi => oi.SkuId);

            modelBuilder.Entity<Component>()
                .Property(e => e.DataType)
                .HasConversion<string>();

            modelBuilder.Entity<ProductComponent>()
                .Property(e => e.ValueJson)
                .HasColumnType("jsonb");
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=phone_store;Username=postgres;Password=1234");


        }
    }
}
