namespace fidelizPlus_back.AppDomain
{
    public class PrivateClient : UserDTO<ClientAccountDTO>
    {
        public string AdminPassword { get; set; }
    }
}
