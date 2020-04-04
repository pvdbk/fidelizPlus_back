using System.Collections.Generic;
using System.Linq;

namespace fidelizPlus_back.Services
{
    using Models;
    using Repositories;
    using DTO;

    public class ClientStandardService : UserStandardService<Client, ClientDTO>, ClientService
    {
        private ClientAccountRepository accountRepo;
        private ClientOfferRepository clientOfferRepo;
        private CrudService<Trader, TraderDTO> traderService;

        public ClientStandardService(
            UserEntityRepository<Client> entityRepo,
            CrudRepository<User> userRepo,
            CommercialLinkRepository clRepo,
            ClientAccountRepository accountRepo,
            ClientOfferRepository clientOfferRepo,
            CrudService<Trader, TraderDTO> traderService
        ) : base(entityRepo, userRepo, clRepo)
        {
            this.accountRepo = accountRepo;
            this.clientOfferRepo = clientOfferRepo;
            this.traderService = traderService;
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
            return client.ClientAccount.Select(account => Utils.Cast<ClientAccountDTO, ClientAccount>(account));
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
            return Utils.Cast<ClientAccountDTO, ClientAccount>(this.FindAccountEntity(clientId, accountId));
        }

        public ClientAccountDTO UpdateAccount(int clientId, int accountId, int amount)
        {
            ClientAccount account = this.FindAccountEntity(clientId, accountId);
            if (new AccountManager(account.ExternalAccount).Update(amount))
            {
                account.Balance += amount;
                this.accountRepo.Update(account);
            }
            return Utils.Cast<ClientAccountDTO, ClientAccount>(account);
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
                ret = FiltersHandler.Apply(ret, new Tree(filter));
            }
            return ret;
        }

        public CommercialLink FindCommercialLink(int clientId, int traderId)
        {
            Client client = this.FindEntity(clientId);
            this.repo.Entry(client).Collection("CommercialLink").Load();
            return client.CommercialLink.Where(cl => cl.TraderId == traderId).FirstOrDefault();
        }

        public CommercialLinkDTO MarkTrader(int clientId, int traderId, bool bookMark)
        {
            CommercialLink newCl = new CommercialLink()
            {
                ClientId = clientId,
                TraderId = traderId
            };
            CommercialLink oldCl = FindCommercialLink(clientId, traderId);
            if (oldCl == null)
            {
                this.traderService.FindEntity(traderId);
                newCl.Type = Utils.SetBit(CommercialLink.DEFAULT_TYPE, CommercialLink.BOOKMARK, bookMark);
                this.ClRepo.Save(newCl);
            }
            else
            {
                newCl.Id = oldCl.Id;
                newCl.Type = Utils.SetBit(oldCl.Type, CommercialLink.BOOKMARK, bookMark);
                newCl = this.ClRepo.Update(newCl);
            }
            return newCl.ToDTO();
        }
    }
}
