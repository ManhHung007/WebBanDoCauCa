using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanDoCauCa.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        public int CategoryId { get; set; }

        public virtual Category? Category { get; set; }

        // =========================
        // SẢN PHẨM HOT
        // =========================

        public bool IsHot { get; set; } = false;

        // =========================
        // KHUYẾN MÃI
        // =========================

        public bool IsOnSale { get; set; } = false;

        public int DiscountPercent { get; set; } = 0;

        public DateTime? SaleStartDate { get; set; }

        public DateTime? SaleEndDate { get; set; }

        // =========================
        // GIÁ SAU GIẢM
        // =========================

        [NotMapped]
        public decimal SalePrice
        {
            get
            {
                if (IsSaleActive)
                {
                    return Price - (Price * DiscountPercent / 100);
                }

                return Price;
            }
        }

        // =========================
        // KIỂM TRA SALE CÒN HIỆU LỰC
        // =========================

        public string? Brand { get; set; }

        [NotMapped]
        public bool IsSaleActive
        {
            get
            {
                return IsOnSale
                    && DiscountPercent > 0
                    && SaleStartDate <= DateTime.Now
                    && SaleEndDate >= DateTime.Now;
            }
        }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}