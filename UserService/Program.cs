using Microsoft.EntityFrameworkCore;
using UserService.Models;
using UserService.Application.Clients;
using UserService.Application.Handlers;
using EasyNetQ;
using UserService.Core.Services;
using UserService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Add database context
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("SQLiteDatabasePath")));

// Add services
builder.Services.AddSingleton(
    new MessageClient(RabbitHutch.CreateBus("host=localhost:5672;port=5672;virtualHost=;username=guest;password=guest")));
builder.Services.AddHostedService<MessageHandler>();
builder.Services.AddScoped<UserManager>();
builder.Services.AddScoped<UserRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
//app.MapControllers();

app.Run();
