namespace fidelizPlus_back.DTO
{
    public abstract class UserDTO : DTO
    {
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConnectionId { get; set; }
    }
}
