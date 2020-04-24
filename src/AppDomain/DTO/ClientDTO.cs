namespace fidelizPlus_back.AppDomain
{
    public class ClientDTO : UserDTO<ClientAccountDTO>
    {
        public string AdminPassword { get; set; }
    }
}
