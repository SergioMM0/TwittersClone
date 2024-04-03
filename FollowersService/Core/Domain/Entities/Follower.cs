namespace FollowersService.Core.Domain.Entities
{
    public class Follower
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int FollowerId { get; set; }
    }
}