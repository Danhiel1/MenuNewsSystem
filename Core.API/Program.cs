using Core.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Core.Application.Interfaces;
using Core.Infrastructure.Repositories;
using Core.Application.Features.Menus.Commands;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Đăng ký Repository
builder.Services.AddScoped<IMenuRepository, MenuRepository>();
// Đăng ký MediatR (Quét toàn bộ tầng Application để tìm Handler)
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateMenuCommand).Assembly));

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
