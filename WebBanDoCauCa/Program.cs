using Microsoft.EntityFrameworkCore;
using WebBanDoCauCa.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// =====================================================
// FIX LỖI NEON POSTGRESQL (DateTime UTC)
// =====================================================
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

// =====================================================
// 1. DATABASE
// =====================================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)
);

// =====================================================
// 2. IDENTITY
// =====================================================
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// =====================================================
// 3. SESSION
// =====================================================
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// =====================================================
// 4. MVC + RAZOR
// =====================================================
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// =====================================================
// 5. DATABASE MIGRATION + SEED DATA
// =====================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        var db = services.GetRequiredService<ApplicationDbContext>();

        // Auto migrate
        db.Database.Migrate();

        // Seed categories nếu chưa có
        if (!db.Categories.Any())
        {
            db.Categories.AddRange(
                new Category { Name = "Cần câu" },
                new Category { Name = "Máy câu" },
                new Category { Name = "Dây câu" },
                new Category { Name = "Mồi câu" },
                new Category { Name = "Phụ kiện" }
            );

            db.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Database initialization error: {Message}", ex.Message);
    }
}

// =====================================================
// 6. MIDDLEWARE PIPELINE
// =====================================================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// =====================================================
// 7. ROUTES
// =====================================================
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();