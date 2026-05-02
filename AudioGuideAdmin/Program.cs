using AudioGuideAdmin.Database;
using AudioGuideAdmin.Models;
using AudioGuideAPI.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using AudioGuideAdmin.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddHttpClient<AdminFoodStallApiService>();

builder.Services.AddHttpClient<AdminTourApiService>();

builder.Services.AddHttpClient<WebVisitApiService>();

builder.Services.AddHttpClient<AdminPlaybackLogApiService>();

// Tạo đường dẫn tuyệt đối tới file DB thật của AudioGuideAPI
var domainDbPath = Path.GetFullPath(
    Path.Combine(builder.Environment.ContentRootPath, "..", "AudioGuideAPI", "audio_guide.db")
);

// DB cho domain data dùng chung với AudioGuideAPI
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite($"Data Source={domainDbPath}"));

// DB riêng cho auth của Web Admin
var authDbPath = Path.GetFullPath(
    Path.Combine(builder.Environment.ContentRootPath, "admin_auth.db")
);

builder.Services.AddDbContext<AuthDbContext>(options =>
    options.UseSqlite($"Data Source={authDbPath}"));

// Identity
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;
        options.Password.RequiredUniqueChars = 1;
    })
    .AddEntityFrameworkStores<AuthDbContext>()
    .AddDefaultTokenProviders();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Dashboard}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var authDb = services.GetRequiredService<AuthDbContext>();
    await authDb.Database.MigrateAsync();

    await AuthSeeder.SeedAsync(services);
}

app.Run();