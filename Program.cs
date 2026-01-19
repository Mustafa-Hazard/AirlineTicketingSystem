using AirlineTicketingSystem.Models;
using AirlineTicketingSystem.Models.Settings;
using AirlineTicketingSystem.Services;
using AirlineTicketingSystem.Data; // Ensure this matches your namespace for DbInitializer
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// 1. Set the QuestPDF license
QuestPDF.Settings.License = LicenseType.Community;

// 2. Add services to the container.
builder.Services.AddControllersWithViews();

// 3. Configure DatabaseContext
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

// 4. Configure JWT Authentication
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Look for the token in the "jwt_token" cookie
                var token = context.Request.Cookies["jwt_token"];
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }
                return Task.CompletedTask;
            }
        };
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "https://localhost:7175/",
            ValidAudience = "https://localhost:7175/",
            IssuerSigningKey = new SymmetricSecurityKey(
                System.Text.Encoding.UTF8.GetBytes("class-work-5E-your-super-secret-key-minimum-32-characters"))
        };
    });

builder.Services.AddAuthorization();

// 5. Configure Email Services
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

// ==========================================
// 6. SEED DATABASE (DbInitializer)
// ==========================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<DatabaseContext>();
        // This will create the DB, run HasData seeds, and add your Admin user
        DbInitializer.Initialize(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}
// ==========================================

// 7. Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Middleware Order: Auth must come after Routing and before MapControllerRoute
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Splash}/{action=Index}/{id?}");

app.Run();