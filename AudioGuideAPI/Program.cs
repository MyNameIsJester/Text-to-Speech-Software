using AudioGuideAPI.Configuration;
using AudioGuideAPI.Database;
using AudioGuideAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler =
        System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<ElevenLabsOptions>(
    builder.Configuration.GetSection("ElevenLabs"));

builder.Services.Configure<FptTtsOptions>(
    builder.Configuration.GetSection("FptTts"));

builder.Services.AddHttpClient();

builder.Services.AddScoped<ElevenLabsTtsService>();
builder.Services.AddScoped<FptTtsService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AudioGuideWebPolicy", policy =>
    {
        policy.WithOrigins(
                "https://localhost:7296",
                "http://localhost:5296",
                "http://audioguideweb.somee.com",
                "https://audioguideweb.somee.com"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
    await DbSeeder.SeedAsync(db);
}

app.UseStaticFiles();
app.UseCors("AudioGuideWebPolicy");
app.UseAuthorization();
app.MapControllers();

app.Run();