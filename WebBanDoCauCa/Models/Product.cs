using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanDoCauCa.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required(ErrorMessage = "Giá sản phẩm là bắt buộc")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public virtual Category? Category { get; set; }

        public bool IsHot { get; set; } = false;

        public bool IsOnSale { get; set; } = false;

        [Range(0, 100, ErrorMessage = "Phần trăm giảm giá từ 0-100")]
        public int DiscountPercent { get; set; } = 0;

        // FIX: Chỉ định rõ kiểu timestamp cho PostgreSQL/Neon
        [Column(TypeName = "timestamp with time zone")]
        public DateTime? SaleStartDate { get; set; }

        [Column(TypeName = "timestamp with time zone")]
        public DateTime? SaleEndDate { get; set; }

        public string? Brand { get; set; }

        [NotMapped]
        public decimal SalePrice => IsSaleActive
            ? Price - (Price * DiscountPercent / 100)
            : Price;

        [NotMapped]
        public bool IsSaleActive
        {
            get
            {
                var now = DateTime.UtcNow;
                return IsOnSale
                    && DiscountPercent > 0
                    && SaleStartDate.HasValue
                    && SaleEndDate.HasValue
                    && SaleStartDate.Value.ToUniversalTime() <= now
                    && SaleEndDate.Value.ToUniversalTime() >= now;
            }
        }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}