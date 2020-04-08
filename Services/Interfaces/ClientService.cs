namespace fidelizPlus_back.Services
{
    using AppModel;
    using DTO;

    public interface ClientService : UserService<Client, ClientDTO, ExtendedClientDTO, ClientAccountDTO>
    {
    }
}
