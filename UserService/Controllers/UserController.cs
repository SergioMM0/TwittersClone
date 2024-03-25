using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserService.Models;
using UserService.Application.Clients;
using RabbitMQMessages;

namespace UserService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserServiceController : ControllerBase
    {
        private readonly DatabaseContext _dbContext;
        private readonly MessageClient _messageClient;

        public UserServiceController(DatabaseContext dbContext, MessageClient messageClient)
        {
            _dbContext = dbContext;
            _messageClient = messageClient;
        }

        // POST: Authenticate User
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] RequestAuthMsg authRequest)
        {
            var userExists = await _dbContext.Users.AnyAsync(u => u.Name == authRequest.Username);
            if (!userExists)
            {
                return NotFound(new { Message = "Username does not exist." });
            }

            _messageClient.Send(authRequest, "auth.request");
            return Accepted(new { Message = "Authentication request received. Processing..." });
        }

        // GET: All Users
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _dbContext.Users.ToListAsync();
            return Ok(users);
        }

        // GET: User by ID
        [HttpGet("users/{id}")]
        public async Task<IActionResult> GetUserById(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            return Ok(user);
        }

        // POST: Add New User
        [HttpPost("users")]
        public async Task<IActionResult> AddUser([FromBody] Users user)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
        }

        // PUT: Update User's Username
        [HttpPut("username/{id}")]
        public async Task<IActionResult> UpdateUsername(int id, [FromBody] Users updatedUser)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.Name = updatedUser.Name;
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        // PUT: Update User's Password
        [HttpPut("userpassword/{id}")]
        public async Task<IActionResult> UpdatePassword(int id, [FromBody] Users updatedUser)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            user.Password = updatedUser.Password;
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: Remove User
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            return NoContent();
        }
    }
}
