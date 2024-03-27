
using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using UserService.Application.Clients;
using UserService.Application.Handlers;
using UserService.Core.Services;
using UserService.Infrastructure.Context;
using UserService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Add services

builder.Services.AddSingleton(new MessageClient(RabbitHutch.CreateBus("host=rabbitmq;port=5672;virtualHost=/;username=guest;password=guest")));
builder.Services.AddHostedService<MessageHandler>();
builder.Services.AddScoped<UserManager>();
builder.Services.AddScoped<UserRepository>();

builder.Services.AddDbContext<UserDbContext>(options =>
    options.UseSqlite(builder.Configuration
        .GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();

