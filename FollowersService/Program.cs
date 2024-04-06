using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using FollowersService.Application.Clients;
using FollowersService.Application.Handlers;
using FollowersService.Application.Interfaces.Repositories;
using FollowersService.Models;
using FollowersService.Core.Services;
using FollowersService.Infrastructure.Repositories;
using EasyNetQ.Consumer;
using FollowersService.Application.Interfaces.Clients;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add database context
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("SQLiteDatabasePath")));

builder.Services.AddControllers();
builder.Services.AddSingleton<IMessageClient>(new MessageClient(RabbitHutch.CreateBus("host=rabbitmq;port=5672;virtualHost=/;username=guest;password=guest")));
builder.Services.AddHostedService<FollowerMessageHandler>();
builder.Services.AddScoped<FollowingManager>();
builder.Services.AddScoped<IFollowersRepository, FollowersRepository>();

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