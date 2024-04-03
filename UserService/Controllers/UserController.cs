using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Models;
using UserService.Core.Services;
using RabbitMQMessages.Login;
using UserService.Core.Domain.Entities;

namespace UserService.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly UserManager _userManager;

    public UserController(UserManager userManager)
    {
        _userManager = userManager;
    }

    public void LocalTestAddUser()
    {
        _userManager.LocalTestAddUser();

    }
    /*
    private readonly UserManager _userManager;
    private readonly DatabaseContext _dbContext;

    public UserServiceController(UserManager userManager, DatabaseContext dbContext) {
        _userManager = userManager;
        _dbContext = dbContext;
    }

    // POST: Authenticate User
    [HttpPost("authenticate")]
    public IActionResult Authenticate([FromBody] LoginReqMsg LoginReqMsg) {
        _ = _userManager.HandleLogin(LoginReqMsg.Username, LoginReqMsg.Password);
        // Processes the authentication request

        return Accepted(new { Message = "Authentication request received. Processing..." });
    }

    // GET: All Users
    [HttpGet("users")]
    public async Task<IActionResult> GetAllUsers() {
        try {
            var users = await _dbContext.Users.ToListAsync();
            return Ok(users);
        }
        catch (Exception ex) {
            Console.WriteLine(ex.ToString());
            return StatusCode(500, "An internal error occurred.");
        }
    }

    // GET: User by ID
    [HttpGet("users/{id}")]
    public async Task<IActionResult> GetUserById(int id) {
        var user = await _dbContext.Users.FindAsync(id);
        if (user == null) {
            return NotFound();
        }
        return Ok(user);
    }

    // POST: Add New User
    [HttpPost("users")]
    public async Task<IActionResult> AddUser([FromBody] User user) {
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
    }

    // PUT: Update User's Username
    [HttpPut("username/{id}")]
    public async Task<IActionResult> UpdateUsername(int id, [FromBody] User updatedUser) {
        var user = await _dbContext.Users.FindAsync(id);
        if (user == null) {
            return NotFound();
        }

        user.Name = updatedUser.Name;
        await _dbContext.SaveChangesAsync();
        return NoContent();
    }

    // PUT: Update User's Password
    [HttpPut("userpassword/{id}")]
    public async Task<IActionResult> UpdatePassword(int id, [FromBody] Users updatedUser) {
        var user = await _dbContext.Users.FindAsync(id);
        if (user == null) {
            return NotFound();
        }

        user.Password = updatedUser.Password;
        await _dbContext.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: Remove User
    [HttpDelete("users/{id}")]
    public async Task<IActionResult> DeleteUser(int id) {
        var user = await _dbContext.Users.FindAsync(id);
        if (user == null) {
            return NotFound();
        }

        _dbContext.Users.Remove(user);
        await _dbContext.SaveChangesAsync();
        return NoContent();
    }
}
*/
}