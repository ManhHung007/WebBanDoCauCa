using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanDoCauCa.Models;

namespace WebBanDoCauCa.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ==========================================================
        // Cải tiến GetCartId: Đảm bảo Session được lưu ổn định
        // ==========================================================
        private string GetCartId()
        {
            var cartId = HttpContext.Session.GetString("CartId");
            if (string.IsNullOrEmpty(cartId))
            {
                cartId = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("CartId", cartId);
                // Đảm bảo Session được lưu ngay lập tức
                HttpContext.Session.CommitAsync().Wait();
            }
            return cartId;
        }

        public async Task<IActionResult> Index()
        {
            var cartId = GetCartId();
            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.CartId == cartId)
                .ToListAsync();

            return View(cartItems);
        }

        // ==========================================================
        // THÊM VÀO GIỎ (Đã thêm kiểm tra lỗi)
        // ==========================================================
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            try
            {
                if (quantity <= 0) return Json(new { success = false, message = "Số lượng không hợp lệ." });

                var cartId = GetCartId();
                var product = await _context.Products.FindAsync(productId);

                if (product == null) return Json(new { success = false, message = "Sản phẩm không tồn tại." });

                var cartItem = await _context.CartItems
                    .FirstOrDefaultAsync(c => c.ProductId == productId && c.CartId == cartId);

                if (cartItem == null)
                {
                    _context.CartItems.Add(new CartItem
                    {
                        ProductId = productId,
                        Quantity = quantity,
                        CartId = cartId,
                        DateCreated = DateTime.Now
                    });
                }
                else
                {
                    cartItem.Quantity += quantity;
                }

                await _context.SaveChangesAsync();

                var totalCount = await _context.CartItems.Where(c => c.CartId == cartId).SumAsync(c => c.Quantity);
                return Json(new { success = true, newCount = totalCount });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        // ==========================================================
        // THANH TOÁN (Checkout)
        // ==========================================================
        [HttpPost]
        public async Task<IActionResult> Checkout(string customerName, string phone, string address)
        {
            var cartId = GetCartId();
            var cartItems = await _context.CartItems.Include(c => c.Product).Where(c => c.CartId == cartId).ToListAsync();

            if (!cartItems.Any())
                return Json(new { success = false, message = "Giỏ hàng của bạn đang trống." });

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var order = new Order
                {
                    CustomerName = customerName,
                    Phone = phone,
                    Address = address,
                    Status = "Pending",
                    OrderDate = DateTime.UtcNow,
                    TotalAmount = cartItems.Sum(x => (x.Product?.Price ?? 0) * x.Quantity)
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                var details = cartItems.Select(item => new OrderDetail
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Product!.Price
                }).ToList();

                _context.OrderDetails.AddRange(details);
                _context.CartItems.RemoveRange(cartItems);

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return Json(new { success = true, orderId = order.Id });
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return Json(new { success = false, message = "Lỗi đặt hàng: " + ex.Message });
            }
        }
    }
}