namespace UserService.Core.Domain.Entities;

public class User {
    public Guid Id { get; set; }
    public required string Username { get; set; }
    public required string Password { get; set; }
}
