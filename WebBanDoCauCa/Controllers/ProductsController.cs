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
        // LIST PRODUCT
        // =========================
        public async Task<IActionResult> Index(int? categoryId, string brand, decimal? maxPrice, string sortOrder)
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            if (categoryId.HasValue)
                query = query.Where(p => p.CategoryId == categoryId);

            if (!string.IsNullOrWhiteSpace(brand))
                query = query.Where(p => p.Brand == brand);

            if (maxPrice.HasValue)
                query = query.Where(p => p.Price <= maxPrice);

            query = sortOrder switch
            {
                "price_desc" => query.OrderByDescending(p => p.Price),
                _ => query.OrderBy(p => p.Price)
            };

            ViewBag.Brands = await _context.Products.Select(p => p.Brand).Distinct().ToListAsync();
            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", categoryId);

            return View(await query.ToListAsync());
        }

        // =========================
        // CREATE (GET)
        // =========================
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // =========================
        // CREATE (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Product product)
        {
            // ── DEBUG: in lỗi ModelState ra Render Logs ──
            if (!ModelState.IsValid)
            {
                Console.WriteLine("=== [Create] ModelState INVALID ===");
                foreach (var kvp in ModelState)
                {
                    foreach (var err in kvp.Value.Errors)
                        Console.WriteLine($"  Field '{kvp.Key}': {err.ErrorMessage}");
                }

                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                return View(product);
            }

            try
            {
                // Fix ImageUrl: không để null, dùng empty string để tránh NOT NULL constraint
                product.ImageUrl = string.IsNullOrWhiteSpace(product.ImageUrl)
                    ? ""
                    : product.ImageUrl.Trim();

                // Fix Sale logic
                if (!product.IsOnSale)
                {
                    product.DiscountPercent = 0;
                    product.SaleStartDate = null;
                    product.SaleEndDate = null;
                }
                else
                {
                    // Fix UTC cho PostgreSQL/Neon: phải là DateTimeKind.Utc
                    if (product.SaleStartDate.HasValue)
                        product.SaleStartDate = product.SaleStartDate.Value.Kind == DateTimeKind.Utc
                            ? product.SaleStartDate.Value
                            : DateTime.SpecifyKind(product.SaleStartDate.Value, DateTimeKind.Utc);

                    if (product.SaleEndDate.HasValue)
                        product.SaleEndDate = product.SaleEndDate.Value.Kind == DateTimeKind.Utc
                            ? product.SaleEndDate.Value
                            : DateTime.SpecifyKind(product.SaleEndDate.Value, DateTimeKind.Utc);
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                Console.WriteLine($"=== [Create] Product '{product.Name}' saved OK (Id={product.Id}) ===");
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbEx)
            {
                var msg = dbEx.InnerException?.Message ?? dbEx.Message;
                Console.WriteLine($"=== [Create] DbUpdateException: {msg} ===");
                ModelState.AddModelError("", $"Lỗi lưu dữ liệu: {msg}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== [Create] Exception: {ex} ===");
                ModelState.AddModelError("", $"Lỗi hệ thống: {ex.Message}");
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // =========================
        // EDIT (GET)
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

        // =========================
        // EDIT (POST)
        // =========================
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Product product)
        {
            if (id != product.Id) return NotFound();

            // ── DEBUG: in lỗi ModelState ra Render Logs ──
            if (!ModelState.IsValid)
            {
                Console.WriteLine("=== [Edit] ModelState INVALID ===");
                foreach (var kvp in ModelState)
                {
                    foreach (var err in kvp.Value.Errors)
                        Console.WriteLine($"  Field '{kvp.Key}': {err.ErrorMessage}");
                }

                ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                return View(product);
            }

            try
            {
                product.ImageUrl = string.IsNullOrWhiteSpace(product.ImageUrl)
                    ? ""
                    : product.ImageUrl.Trim();

                if (!product.IsOnSale)
                {
                    product.DiscountPercent = 0;
                    product.SaleStartDate = null;
                    product.SaleEndDate = null;
                }
                else
                {
                    if (product.SaleStartDate.HasValue)
                        product.SaleStartDate = product.SaleStartDate.Value.Kind == DateTimeKind.Utc
                            ? product.SaleStartDate.Value
                            : DateTime.SpecifyKind(product.SaleStartDate.Value, DateTimeKind.Utc);

                    if (product.SaleEndDate.HasValue)
                        product.SaleEndDate = product.SaleEndDate.Value.Kind == DateTimeKind.Utc
                            ? product.SaleEndDate.Value
                            : DateTime.SpecifyKind(product.SaleEndDate.Value, DateTimeKind.Utc);
                }

                _context.Update(product);
                await _context.SaveChangesAsync();

                Console.WriteLine($"=== [Edit] Product Id={product.Id} updated OK ===");
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Products.AnyAsync(p => p.Id == product.Id))
                    return NotFound();
                throw;
            }
            catch (DbUpdateException dbEx)
            {
                var msg = dbEx.InnerException?.Message ?? dbEx.Message;
                Console.WriteLine($"=== [Edit] DbUpdateException: {msg} ===");
                ModelState.AddModelError("", $"Lỗi lưu dữ liệu: {msg}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== [Edit] Exception: {ex} ===");
                ModelState.AddModelError("", $"Lỗi hệ thống: {ex.Message}");
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
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
        // DELETE (GET)
        // =========================
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null) return NotFound();

            return View(product);
        }

        // =========================
        // DELETE (POST)
        // =========================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
                Console.WriteLine($"=== [Delete] Product Id={id} deleted OK ===");
            }
            return RedirectToAction(nameof(Index));
        }

        // =========================
        // ADD REVIEW
        // =========================
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddReview(int productId, int rating, string comment)
        {
            try
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
            catch (Exception ex)
            {
                Console.WriteLine($"=== [AddReview] Exception: {ex.Message} ===");
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}