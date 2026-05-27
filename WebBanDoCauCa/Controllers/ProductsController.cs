using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBanDoCauCa.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace WebBanDoCauCa.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        // ĐÃ SỬA: Chuyển từ ApplicationUser sang ApplicationUser
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductsController(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ==========================================================
        // KHU VỰC NGƯỜI DÙNG (PUBLIC)
        // ==========================================================

        public async Task<IActionResult> Index(int? categoryId, string brand, decimal? maxPrice, string sortOrder)
        {
            var productsQuery = _context.Products
                .Include(p => p.Category)
                .AsQueryable();

            if (categoryId.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId);
                var currentCategory = await _context.Categories.FindAsync(categoryId);
                ViewData["CurrentCategory"] = currentCategory?.Name ?? "Sản phẩm";
            }
            else
            {
                ViewData["CurrentCategory"] = "Tất cả sản phẩm";
            }

            if (!string.IsNullOrEmpty(brand))
            {
                productsQuery = productsQuery.Where(p => p.Brand == brand);
            }

            if (maxPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price <= maxPrice);
            }

            switch (sortOrder)
            {
                case "price_desc":
                    productsQuery = productsQuery.OrderByDescending(p => p.Price);
                    break;
                case "price_asc":
                default:
                    productsQuery = productsQuery.OrderBy(p => p.Price);
                    break;
            }

            ViewBag.Brands = await _context.Products.Select(p => p.Brand).Distinct().ToListAsync();
            ViewBag.MinPrice = _context.Products.Any() ? await _context.Products.MinAsync(p => p.Price) : 0;
            ViewBag.MaxPrice = _context.Products.Any() ? await _context.Products.MaxAsync(p => p.Price) : 0;
            ViewBag.Categories = new SelectList(_context.Categories, "Id", "Name", categoryId);

            return View(await productsQuery.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (product == null) return NotFound();

            ViewBag.Reviews = await _context.Reviews
                .Where(r => r.ProductId == product.Id)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> SearchSuggestions(string term)
        {
            if (string.IsNullOrWhiteSpace(term) || term.Length < 1)
                return Json(new List<object>());

            var suggestions = await _context.Products
                .Where(p => p.Name.Contains(term))
                .Select(p => new {
                    id = p.Id,
                    name = p.Name,
                    price = p.Price.ToString("N0") + "đ"
                })
                .Take(5)
                .ToListAsync();

            return Json(suggestions);
        }

        // ==========================================================
        // KHU VỰC QUẢN TRỊ (ADMIN ONLY)
        // ==========================================================

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            PrepareCategoryList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Price,ImageUrl,CategoryId,IsOnSale,DiscountPercent,SaleStartDate,SaleEndDate,Brand")] Product product)
        {
            if (ModelState.IsValid)
            {
                if (!product.IsOnSale)
                {
                    product.DiscountPercent = 0;
                    product.SaleStartDate = null;
                    product.SaleEndDate = null;
                }
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            PrepareCategoryList(product.CategoryId);
            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();
            PrepareCategoryList(product.CategoryId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Price,ImageUrl,CategoryId,IsOnSale,DiscountPercent,SaleStartDate,SaleEndDate,Brand")] Product product)
        {
            if (id != product.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    if (!product.IsOnSale)
                    {
                        product.DiscountPercent = 0;
                        product.SaleStartDate = null;
                        product.SaleEndDate = null;
                    }
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            PrepareCategoryList(product.CategoryId);
            return View(product);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(m => m.Id == id);
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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddReview(int productId, int rating, string comment)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                Review review = new Review
                {
                    ProductId = productId,
                    Rating = rating,
                    Comment = comment,
                    UserName = user?.UserName ?? "User",
                    CreatedAt = DateTime.Now
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

        private void PrepareCategoryList(int? selectedId = null)
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", selectedId);
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}