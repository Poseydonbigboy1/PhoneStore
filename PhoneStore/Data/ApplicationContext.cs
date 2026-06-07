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
        public DbSet<Brand> Brands { get; set; } = null!;
        public DbSet<ComponentCategory> ComponentCategories { get; set; } = null!;
        public DbSet<Cart> Carts { get; set; } = null!;
        public DbSet<Delivery> Deliveries { get; set; } = null!;
        public DbSet<UserAddress> UserAddresses { get; set; } = null!;
        public DbSet<Wishlist> Wishlists { get; set; } = null!;
        public DbSet<Banner> Banners { get; set; } = null!;

        public ApplicationContext(DbContextOptions<ApplicationContext> options)
        : base(options)
        {
            Database.EnsureCreated();
            EnsureCustomTables();
        }

        /// <summary>
        /// Создаёт таблицы, добавленные после первого EnsureCreated (когда БД уже существует).
        /// EnsureCreated не обновляет схему существующей БД — эти DDL-команды восполняют пробел.
        /// </summary>
        private void EnsureCustomTables()
        {
            Database.ExecuteSqlRaw("""
                CREATE TABLE IF NOT EXISTS "Banners" (
                    "Id"        uuid         NOT NULL DEFAULT gen_random_uuid() PRIMARY KEY,
                    "ImageUrl"  text         NOT NULL DEFAULT '',
                    "Link"      text         NOT NULL DEFAULT '',
                    "SortOrder" integer      NOT NULL DEFAULT 0,
                    "IsActive"  boolean      NOT NULL DEFAULT true
                );
                """);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var users = SeedData.CreateUsers();
            var brands = SeedData.CreateBrands();
            var categories = SeedComponentCategories.CreateComponentCategories();
            var products = SeedData.CreateProducts(brands);
            var components = SeedData.CreateComponents(categories);
            var skus = SeedData.CreateSkus(products);
            var productComponents = SeedData.CreateProductComponents(products, skus, components);
            var (orders, orderItems) = SeedData.CreateOrders(users, skus);

            modelBuilder.Entity<User>().HasData(users);
            modelBuilder.Entity<Brand>().HasData(brands);
            modelBuilder.Entity<ComponentCategory>().HasData(categories);
            modelBuilder.Entity<Product>().HasData(products);
            modelBuilder.Entity<Component>().HasData(components);
            modelBuilder.Entity<Sku>().HasData(skus);
            modelBuilder.Entity<ProductComponent>().HasData(productComponents);
            modelBuilder.Entity<Order>().HasData(orders);
            modelBuilder.Entity<OrderItem>().HasData(orderItems);

            // Настройка связей
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Brand)
                .WithMany(b => b.Products)
                .HasForeignKey(p => p.BrandId);

            modelBuilder.Entity<Component>()
                .HasOne(c => c.ComponentCategory)
                .WithMany(cc => cc.Components)
                .HasForeignKey(c => c.ComponentCategoryId);

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

            modelBuilder.Entity<ProductComponent>()
                .HasOne(pc => pc.Sku)
                .WithMany(s => s.ProductComponents)
                .HasForeignKey(pc => pc.SkuId);

            // Cart
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId);

            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Sku)
                .WithMany()
                .HasForeignKey(c => c.SkuId);

            modelBuilder.Entity<Cart>()
                .HasIndex(c => new { c.UserId, c.SkuId })
                .IsUnique();

            // Delivery (1:1 Order)
            modelBuilder.Entity<Delivery>()
                .HasOne(d => d.Order)
                .WithOne(o => o.Delivery)
                .HasForeignKey<Delivery>(d => d.OrderId);

            // UserAddress
            modelBuilder.Entity<UserAddress>()
                .HasOne(a => a.User)
                .WithMany()
                .HasForeignKey(a => a.UserId);

            // Wishlist
            modelBuilder.Entity<Wishlist>()
                .HasOne(w => w.User)
                .WithMany()
                .HasForeignKey(w => w.UserId);

            modelBuilder.Entity<Wishlist>()
                .HasOne(w => w.Sku)
                .WithMany()
                .HasForeignKey(w => w.SkuId);

            modelBuilder.Entity<Wishlist>()
                .HasIndex(w => new { w.UserId, w.SkuId })
                .IsUnique();

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
