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

        // =========================
        // Lấy CartId từ Session
        // =========================
        private string GetCartId()
        {
            var cartId = HttpContext.Session.GetString("CartId");

            if (string.IsNullOrEmpty(cartId))
            {
                cartId = Guid.NewGuid().ToString();
                HttpContext.Session.SetString("CartId", cartId);
            }

            return cartId;
        }

        // =========================
        // 1. XEM GIỎ HÀNG
        // =========================
        public async Task<IActionResult> Index()
        {
            var cartId = GetCartId();

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.CartId == cartId)
                .ToListAsync();

            return View(cartItems);
        }

        // =========================
        // 2. THÊM VÀO GIỎ
        // =========================
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            if (quantity <= 0)
            {
                return Json(new { success = false, message = "Số lượng không hợp lệ." });
            }

            var cartId = GetCartId();

            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                return Json(new { success = false, message = "Sản phẩm không tồn tại." });
            }

            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.ProductId == productId && c.CartId == cartId);

            if (cartItem == null)
            {
                cartItem = new CartItem
                {
                    ProductId = productId,
                    Quantity = quantity,
                    CartId = cartId,
                    DateCreated = DateTime.Now
                };

                _context.CartItems.Add(cartItem);
            }
            else
            {
                cartItem.Quantity += quantity;
            }

            await _context.SaveChangesAsync();

            var totalCartCount = await _context.CartItems
                .Where(c => c.CartId == cartId)
                .SumAsync(c => c.Quantity);

            return Json(new { success = true, newCount = totalCartCount });
        }

        // =========================
        // 3. CHECKOUT
        // =========================
        [HttpPost]
        public async Task<IActionResult> Checkout(string customerName, string phone, string address)
        {
            var cartId = GetCartId();

            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.CartId == cartId)
                .ToListAsync();

            if (cartItems == null || !cartItems.Any())
            {
                return Json(new { success = false, message = "Giỏ hàng trống" });
            }

            // đảm bảo Product không null
            var validItems = cartItems
                .Where(x => x.Product != null)
                .ToList();

            if (!validItems.Any())
            {
                return Json(new { success = false, message = "Dữ liệu sản phẩm không hợp lệ" });
            }

            // =====================
            // 1. Tạo Order
            // =====================
            var order = new Order
            {
                CustomerName = customerName,
                Phone = phone,
                Address = address,
                Status = "Pending",
                TotalAmount = validItems.Sum(x => x.Product!.Price * x.Quantity)
            };

            _context.Orders.Add(order);
            await _context.SaveChangesAsync(); // phải save để có Order.Id

            // =====================
            // 2. Tạo OrderDetails
            // =====================
            var orderDetails = validItems.Select(item => new OrderDetail
            {
                OrderId = order.Id,
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = item.Product!.Price
            }).ToList();

            _context.OrderDetails.AddRange(orderDetails);
            await _context.SaveChangesAsync();

            // =====================
            // 3. Xoá giỏ hàng
            // =====================
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return Json(new { success = true, orderId = order.Id });
        }
    }
}