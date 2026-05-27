using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using WebBanDoCauCa.Models;

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
        // INDEX (FILTER FULL FIX)
        // =========================
        public async Task<IActionResult> Index(string categoryIds, decimal? minPrice, decimal? maxPrice, string sortOrder)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            // =========================
            // 1. Filter Category (SAFE PARSE)
            // =========================
            if (!string.IsNullOrWhiteSpace(categoryIds))
            {
                var idList = categoryIds
                    .Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => int.TryParse(x, out var v) ? v : 0)
                    .Where(x => x > 0)
                    .ToList();

                query = query.Where(p => idList.Contains(p.CategoryId));
            }

            // =========================
            // 2. Filter Price (MIN - MAX)
            // =========================
            if (minPrice.HasValue)
                query = query.Where(p => p.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice.Value);

            // =========================
            // 3. Sort
            // =========================
            query = sortOrder switch
            {
                "price_desc" => query.OrderByDescending(p => p.Price),
                "price_asc" => query.OrderBy(p => p.Price),
                _ => query.OrderByDescending(p => p.Id)
            };

            // =========================
            // 4. ViewBag FILTER DATA
            // =========================
            var productList = await _context.Products.ToListAsync();

            ViewBag.Categories = await _context.Categories.ToListAsync();

            ViewBag.MinPrice = productList.Min(p => (decimal?)p.Price) ?? 0;
            ViewBag.MaxPrice = productList.Max(p => (decimal?)p.Price) ?? 5000000;

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
        // CREATE
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

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // EDIT
        // =========================
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
                    if (!_context.Products.Any(e => e.Id == id))
                        return NotFound();
                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // =========================
        // REVIEW
        // =========================
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