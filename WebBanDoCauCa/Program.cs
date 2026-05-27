using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.DataProtection;
using WebBanDoCauCa.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// 1. DATABASE
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString)
);

// 2. DATA PROTECTION
builder.Services.AddDataProtection()
    .PersistKeysToDbContext<ApplicationDbContext>();

// 3. IDENTITY
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

// 4. SESSION - CẤU HÌNH QUAN TRỌNG CHO RENDER/HOSTING
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    // THÊM DÒNG NÀY ĐỂ TRÁNH MẤT SESSION KHI HOSTING
    options.Cookie.SameSite = SameSiteMode.Lax;
});

// 5. MVC + RAZOR
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// 6. MIGRATION
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var db = services.GetRequiredService<ApplicationDbContext>();
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"=== Migration FAILED: {ex.Message} ===");
    }
}

// 7. MIDDLEWARE (THỨ TỰ RẤT QUAN TRỌNG)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Cấu hình ForwardedHeaders để nhận diện đúng giao thức HTTPS từ Proxy (Render)
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Cần UseSession TRƯỚC UseAuthentication/Authorization để các Controller có thể dùng được Session
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// 8. ROUTES
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Products}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();