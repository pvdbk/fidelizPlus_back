namespace fidelizPlus_back.Services
{
    using AppModel;
    using DTO;

    public interface TraderService : UserService<Trader, TraderDTO, ExtendedTraderDTO, TraderAccountDTO>
    {
    }
}
