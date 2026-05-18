using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Northwind.Mvc.Models;
using Northwind.Mvc.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("NorthwindConnection")
    ?? throw new InvalidOperationException("Connection string 'NorthwindConnection' not found.");
var projectConnectionString = builder.Configuration.GetConnectionString("ProjectConnection")
    ?? throw new InvalidOperationException("Connection string 'ProjectConnection' not found.");

builder.Services.AddDbContext<NorthwindContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDbContext<CateringProjectContext>(options =>
    options.UseSqlServer(projectConnectionString));

builder.Services.Configure<CateringPlatformOptions>(builder.Configuration.GetSection("CateringPlatform"));
builder.Services.AddScoped<ILogService, LogService>();
builder.Services.AddScoped<ILocationService, DistanceLocationService>();
builder.Services.AddScoped<IEmailService, DemoEmailService>();
builder.Services.AddScoped<IPdfService, SimplePdfService>();
builder.Services.AddScoped<IUserClaimsPrincipalFactory<ApplicationUser>, CateraUserClaimsPrincipalFactory>();
builder.Services.AddSingleton<ICartService, CartService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.Name = "Catera.Session";
    options.IdleTimeout = TimeSpan.FromHours(2);
});
builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<CateringProjectContext>()
    .AddDefaultTokenProviders();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Auth/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(3);
});
builder.Services.AddAuthorization(options =>
{
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
});
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

var app = builder.Build();

await CateraIdentitySeeder.SeedAsync(app.Services);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Shared/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/Shared/Error", "?code={0}");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapHub<Northwind.Mvc.Hubs.CateringCallHub>("/catering-call-hub");

app.Run();
