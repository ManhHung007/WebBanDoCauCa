using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebBanDoCauCa.Models;

namespace WebBanDoCauCa.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>, IDataProtectionKeyContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<CartItem> CartItems { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;
        public DbSet<DataProtectionKey> DataProtectionKeys { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. TỐI ƯU HÓA INDEX VÀ QUAN HỆ (Cart & Order)
            modelBuilder.Entity<CartItem>().HasIndex(c => c.CartId); // Index để giỏ hàng không bị chậm

            modelBuilder.Entity<CartItem>()
                .HasOne(c => c.Product)
                .WithMany()
                .HasForeignKey(c => c.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany()
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // 2. DECIMAL PRECISION (Tránh lỗi làm tròn tiền)
            modelBuilder.Entity<Order>().Property(o => o.TotalAmount).HasPrecision(18, 2);
            modelBuilder.Entity<OrderDetail>().Property(od => od.Price).HasPrecision(18, 2);
            modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);

            // 3. DATETIME UTC (Tương thích Postgres)
            modelBuilder.Entity<Product>().Property(p => p.SaleStartDate).HasColumnType("timestamp with time zone");
            modelBuilder.Entity<Product>().Property(p => p.SaleEndDate).HasColumnType("timestamp with time zone");
            modelBuilder.Entity<Review>().Property(r => r.CreatedAt).HasColumnType("timestamp with time zone");
            modelBuilder.Entity<Order>().Property(o => o.OrderDate).HasColumnType("timestamp with time zone");
            modelBuilder.Entity<CartItem>().Property(c => c.DateCreated).HasColumnType("timestamp with time zone");

            // 4. IDENTITY BOOLEAN FIX (Postgres)
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.EmailConfirmed).HasColumnType("boolean");
                entity.Property(u => u.PhoneNumberConfirmed).HasColumnType("boolean");
                entity.Property(u => u.TwoFactorEnabled).HasColumnType("boolean");
                entity.Property(u => u.LockoutEnabled).HasColumnType("boolean");
            });

            // 5. STRING → TEXT (Tối ưu Postgres)
            if (Database.IsNpgsql())
            {
                foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                {
                    foreach (var property in entityType.GetProperties())
                    {
                        if (property.ClrType == typeof(string))
                            property.SetColumnType("text");
                    }
                }
            }

            // 6. SEED DATA (Category, Product, Admin)
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Cần câu" },
                new Category { Id = 2, Name = "Máy câu" },
                new Category { Id = 3, Name = "Dây câu" },
                new Category { Id = 4, Name = "Mồi câu" },
                new Category { Id = 5, Name = "Phụ kiện" }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Cần câu Shimano", Price = 1500000, CategoryId = 1, Brand = "Shimano", ImageUrl = "", IsHot = false, IsOnSale = false, DiscountPercent = 0 },
                new Product { Id = 2, Name = "Máy câu Daiwa", Price = 2000000, CategoryId = 2, Brand = "Daiwa", ImageUrl = "", IsHot = false, IsOnSale = false, DiscountPercent = 0 }
            );

            string adminRoleId = "8d04dce2-969a-435d-bba4-df3f325983dc";
            string adminUserId = "b7237254-8c44-486a-85b4-7b4455589025";

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = adminRoleId, Name = "Admin", NormalizedName = "ADMIN" }
            );

            var adminUser = new ApplicationUser
            {
                Id = adminUserId,
                UserName = "admin@fishingpro.com",
                NormalizedUserName = "ADMIN@FISHINGPRO.COM",
                Email = "admin@fishingpro.com",
                NormalizedEmail = "ADMIN@FISHINGPRO.COM",
                EmailConfirmed = true,
                FullName = "Administrator",
                Address = "Hanoi, Vietnam",
                PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null!, "Admin@123"),
                SecurityStamp = Guid.NewGuid().ToString()
            };

            modelBuilder.Entity<ApplicationUser>().HasData(adminUser);

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { RoleId = adminRoleId, UserId = adminUserId }
            );
        }
    }
}