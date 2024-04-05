using EasyNetQ;
using Microsoft.EntityFrameworkCore;
using PostService.Application.Clients;
using PostService.Application.Handlers;
using PostService.Application.Interfaces.Clients;
using PostService.Application.Interfaces.Repositories;
using PostService.Core.Services;
using PostService.Infrastructure.Context;
using PostService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("SQLiteDatabasePath")));

builder.Services.AddSingleton<IMessageClient>(
    new MessageClient(RabbitHutch.CreateBus("host=rabbitmq;port=5672;virtualHost=/;username=guest;password=guest")));
builder.Services.AddHostedService<PostServiceMessageHandler>();
builder.Services.AddScoped<PostManager>();
builder.Services.AddScoped<IPostRepository, PostRepository>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
    dbContext.Database.Migrate();
}

// Configure the HTTP request pipeline.
app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();


app.Run();
