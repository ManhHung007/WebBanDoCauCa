using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebBanDoCauCa.Models;

namespace WebBanDoCauCa.Controllers
{
    [Authorize]
    public class OrdersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. ADMIN: Quản lý danh sách đơn hàng
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders.OrderByDescending(o => o.Id).ToListAsync();
            return View(orders);
        }

        // 2. NGƯỜI DÙNG: Xem đơn hàng cá nhân
        public async Task<IActionResult> MyOrders()
        {
            if (User.IsInRole("Admin")) return RedirectToAction(nameof(Index));

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var orders = await _context.Orders
                .Where(o => o.UserId == userId && o.Status != "Đã hủy")
                .OrderByDescending(o => o.Id)
                .ToListAsync();
            return View(orders);
        }

        // 3. ADMIN: Cập nhật trạng thái
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                order.Status = status;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // 4. NGƯỜI DÙNG: Xác nhận đã nhận hàng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmReceived(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await _context.Orders
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId && o.Status == "Đang giao");

            if (order != null)
            {
                order.Status = "Đã giao";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(MyOrders));
        }

        // 5. THỐNG KÊ DOANH THU (Đơn giản)
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Statistics()
        {
            // Trả về trực tiếp danh sách RevenueViewModel
            return View(await GetRevenueData());
        }

        private async Task<List<RevenueViewModel>> GetRevenueData()
        {
            return await _context.Orders
                .Where(o => o.Status == "Đã giao")
                .GroupBy(o => new { o.OrderDate.Year, o.OrderDate.Month })
                .Select(g => new RevenueViewModel
                {
                    Month = g.Key.Month,
                    Year = g.Key.Year,
                    TotalRevenue = g.Sum(o => o.TotalAmount)
                })
                .OrderByDescending(g => g.Year).ThenByDescending(g => g.Month)
                .ToListAsync();
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);
            if (order != null && order.Status == "Pending")
            {
                order.Status = "Đã hủy";
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(MyOrders));
        }
    }

    // Class ViewModel chỉ còn RevenueViewModel
    public class RevenueViewModel
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}