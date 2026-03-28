using AudioGuideAPI.Database;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Đăng ký DbContext và dùng SQLite
builder.Services.AddDbContext<AppDbContext>(options =>              //Khi app cần làm việc với DB, thì cần dùng AppDbContext
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));     //Database dùng loại SQLite
                                                                                            //Lấy chuỗi kết nối từ appsettings.json

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();