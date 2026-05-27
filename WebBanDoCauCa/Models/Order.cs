using System;
using System.Collections.Generic;

namespace WebBanDoCauCa.Models
{
    public class Order
    {
        public int Id { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        // User liên kết
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        // Thông tin khách hàng
        public string CustomerName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "Chờ xử lý";

        // Navigation
        public List<OrderDetail> OrderDetails { get; set; } = new();
    }
}