using System.Collections.Generic;
using System.Linq;

namespace fidelizPlus_back.Services
{
    using DTO;
    using Models;
    using Repositories;

    public class TraderStandardService : UserStandardService<Trader, TraderDTO>, TraderService
    {
        private TraderAccountRepository accountRepo;
        private OfferRepository offerRepo;
        private CrudService<Client, ClientDTO> clientService;
        private FiltersHandler filtersHandler;

        public TraderStandardService(
            UserEntityRepository<Trader> entityRepo,
            Utils utils,
            CrudRepository<User> userRepo,
            CommercialLinkRepository commercialLinkRepo,
            TraderAccountRepository accountRepo,
            OfferRepository offerRepo,
            CrudService<Client, ClientDTO> clientService,
            FiltersHandler filtersHandler
        ) : base(entityRepo, utils, userRepo, commercialLinkRepo)
        {
            this.offerRepo = offerRepo;
            this.accountRepo = accountRepo;
            this.clientService = clientService;
            this.filtersHandler = filtersHandler;
        }

        public override bool IsRequiredProp(string propName)
        {
            return propName != "Id" && propName != "Phone" && propName != "LogoPath" && propName != "Address";
        }

        public override void Delete(int id)
        {
            int userId = this.FindEntity(id).UserId;
            this.ClRepo.NullifyTrader(id);
            this.accountRepo.DeleteTrader(id);
            this.offerRepo.NullifyTrader(id);
            this.repo.Delete(id);
            this.UserRepo.Delete(userId);
        }

        public IEnumerable<TraderAccountDTO> Accounts(int id)
        {
            Trader trader = this.FindEntity(id);
            this.repo.Entry(trader).Collection("TraderAccount").Load();
            return trader.TraderAccount.Select(account => this.Utils.Cast<TraderAccountDTO, TraderAccount>(account));
        }

        public TraderAccount FindAccountEntity(int traderId, int accountId)
        {
            TraderAccount account = this.accountRepo.FindById(accountId);
            if (account == null || account.TraderId != traderId)
            {
                throw new AppException("Account not found", 404);
            }
            return account;
        }

        public TraderAccountDTO FindAccount(int traderId, int accountId)
        {
            return this.Utils.Cast<TraderAccountDTO, TraderAccount>(this.FindAccountEntity(traderId, accountId));
        }

        public CommercialLink FindCommercialLink(int traderId, int clientId)
        {
            Trader trader = this.FindEntity(traderId);
            this.repo.Entry(trader).Collection("CommercialLink").Load();
            return trader.CommercialLink.Where(cl => cl.ClientId == clientId).FirstOrDefault();
        }

        public ClientDTOForTrader ExtendClientDTO(int traderId, ClientDTO dto)
        {
            if (dto.Id == null)
            {
                throw new AppException("Bad use of TraderStandardService.ExtendClientDTO");
            }
            ClientDTOForTrader ret = this.Utils.Cast<ClientDTOForTrader, ClientDTO>(dto);
            ret.Id = dto.Id;
            CommercialLink cl = this.FindCommercialLink(traderId, (int)dto.Id);
            if (cl != null)
            {
                ret.Flags = this.ClToDTO(cl).Flags;
            }
            return ret;
        }

        public IEnumerable<ClientDTOForTrader> Clients(int id, string filter)
        {
            Trader trader = this.FindEntity(id);
            this.repo.Entry(trader).Collection("CommercialLink").Load();
            foreach (CommercialLink cl in trader.CommercialLink)
            {
                this.ClRepo.Entry(cl).Reference("Client").Load();
            }
            IEnumerable<Client> clients = trader.CommercialLink.Select(cl => cl.Client);
            IEnumerable<ClientDTO> clientDTOs = clients.Select(client => this.clientService.EntityToDTO(client));
            IEnumerable<ClientDTOForTrader> ret = clientDTOs.Select(dto => this.ExtendClientDTO(id, dto));
            if (filter != null)
            {
                ret = this.filtersHandler.Apply(ret, new Tree(filter));
            }
            return ret;
        }
    }
}
