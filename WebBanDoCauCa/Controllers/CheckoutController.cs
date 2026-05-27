using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanDoCauCa.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace WebBanDoCauCa.Controllers
{
    [Authorize] // Bắt buộc đăng nhập để sử dụng các chức năng trong Controller này
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CheckoutController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Checkout/Index
        // Hiển thị form nhập thông tin giao hàng và tóm tắt đơn hàng
        public async Task<IActionResult> Index()
        {
            // 1. Lấy danh sách sản phẩm trong giỏ hàng
            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .ToListAsync();

            // 2. Nếu giỏ hàng trống, quay về trang sản phẩm
            if (cartItems == null || !cartItems.Any())
            {
                return RedirectToAction("Index", "Products");
            }

            // 3. Tính tổng tiền để hiển thị lên View
ViewBag.Total = cartItems.Sum(item => item.Quantity * (item.Product?.Price ?? 0));
            ViewBag.CartItems = cartItems;

            return View();
        }

        // POST: Checkout/Process
        // Xử lý lưu đơn hàng vào Database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Process([Bind("CustomerName,Phone,Address")] Order order)
        {
            // 1. Lấy dữ liệu giỏ hàng để xử lý thanh toán
            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .ToListAsync();

            if (ModelState.IsValid)
            {
                // 2. Thiết lập thông tin tự động cho đơn hàng
                order.OrderDate = DateTime.Now;
                order.Status = "Pending"; // Chờ xử lý
                order.TotalAmount = cartItems.Sum(item => item.Quantity * (item.Product?.Price ?? 0));

                // Lấy ID của người dùng đang đăng nhập để gán vào đơn hàng
                order.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // 3. Lưu đơn hàng
                _context.Add(order);

                // 4. Xóa sạch giỏ hàng của người dùng sau khi đặt hàng thành công
                _context.CartItems.RemoveRange(cartItems);

                await _context.SaveChangesAsync();

                // 5. Chuyển đến trang thông báo thành công
                return RedirectToAction(nameof(Success));
            }

            // Nếu dữ liệu nhập vào lỗi (ví dụ thiếu số điện thoại), load lại trang Index
            ViewBag.Total = cartItems.Sum(item => item.Quantity * item.Product?.Price ?? 0);
            ViewBag.CartItems = cartItems;
            return View("Index", order);
        }

        // GET: Checkout/Success
        // Hiển thị trang thông báo đặt hàng thành công
        public IActionResult Success()
        {
            return View();
        }

        // Quản lý đơn hàng dành riêng cho Admin (Xem danh sách đơn hàng đã đặt)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageOrders()
        {
            return View(await _context.Orders.OrderByDescending(o => o.OrderDate).ToListAsync());
        }
    }
}