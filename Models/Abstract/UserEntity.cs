namespace fidelizPlus_back.Models
{
    public interface UserEntity
    {
        public int UserId { get; set; }
        public string ConnectionId { get; set; }
        public User User { get; set; }
    }
}
