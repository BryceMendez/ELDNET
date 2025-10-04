using ELDNET.Data;
using ELDNET.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies; // <--- ADD THIS USING STATEMENT

var builder = WebApplication.CreateBuilder(args);

// --- Add services to the container ----------------------------------------
builder.Services.AddControllersWithViews();

// EF Core (SQL Server)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// --- ADD AUTHENTICATION SERVICES HERE ----------------------------------
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) // Use CookieAuthenticationDefaults.AuthenticationScheme for clarity
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";         // Path to your login page
        options.AccessDeniedPath = "/Account/AccessDenied"; // Path for unauthorized access
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Cookie expiration
        options.SlidingExpiration = true;             // Renew cookie on activity
    });
// ----------------------------------------------------------------------

// Required for Session state
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Allow injecting IHttpContextAccessor
builder.Services.AddHttpContextAccessor();

// --------------------------------------------------------------------------

var app = builder.Build();

// --- Configure HTTP pipeline ----------------------------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// --- ADD AUTHENTICATION MIDDLEWARE HERE --------------------------------
// ✅ Enable sessions (must be before authentication if session is used for identity storage)
app.UseSession();
// ✅ Enable Authentication (MUST BE BEFORE Authorization)
app.UseAuthentication();
// ----------------------------------------------------------------------

app.UseAuthorization(); // This was already here, but now it has authentication to work with!

// Default route → Login page
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();