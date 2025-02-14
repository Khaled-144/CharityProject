using CharityProject.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Microsoft.Extensions.Logging;
using System.IO;
using OfficeOpenXml;
using CharityProject.Services;
using CharityProject.Middleware;

var builder = WebApplication.CreateBuilder(args);

ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // or LicenseContext.Commercial if applicable

// Add services to the container.
builder.Services.AddControllersWithViews();

// Configure the database connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Add DbContext with SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add Identity services
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddSession(options => { options.IdleTimeout = TimeSpan.FromMinutes(10); });

var logPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
if (Directory.Exists(logPath))
{
    var cutoff = DateTime.Now.AddDays(-7);
    foreach (var file in Directory.GetFiles(logPath, "*.txt"))
    {
        var fileInfo = new FileInfo(file);
        if (fileInfo.CreationTime < cutoff)
        {
            try
            {
                fileInfo.Delete();
            }
            catch (Exception ex)
            {
                Log.Warning($"Could not delete old log file {file}: {ex.Message}");
            }
        }
    }
}

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.File(
        path: Path.Combine("Logs", $"log-{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt"),
        fileSizeLimitBytes: 10 * 1024 * 1024, // 10MB
        rollOnFileSizeLimit: true,
        retainedFileCountLimit: null, // Disable Serilog's retention
        flushToDiskInterval: TimeSpan.FromSeconds(30))
    .CreateLogger();

builder.Host.UseSerilog();

// Register EmailService with its interface
builder.Services.AddScoped<IEmailService, EmailService>(); // Use the interface

var app = builder.Build();


app.UseMiddleware<ErrorHandlingMiddleware>();

// Configure the HTTP request pipeline.


app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=LoginPage}/{id?}");
app.MapRazorPages();

// Run the application and log any errors
try
{
    Log.Information("Starting web application");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
