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

        public ProductsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ==========================================================
        // PUBLIC - LIST PRODUCTS
        // ==========================================================
        public async Task<IActionResult> Index(int? categoryId, string brand, decimal? maxPrice, string sortOrder)
        {
            var query = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            if (!string.IsNullOrEmpty(brand))
            {
                query = query.Where(p => p.Brand == brand);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice);
            }

            query = sortOrder switch
            {
                "price_desc" => query.OrderByDescending(p => p.Price),
                _ => query.OrderBy(p => p.Price)
            };

            ViewBag.Brands = await _context.Products
                .Select(p => p.Brand)
                .Distinct()
                .ToListAsync();

            ViewBag.Categories = new SelectList(await _context.Categories.ToListAsync(), "Id", "Name", categoryId);

            return View(await query.ToListAsync());
        }

        // ==========================================================
        // DETAILS
        // ==========================================================
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

        // ==========================================================
        // CREATE (ADMIN)
        // ==========================================================
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
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                    return View(product);
                }

                // 🔥 FIX NULL IMAGE
                product.ImageUrl ??= "";

                // 🔥 CHECK CATEGORY EXISTS
                var categoryExists = await _context.Categories
                    .AnyAsync(c => c.Id == product.CategoryId);

                if (!categoryExists)
                {
                    return Content("CategoryId không tồn tại trong database");
                }

                // SALE LOGIC
                if (!product.IsOnSale)
                {
                    product.DiscountPercent = 0;
                    product.SaleStartDate = null;
                    product.SaleEndDate = null;
                }

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                // 🔥 HIỆN LỖI THẬT TRÊN RENDER
                return Content(ex.ToString());
            }
        }

        // ==========================================================
        // EDIT
        // ==========================================================
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

            try
            {
                if (!ModelState.IsValid)
                {
                    ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
                    return View(product);
                }

                product.ImageUrl ??= "";

                if (!product.IsOnSale)
                {
                    product.DiscountPercent = 0;
                    product.SaleStartDate = null;
                    product.SaleEndDate = null;
                }

                _context.Update(product);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                return Content(ex.ToString());
            }
        }

        // ==========================================================
        // DELETE
        // ==========================================================
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
            }

            return RedirectToAction(nameof(Index));
        }

        // ==========================================================
        // REVIEW
        // ==========================================================
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
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}