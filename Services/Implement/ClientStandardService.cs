using System.Collections.Generic;
using System.Linq;

namespace fidelizPlus_back.Services
{
    using DTO;
    using Errors;
    using Models;
    using Repositories;

    public class ClientStandardService : UserStandardService<Client, ClientDTO>, ClientService
    {
        private ClientAccountRepository accountRepo;
        private ClientOfferRepository clientOfferRepo;
        private CrudService<Trader, TraderDTO> traderService;
        private FiltersHandler filtersHandler;

        public ClientStandardService(
            Error error,
            UserEntityRepository<Client> entityRepo,
            Utils utils,
            CrudRepository<User> userRepo,
            CommercialLinkRepository clRepo,
            ClientAccountRepository accountRepo,
            ClientOfferRepository clientOfferRepo,
            CrudService<Trader, TraderDTO> traderService,
            FiltersHandler filtersHandler
        ) : base(error, entityRepo, utils, userRepo, clRepo)
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
            this.Repo.Delete(id);
            this.UserRepo.Delete(userId);
        }

        public IEnumerable<ClientAccountDTO> Accounts(int id, string filter)
        {
            Client client = this.FindEntity(id);
            this.Repo.Entry(client).Collection("ClientAccount").Load();
            IEnumerable<ClientAccountDTO> ret = client.ClientAccount.Select(account => this.Utils.Cast<ClientAccountDTO, ClientAccount>(account));
            if (filter != null)
            {
                ret = this.filtersHandler.Apply(ret, new Tree(filter, this.Error));
            }
            return ret;
        }

        public ClientAccount FindAccountEntity(int clientId, int accountId)
        {
            ClientAccount account = this.accountRepo.FindById(accountId);
            if (account?.ClientId != clientId)
            {
                this.Error.Throw("Account not found", 404);
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
            this.Repo.Entry(client).Collection("CommercialLink").Load();
            foreach (CommercialLink cl in client.CommercialLink)
            {
                this.ClRepo.Entry(cl).Reference("Trader").Load();
            }
            IEnumerable<Trader> traders = client.CommercialLink.Select(cl => cl.Trader);
            IEnumerable<TraderDTO> ret = traders.Select(trader => this.traderService.EntityToDTO(trader));
            if (filter != null)
            {
                ret = this.filtersHandler.Apply(ret, new Tree(filter, this.Error));
            }
            return ret;
        }

        public CommercialLink FindOrCreateCl(int clientId, int traderId)
        {
            Client client = this.FindEntity(clientId);
            this.Repo.Entry(client).Collection("CommercialLink").Load();
            CommercialLink cl = client.CommercialLink
                .Where(cl => cl.TraderId == traderId)
                .FirstOrDefault();
            if (cl == null)
            {
                cl = new CommercialLink()
                {
                    ClientId = clientId,
                    TraderId = traderId,
                    Status = CommercialLink.DEFAULT_TYPE
                };
                this.ClRepo.Save(cl);
            }
            return cl;
        }

        public CommercialLinkDTO MarkTrader(int clientId, int traderId, bool bookMark)
        {
            CommercialLink cl = this.FindOrCreateCl(clientId, traderId);
            cl.Status = Utils.SetBit(cl.Status, CommercialLink.BOOKMARK, bookMark);
            cl = this.ClRepo.Update(cl);
            return this.ClToDTO(cl);
        }
    }
}
