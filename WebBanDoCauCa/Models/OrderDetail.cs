namespace WebBanDoCauCa.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }

        // FK Order
        public int OrderId { get; set; }
        public Order Order { get; set; }

        // FK Product
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}