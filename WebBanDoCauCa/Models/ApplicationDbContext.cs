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

            // =========================
            // CART INDEX
            // =========================
            modelBuilder.Entity<CartItem>()
                .HasIndex(c => c.CartId);

            modelBuilder.Entity<CartItem>()
                .HasOne(c => c.Product)
                .WithMany()
                .HasForeignKey(c => c.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            // =========================
            // ORDER DETAIL FIX (QUAN TRỌNG NHẤT)
            // =========================
            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)   // ✔ FIX HERE
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany()
                .HasForeignKey(od => od.ProductId);

            // =========================
            // DECIMAL PRECISION
            // =========================
            modelBuilder.Entity<Order>().Property(o => o.TotalAmount).HasPrecision(18, 2);
            modelBuilder.Entity<OrderDetail>().Property(od => od.Price).HasPrecision(18, 2);
            modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);

            // =========================
            // DATETIME
            // =========================
            modelBuilder.Entity<Product>().Property(p => p.SaleStartDate).HasColumnType("timestamp with time zone");
            modelBuilder.Entity<Product>().Property(p => p.SaleEndDate).HasColumnType("timestamp with time zone");
            modelBuilder.Entity<Review>().Property(r => r.CreatedAt).HasColumnType("timestamp with time zone");
            modelBuilder.Entity<Order>().Property(o => o.OrderDate).HasColumnType("timestamp with time zone");
            modelBuilder.Entity<CartItem>().Property(c => c.DateCreated).HasColumnType("timestamp with time zone");

            // =========================
            // IDENTITY BOOLEAN FIX
            // =========================
            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.Property(u => u.EmailConfirmed).HasColumnType("boolean");
                entity.Property(u => u.PhoneNumberConfirmed).HasColumnType("boolean");
                entity.Property(u => u.TwoFactorEnabled).HasColumnType("boolean");
                entity.Property(u => u.LockoutEnabled).HasColumnType("boolean");
            });

            // =========================
            // STRING OPTIMIZATION
            // =========================
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

            // =========================
            // SEED DATA
            // =========================
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Cần câu" },
                new Category { Id = 2, Name = "Máy câu" },
                new Category { Id = 3, Name = "Dây câu" },
                new Category { Id = 4, Name = "Mồi câu" },
                new Category { Id = 5, Name = "Phụ kiện" }
            );
        }
    }
}