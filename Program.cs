using CharityProject.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using IronPdf;
using Microsoft.Extensions.Logging;
using System.IO;

var builder = WebApplication.CreateBuilder(args);



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

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.File(Path.Combine("Logs", $"log-{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt"),
        rollingInterval: RollingInterval.Infinite,
        retainedFileCountLimit: 5)
    .CreateLogger();

builder.Host.UseSerilog();

var app = builder.Build();


// Configure the HTTP request pipeline.
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

app.MapControllerRoute(
name: "default",
pattern: "{controller=Home}/{action=LoginPage}/{id?}");
app.MapRazorPages();
IronPdf.License.LicenseKey = "IRONSUITE.Z1EX3VY5Z.RELAY.FIREFOX.COM.7292-658CF33405-BZF3RPP3HBLVOH-3GR3YXCKLQZO-TC5LPG4R4Y3I-HEWQ4YQSSFMO-TPO3HMFJU6ZK-P67MZSAWCZX5-MRWIUO-TRNLFLGTPNGNEA-DEPLOYMENT.TRIAL-F42NKS.TRIAL.EXPIRES.17.AUG.2024";

//Run the application and log any errors
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

/*
using CharityProject.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using IronPdf;
using Microsoft.Extensions.Logging;
using System.IO;
using Microsoft.Extensions.Options;
using static CharityProject.Controllers.HomeController;
using CharityProject.Controllers;
using CharityProject;

var builder = WebApplication.CreateBuilder(args);

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

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.File(Path.Combine("Logs", $"log-{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt"),
        rollingInterval: RollingInterval.Infinite,
        retainedFileCountLimit: 5)
    .CreateLogger();

builder.Host.UseSerilog();
builder.Services.AddTransient<EmailService>();  // Register EmailService

// Add SMTP settings configuration
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));

var app = builder.Build();

// Configure the HTTP request pipeline.
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

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=LoginPage}/{id?}");

app.MapRazorPages();
IronPdf.License.LicenseKey = "IRONSUITE.Z1EX3VY5Z.RELAY.FIREFOX.COM.7292-658CF33405-BZF3RPP3HBLVOH-3GR3YXCKLQZO-TC5LPG4R4Y3I-HEWQ4YQSSFMO-TPO3HMFJU6ZK-P67MZSAWCZX5-MRWIUO-TRNLFLGTPNGNEA-DEPLOYMENT.TRIAL-F42NKS.TRIAL.EXPIRES.17.AUG.2024";

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
}*/