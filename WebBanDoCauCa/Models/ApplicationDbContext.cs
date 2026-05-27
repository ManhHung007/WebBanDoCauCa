using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Linq;
using WebBanDoCauCa.Models;

namespace WebBanDoCauCa.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<CartItem> CartItems { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =====================================================
            // 1. DECIMAL PRECISION
            // =====================================================
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderDetail>()
                .Property(od => od.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(18, 2);

            // =====================================================
            // 2. IDENTITY BOOLEAN FIX (POSTGRES)
            // =====================================================
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.EmailConfirmed).HasColumnType("boolean");
                entity.Property(u => u.PhoneNumberConfirmed).HasColumnType("boolean");
                entity.Property(u => u.TwoFactorEnabled).HasColumnType("boolean");
                entity.Property(u => u.LockoutEnabled).HasColumnType("boolean");
            });

            // =====================================================
            // 3. STRING → TEXT (POSTGRES OPTIMIZATION)
            // =====================================================
            if (Database.IsNpgsql())
            {
                foreach (var entityType in modelBuilder.Model.GetEntityTypes())
                {
                    foreach (var property in entityType.GetProperties())
                    {
                        if (property.ClrType == typeof(string))
                        {
                            property.SetColumnType("text");
                        }
                    }
                }
            }

            // =====================================================
            // 4. GLOBAL DATETIME → UTC FIX (SAFE VERSION)
            // =====================================================
            var utcConverter = new ValueConverter<DateTime, DateTime>(
                v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
            );

            var nullableUtcConverter = new ValueConverter<DateTime?, DateTime?>(
                v => v.HasValue
                    ? (v.Value.Kind == DateTimeKind.Utc ? v : v.Value.ToUniversalTime())
                    : v,
                v => v.HasValue
                    ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc)
                    : v
            );

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(DateTime))
                    {
                        property.SetValueConverter(utcConverter);
                    }

                    if (property.ClrType == typeof(DateTime?))
                    {
                        property.SetValueConverter(nullableUtcConverter);
                    }
                }
            }

            // =====================================================
            // 5. SEED DATA - CATEGORY
            // =====================================================
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Cần câu" },
                new Category { Id = 2, Name = "Máy câu" }
            );

            // =====================================================
            // 6. SEED DATA - PRODUCT
            // =====================================================
            modelBuilder.Entity<Product>().HasData(
                new Product
                {
                    Id = 1,
                    Name = "Cần câu Shimano",
                    Price = 1500000,
                    CategoryId = 1,
                    Brand = "Shimano",
                    ImageUrl = "",
                    IsHot = false,
                    IsOnSale = false,
                    DiscountPercent = 0
                },
                new Product
                {
                    Id = 2,
                    Name = "Máy câu Daiwa",
                    Price = 2000000,
                    CategoryId = 2,
                    Brand = "Daiwa",
                    ImageUrl = "",
                    IsHot = false,
                    IsOnSale = false,
                    DiscountPercent = 0
                }
            );

            // =====================================================
            // 7. SEED ROLE + ADMIN USER
            // =====================================================
            string adminRoleId = "8d04dce2-969a-435d-bba4-df3f325983dc";
            string adminUserId = "b7237254-8c44-486a-85b4-7b4455589025";

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                }
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
                PasswordHash = new PasswordHasher<ApplicationUser>()
                    .HashPassword(null!, "Admin@123"),
                SecurityStamp = Guid.NewGuid().ToString()
            };

            modelBuilder.Entity<ApplicationUser>().HasData(adminUser);

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = adminRoleId,
                    UserId = adminUserId
                }
            );
        }
    }
}