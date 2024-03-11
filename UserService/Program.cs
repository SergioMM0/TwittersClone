using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using UserService.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<DatabaseContext>(options =>
    options.UseSqlite(builder.Configuration
        .GetConnectionString("userservice")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// GET all users
app.MapGet("/userservice/users", async (DatabaseContext dbContext) =>
{
    return await dbContext.Users.ToListAsync();
})
.WithName("GetUsers")
.WithMetadata(new HttpMethodMetadata(new[] { "GET" }))
.WithOpenApi();

// GET user by ID
app.MapGet("/userservice/users/{id}", async (int id, DatabaseContext dbContext) =>
{
    return await dbContext.Users.FindAsync(id);
})
.WithName("GetUserById")
.WithMetadata(new HttpMethodMetadata(new[] { "GET" }))
.WithOpenApi();

// POST new user
app.MapPost("/userservice/users", async (Users user, DatabaseContext dbContext) =>
{
    dbContext.Users.Add(user);
    await dbContext.SaveChangesAsync();
    return Results.Created($"/userservice/users/{user.Id}", user);
})
.WithName("AddUser")
.WithMetadata(new HttpMethodMetadata(new[] { "POST" }))
.WithOpenApi();

// PUT update username
app.MapPut("/userservice/username/{id}", async (int id, Users updatedUsername, DatabaseContext dbContext) =>
{
    var user = await dbContext.Users.FindAsync(id);
    if (user == null)
    {
        return Results.NotFound();
    }

    user.Name = updatedUsername.Name;
    await dbContext.SaveChangesAsync();
    return Results.NoContent();
})
.WithName("UpdateUsername")
.WithMetadata(new HttpMethodMetadata(new[] { "PUT" }))
.WithOpenApi();

// PUT update user password
app.MapPut("/userservice/userpassword/{id}", async (int id, Users updatedPassword, DatabaseContext dbContext) =>
{
    var user = await dbContext.Users.FindAsync(id);
    if (user == null)
    {
        return Results.NotFound();
    }

    user.Password = updatedPassword.Password;
    await dbContext.SaveChangesAsync();
    return Results.NoContent();
})
.WithName("UpdatePassword")
.WithMetadata(new HttpMethodMetadata(new[] { "PUT" }))
.WithOpenApi();

// DELETE user
app.MapDelete("/userservice/users/{id}", async (int id, DatabaseContext dbContext) =>
{
    var user = await dbContext.Users.FindAsync(id);
    if (user == null)
    {
        return Results.NotFound();
    }

    dbContext.Users.Remove(user);
    await dbContext.SaveChangesAsync();
    return Results.NoContent();
})
.WithName("DeleteUser")
.WithMetadata(new HttpMethodMetadata(new[] { "DELETE" }))
.WithOpenApi();

app.Run();
