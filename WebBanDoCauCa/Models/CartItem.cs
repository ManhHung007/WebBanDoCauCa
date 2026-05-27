using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanDoCauCa.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        // Khóa ngoại tới bảng Product
        [Required]
        public int ProductId { get; set; }

        // Điều hướng tới sản phẩm (dùng virtual cho lazy loading nếu có)
        [ForeignKey("ProductId")]
        public virtual Product? Product { get; set; }

        [Display(Name = "Số lượng")]
        [Range(1, 100, ErrorMessage = "Số lượng phải từ 1 đến 100")]
        public int Quantity { get; set; }

        // Lưu CartId để phân biệt giỏ hàng
        // [Index] -> Nếu bạn dùng SQL Server/Npgsql, nên đánh Index cột này để truy vấn nhanh hơn
        [StringLength(100)]
        public string? CartId { get; set; }

        [Display(Name = "Ngày thêm")]
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}