using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using WebBanDoCauCa.Models;
using System.Collections.Generic;

namespace WebBanDoCauCa.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // =========================
        // INDEX (Với bộ lọc nâng cao)
        // =========================
        public async Task<IActionResult> Index(string categoryIds, decimal? maxPrice, string sortOrder)
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            // 1. Lọc theo nhiều danh mục (categoryIds truyền dạng "1,2,3")
            if (!string.IsNullOrWhiteSpace(categoryIds))
            {
                var idList = categoryIds.Split(',').Select(int.Parse).ToList();
                query = query.Where(p => idList.Contains(p.CategoryId));
            }

            // 2. Lọc theo giá (tối đa)
            if (maxPrice.HasValue && maxPrice > 0)
            {
                query = query.Where(p => p.Price <= maxPrice);
            }

            // 3. Sắp xếp
            query = sortOrder switch
            {
                "price_desc" => query.OrderByDescending(p => p.Price),
                "price_asc" => query.OrderBy(p => p.Price),
                _ => query.OrderByDescending(p => p.Id) // Mặc định mới nhất
            };

            // Truyền dữ liệu cho View bộ lọc
            ViewBag.Categories = await _context.Categories.ToListAsync();
            ViewBag.MaxPrice = await _context.Products.MaxAsync(p => (decimal?)p.Price) ?? 5000000;
            ViewBag.CurrentSort = sortOrder;

            return View(await query.ToListAsync());
        }

        // =========================
        // DETAILS
        // =========================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            ViewBag.Reviews = await _context.Reviews
                .Where(r => r.ProductId == id)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return View(product);
        }

        // =========================
        // CREATE / EDIT / DELETE (Giữ nguyên logic của bạn)
        // =========================

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Product product)
        {
            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                return View(product);
            }

            product.ImageUrl = (product.ImageUrl ?? "").Trim();

            // Xử lý Sale logic và UTC
            if (product.IsOnSale)
            {
                if (product.SaleStartDate.HasValue) product.SaleStartDate = DateTime.SpecifyKind(product.SaleStartDate.Value, DateTimeKind.Utc);
                if (product.SaleEndDate.HasValue) product.SaleEndDate = DateTime.SpecifyKind(product.SaleEndDate.Value, DateTimeKind.Utc);
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Products.Any(e => e.Id == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddReview(int productId, int rating, string comment)
        {
            var user = await _userManager.GetUserAsync(User);
            var review = new Review
            {
                ProductId = productId,
                Rating = rating,
                Comment = comment ?? "",
                UserName = user?.UserName ?? "User",
                CreatedAt = DateTime.UtcNow
            };
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }
    }
}