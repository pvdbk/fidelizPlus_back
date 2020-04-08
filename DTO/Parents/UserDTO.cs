using System;

namespace fidelizPlus_back.DTO
{
    public class UserDTO<T>
    {
        public int? Id { get; set; }
        public string Surname { get; set; }
        public string FirstName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConnectionId { get; set; }
        public DateTime? CreationTime { get; set; }
        public T Account { get; set; }
    }
}
