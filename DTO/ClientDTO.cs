namespace fidelizPlus_back.DTO
{
    public class ClientDTO : UserDTO<ClientAccountDTO>
    {
        public string AdminPassword { get; set; }
    }
}
