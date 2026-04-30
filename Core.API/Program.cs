using Core.API.Infrastructure;
using Core.Application.Consumer;
using Core.Application.Consumers;
using Core.Application.Features.Menus.Commands;
using Core.Application.Interfaces;
using Core.Infrastructure.Configuration;
using Core.Infrastructure.Persistence;
using Core.Infrastructure.Repositories;
using FluentValidation;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
var mongoSettings = builder.Configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>();
mongoSettings!.ConnectionString = builder.Configuration.GetConnectionString("MongoDbConnection")!;
// Đọc cấu hình, nếu không tìm thấy thì mặc định dùng "guest"
var rabbitMqHost = builder.Configuration["RabbitMqSettings:Host"] ?? "localhost";
var rabbitMqUser = builder.Configuration["RabbitMqSettings:Username"] ?? "guest";
var rabbitMqPass = builder.Configuration["RabbitMqSettings:Password"] ?? "guest";

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<MenuCreatedConsumer>();
    x.AddConsumer<MenuUpdatedConsumer>();
    x.AddConsumer<MenuDeletedConsumer>();
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitMqHost, "/", h =>
        {
            h.Username(rabbitMqUser);
            h.Password(rabbitMqPass);
        });

        cfg.ConfigureEndpoints(context);
    });
});
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
builder.Services.AddScoped(typeof(IGenericReadRepository<>), typeof(MongoGenericRepository<>));

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IMenuRepository, MenuRepository>();
builder.Services.AddScoped<IMenuReadRepository, MenuReadRepository>();
// TẠI SAO AddScoped cho UnitOfWork?
// - Scoped = 1 instance PER HTTP request
// - Mỗi request có 1 UoW riêng → không share transaction giữa các requests
// - Cùng lifetime với DbContext (cũng Scoped) → đảm bảo cùng DbContext
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
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
