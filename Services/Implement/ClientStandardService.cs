using System.Collections.Generic;
using System.Linq;

namespace fidelizPlus_back.Services
{
    using DTO;
    using Models;
    using Repositories;

    public class ClientStandardService : UserStandardService<Client, ClientDTO>, ClientService
    {
        private ClientAccountRepository accountRepo;
        private ClientOfferRepository clientOfferRepo;
        private CrudService<Trader, TraderDTO> traderService;
        private FiltersHandler filtersHandler;

        public ClientStandardService(
            UserEntityRepository<Client> entityRepo,
            Utils utils,
            CrudRepository<User> userRepo,
            CommercialLinkRepository clRepo,
            ClientAccountRepository accountRepo,
            ClientOfferRepository clientOfferRepo,
            CrudService<Trader, TraderDTO> traderService,
            FiltersHandler filtersHandler
        ) : base(entityRepo, utils, userRepo, clRepo)
        {
            this.accountRepo = accountRepo;
            this.clientOfferRepo = clientOfferRepo;
            this.traderService = traderService;
            this.filtersHandler = filtersHandler;
        }

        public override bool IsRequiredProp(string propName)
        {
            return propName != "Id" && propName != "AdminPassword";
        }

        public override void Delete(int id)
        {
            int userId = this.FindEntity(id).UserId;
            this.accountRepo.DeleteClient(id);
            this.ClRepo.NullifyClient(id);
            this.clientOfferRepo.NullifyClient(id);
            this.repo.Delete(id);
            this.UserRepo.Delete(userId);
        }

        public IEnumerable<ClientAccountDTO> Accounts(int id)
        {
            Client client = this.FindEntity(id);
            this.repo.Entry(client).Collection("ClientAccount").Load();
            return client.ClientAccount.Select(account => this.Utils.Cast<ClientAccountDTO, ClientAccount>(account));
        }

        public ClientAccount FindAccountEntity(int clientId, int accountId)
        {
            ClientAccount account = this.accountRepo.FindById(accountId);
            if (account == null || account.ClientId != clientId)
            {
                throw new AppException("Account not found", 404);
            }
            return account;
        }

        public ClientAccountDTO FindAccount(int clientId, int accountId)
        {
            return this.Utils.Cast<ClientAccountDTO, ClientAccount>(this.FindAccountEntity(clientId, accountId));
        }

        public ClientAccountDTO UpdateAccount(int clientId, int accountId, int amount)
        {
            ClientAccount account = this.FindAccountEntity(clientId, accountId);
            if (new AccountManager(account.ExternalAccount).Update(amount))
            {
                account.Balance += amount;
                this.accountRepo.Update(account);
            }
            return this.Utils.Cast<ClientAccountDTO, ClientAccount>(account);
        }

        public IEnumerable<TraderDTO> Traders(int id, string filter)
        {
            Client client = this.FindEntity(id);
            this.repo.Entry(client).Collection("CommercialLink").Load();
            foreach (CommercialLink cl in client.CommercialLink)
            {
                this.ClRepo.Entry(cl).Reference("Trader").Load();
            }
            IEnumerable<Trader> traders = client.CommercialLink.Select(cl => cl.Trader);
            IEnumerable<TraderDTO> ret = traders.Select(trader => this.traderService.EntityToDTO(trader));
            if (filter != null)
            {
                ret = this.filtersHandler.Apply(ret, new Tree(filter));
            }
            return ret;
        }

        public CommercialLink FindOrCreateCl(int clientId, int traderId)
        {
            Client client = this.FindEntity(clientId);
            this.repo.Entry(client).Collection("CommercialLink").Load();
            CommercialLink cl = client.CommercialLink
                .Where(cl => cl.TraderId == traderId)
                .FirstOrDefault();
            if (cl == null)
            {
                cl = new CommercialLink()
                {
                    ClientId = clientId,
                    TraderId = traderId,
                    Type = CommercialLink.DEFAULT_TYPE
                };
                this.ClRepo.Save(cl);
            }
            return cl;
        }

        public CommercialLinkDTO MarkTrader(int clientId, int traderId, bool bookMark)
        {
            CommercialLink cl = this.FindOrCreateCl(clientId, traderId);
            cl.Type = Utils.SetBit(cl.Type, CommercialLink.BOOKMARK, bookMark);
            cl = this.ClRepo.Update(cl);
            return this.ClToDTO(cl);
        }
    }
}
