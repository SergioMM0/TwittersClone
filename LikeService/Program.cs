using EasyNetQ;
using LikeService.Application.Clients;
using LikeService.Application.Handlers;
using LikeService.Core.Services;
using LikeService.Infrastructure.Context;
using LikeService.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("SQLiteDatabasePath")));

builder.Services.AddSingleton(
    new MessageClient(RabbitHutch.CreateBus("host=rabbitmq;port=5672;virtualHost=/;username=guest;password=guest")));
builder.Services.AddHostedService<LikeServiceMessageHandler>();
builder.Services.AddScoped<LikeManager>();
builder.Services.AddScoped<LikeRepository>();

var app = builder.Build();

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
