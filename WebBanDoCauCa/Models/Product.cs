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
        [Range(0, double.MaxValue, ErrorMessage = "Giá không được âm")]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        [Required]
        public int CategoryId { get; set; }

        public virtual Category? Category { get; set; }

        public bool IsHot { get; set; } = false;

        public bool IsOnSale { get; set; } = false;

        [Range(0, 100, ErrorMessage = "Phần trăm giảm giá từ 0-100")]
        public int DiscountPercent { get; set; } = 0;

        // PostgreSQL yêu cầu các trường này tương thích với UTC
        public DateTime? SaleStartDate { get; set; }

        public DateTime? SaleEndDate { get; set; }

        public string? Brand { get; set; }

        [NotMapped]
        public decimal SalePrice
        {
            get
            {
                return IsSaleActive
                    ? Price - (Price * DiscountPercent / 100)
                    : Price;
            }
        }

        [NotMapped]
        public bool IsSaleActive
        {
            get
            {
                // Sử dụng UtcNow để so sánh chuẩn với Database
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