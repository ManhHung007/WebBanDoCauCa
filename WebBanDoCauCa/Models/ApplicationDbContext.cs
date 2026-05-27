using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WebBanDoCauCa.Models
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<CartItem> CartItems { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderDetail> OrderDetails { get; set; } = null!;
        public DbSet<Review> Reviews { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Cấu hình độ chính xác cho kiểu Decimal (Fix cảnh báo)
            modelBuilder.Entity<Order>().Property(o => o.TotalAmount).HasPrecision(18, 2);
            modelBuilder.Entity<OrderDetail>().Property(od => od.Price).HasPrecision(18, 2);
            modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);

            // 2. Chuyển đổi nvarchar sang text cho PostgreSQL
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

            // 3. Seed dữ liệu Admin
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
                PasswordHash = new PasswordHasher<ApplicationUser>().HashPassword(null!, "Admin@123")
            };

            modelBuilder.Entity<ApplicationUser>().HasData(adminUser);

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string> { RoleId = adminRoleId, UserId = adminUserId }
            );
        }
    }
}