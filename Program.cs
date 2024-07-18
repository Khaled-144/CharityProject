using CharityProject.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PdfSharp.Fonts;
using IronPdf;


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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// Assign custom font resolver
GlobalFontSettings.FontResolver = new CustomFontResolver();


IronPdf.License.LicenseKey = "IRONSUITE.Z1EX3VY5Z.RELAY.FIREFOX.COM.7292-658CF33405-BZF3RPP3HBLVOH-3GR3YXCKLQZO-TC5LPG4R4Y3I-HEWQ4YQSSFMO-TPO3HMFJU6ZK-P67MZSAWCZX5-MRWIUO-TRNLFLGTPNGNEA-DEPLOYMENT.TRIAL-F42NKS.TRIAL.EXPIRES.17.AUG.2024";

app.Run();