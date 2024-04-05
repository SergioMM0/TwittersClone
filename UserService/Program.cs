using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Clients;
using UserService.Application.Handlers;
using UserService.Application.Interfaces.Clients;
using UserService.Application.Interfaces.Repositories;
using UserService.Models;
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

builder.Services.AddSingleton<IMessageClient>(new MessageClient(RabbitHutch.CreateBus("host=rabbitmq;port=5672;virtualHost=/;username=guest;password=guest")));
builder.Services.AddHostedService<UserServiceMessageHandler>();
builder.Services.AddScoped<UserManager>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

// Ensure the database is created and up-to-date
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();
