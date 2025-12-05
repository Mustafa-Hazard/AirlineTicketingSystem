using AirlineTicketingSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using QuestPDF.Infrastructure;




var builder = WebApplication.CreateBuilder(args);
QuestPDF.Settings.License = LicenseType.Community;


// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure DatabaseContext
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Index";
        options.AccessDeniedPath = "/Auth/AccessDenied";
    });
builder.Services.AddAuthorization();
builder.Services.AddSession();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
