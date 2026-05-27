using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Thêm dòng này

namespace WebBanDoCauCa.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;

        // Lưu ID tài khoản người dùng
        public string? UserId { get; set; }

        // THÊM: Liên kết Navigation Property để truy xuất User dễ dàng
        [ForeignKey("UserId")]
        public ApplicationUser? User { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên")]
        public string CustomerName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng nhập địa chỉ")]
        public string Address { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Chờ xử lý";

        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}