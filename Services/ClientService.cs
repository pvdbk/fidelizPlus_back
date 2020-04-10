namespace fidelizPlus_back.Services
{
    using AppDomain;
    using DTO;
    using Repositories;

    public class ClientService : UserEntityService<Client, ClientDTO, ClientAccount, ClientAccountDTO>
    {
        private ClientOfferService ClientOfferService { get; }

        public ClientService(
            UserEntityRepository<Client, ClientAccount> repo,
            Utils utils,
            FiltersHandler filtersHandler,
            CrudService<User, ClientDTO> userService,
            AccountService<ClientAccount, ClientAccountDTO> accountService,
            CommercialLinkService clService,
            ClientOfferService clientOfferService
        ) : base(repo, utils, filtersHandler, userService, accountService, clService)
        {
            ClientOfferService = clientOfferService;
            NotRequiredForSaving = new string[] { "AdminPassword" };
            NotRequiredForUpdating = new string[] { "AdminPassword" };
        }

        public override void Delete(int id)
        {
            Client client = FindEntity(id);
            SeekReferences(client);
            ClService.NullifyClient(id);
            ClientOfferService.NullifyClient(id);
            Repo.Delete(id);
            UserService.Delete(client.Id);
            AccountService.Delete(client.AccountId);
        }

        public ExtendedClientDTO ExtendDTO(ClientDTO dto, int traderId)
        {
            ExtendedClientDTO ret = null;
            CommercialLink cl = null;
            if (dto.Id != null)
            {
                ret = Utils.Cast<ExtendedClientDTO, ClientDTO>(dto);
                cl = ClService.FindWithBoth((int)dto.Id, traderId);
            }
            if (cl == null)
            {
                throw new AppException("Bad use of ClientStandardService.ExtendDTO");
            }
            ret.RelationStatus = ClService.GetClStatus(cl);
            return ret;
        }
    }
}
