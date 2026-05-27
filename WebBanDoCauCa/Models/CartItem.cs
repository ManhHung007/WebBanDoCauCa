using System.ComponentModel.DataAnnotations;

namespace WebBanDoCauCa.Models
{
    public class CartItem
    {
        [Key]
        public int Id { get; set; }

        // Liên kết với sản phẩm trong cửa hàng
        public int ProductId { get; set; }
        public virtual Product? Product { get; set; }

        [Display(Name = "Số lượng")]
        [Range(1, 100, ErrorMessage = "Số lượng phải từ 1 đến 100")]
        public int Quantity { get; set; }

        // Lưu trữ ID phiên làm việc của người dùng (Session) 
        // để phân biệt giỏ hàng của khách này với khách khác
        public string? CartId { get; set; }

        [Display(Name = "Ngày thêm")]
        public DateTime DateCreated { get; set; }
    }
}