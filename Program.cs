using AirlineTicketingSystem.Models;
using AirlineTicketingSystem.Models.Settings;
using AirlineTicketingSystem.Services;
using AirlineTicketingSystem.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using QuestPDF.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
QuestPDF.Settings.License = LicenseType.Community;
builder.Services.AddControllersWithViews();

var rawDbUrl = Environment.GetEnvironmentVariable("DATABASE_URL") 
    ?? Environment.GetEnvironmentVariable("POSTGRES_URL") 
    ?? Environment.GetEnvironmentVariable("POSTGRESUL_URL");

string? connectionString;
bool usePostgres;

if (!string.IsNullOrEmpty(rawDbUrl))
{
    usePostgres = true;
    connectionString = FormatPostgresUrl(rawDbUrl);
}
else
{
    usePostgres = builder.Configuration.GetValue<bool>("UsePostgreSQL", false);
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
}

if (usePostgres && !string.IsNullOrEmpty(connectionString))
    builder.Services.AddDbContext<DatabaseContext>(options => options.UseNpgsql(connectionString));
else
    builder.Services.AddDbContext<DatabaseContext>(options => options.UseSqlServer(connectionString));

var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? builder.Configuration["Jwt:Issuer"] ?? "https://aeronexa.up.railway.app/";
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? builder.Configuration["Jwt:Audience"] ?? "https://aeronexa.up.railway.app/";
var jwtKey = Environment.GetEnvironmentVariable("JWT_KEY") ?? builder.Configuration["Jwt:Key"] ?? "class-work-5E-your-super-secret-key-minimum-32-characters";

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["jwt_token"];
                if (!string.IsNullOrEmpty(token)) context.Token = token;
                return Task.CompletedTask;
            }
        };
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, ValidateAudience = true, ValidateLifetime = true, ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer, ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddScoped<IEmailService, EmailService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    try { DbInitializer.Initialize(scope.ServiceProvider.GetRequiredService<DatabaseContext>()); }
    catch (Exception ex) { scope.ServiceProvider.GetRequiredService<ILogger<Program>>().LogError(ex, "DB seed error."); }
}

if (!app.Environment.IsDevelopment()) { app.UseExceptionHandler("/Home/Error"); app.UseHsts(); }

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(name: "default", pattern: "{controller=Splash}/{action=Index}/{id?}");
app.Run();

static string FormatPostgresUrl(string url)
{
    if (url.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) || 
        url.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
    {
        try
        {
            var uri = new Uri(url);
            var userInfo = uri.UserInfo.Split(':');
            var user = userInfo.Length > 0 ? Uri.UnescapeDataString(userInfo[0]) : "";
            var pass = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";
            var port = uri.Port > 0 ? uri.Port : 5432;
            var db = uri.AbsolutePath.TrimStart('/');
            return $"Host={uri.Host};Port={port};Database={db};Username={user};Password={pass};SSL Mode=Prefer;Trust Server Certificate=true;";
        }
        catch
        {
            return url;
        }
    }
    return url;
}
