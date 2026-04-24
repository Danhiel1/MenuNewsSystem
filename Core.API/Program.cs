using Core.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Core.Application.Interfaces;
using Core.Infrastructure.Repositories;
using Core.Application.Features.Menus.Commands;
using FluentValidation;
using MediatR;
using Core.API.Infrastructure;
using MongoDB.Driver;
using Core.Infrastructure.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
var mongoSettings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
mongoSettings!.ConnectionString = builder.Configuration.GetConnectionString("MongoDbConnection")!;

// Đăng ký IMongoClient và IMongoDatabase
builder.Services.AddSingleton<IMongoClient>(new MongoClient(mongoSettings.ConnectionString));
builder.Services.AddScoped<IMongoDatabase>(sp =>
{
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(mongoSettings.DatabaseName);
});
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Đăng ký Repository
builder.Services.AddScoped<IMenuRepository, MenuRepository>();
// Đăng ký MediatR (Quét toàn bộ tầng Application để tìm Handler)
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CreateMenuCommand).Assembly));
// Đăng ký FluentValidation (Quét toàn bộ tầng Application để tìm Validator)
builder.Services.AddValidatorsFromAssembly(typeof(CreateMenuCommand).Assembly);
// Đăng ký Pipeline Behavior để tự động gọi Validator trước khi vào Handler
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Core.Application.Behaviors.ValidationBehavior<,>));
// Đăng ký Middleware xử lý lỗi toàn cục
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
var app = builder.Build();
app.UseExceptionHandler();
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
