using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanDoCauCa.Models;

namespace WebBanDoCauCa.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(
            ILogger<HomeController> logger,
            ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // =========================
        // TRANG CHỦ
        // =========================
        public async Task<IActionResult> Index()
        {
            // =========================
            // SẢN PHẨM MỚI NHẤT
            // =========================
            var featuredProducts = await _context.Products
                .Include(p => p.Category)
                .OrderByDescending(p => p.Id)
                .Take(6)
                .ToListAsync();

            // =========================
            // HOT PRODUCTS (BEST SELLER)
            // theo tổng số lượng đã bán
            // =========================
            var hotProductsQuery = await _context.Products
    .Include(p => p.Category)
    .Select(p => new
    {
        Product = p,
        Sold = p.OrderDetails.Sum(x => (int?)x.Quantity) ?? 0
    })
    .OrderByDescending(x => x.Sold)
    .ThenByDescending(x => x.Product.Id)
    .Take(6)
    .ToListAsync();

            ViewBag.HotProducts = hotProductsQuery
                .Select(x => x.Product)
                .ToList();
            return View(featuredProducts);

        }

        // =========================
        // PRIVACY
        // =========================
        public IActionResult Privacy()
        {
            return View();
        }

        // =========================
        // ERROR
        // =========================
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}