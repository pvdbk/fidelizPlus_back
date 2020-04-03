using System.Reflection;

namespace fidelizPlus_back.Services
{
    using DTO;
    using Models;
    using Repositories;

    public class ClientsStandardService : UsersStandardService<Client, ClientDTO>, Service<ClientDTO>
    {
        private AccountRepository accountRepo;
        private ClientOfferRepository clientOfferRepo;

        public ClientsStandardService(
            Repository<User> userRepo,
            UserEntityRepository<Client> entityRepo,
            CommercialLinkRepository commercialLinkRepo,
            AccountRepository accountRepo,
            ClientOfferRepository clientOfferRepo
        ) : base(userRepo, entityRepo, commercialLinkRepo)
        {
            this.accountRepo = accountRepo;
            this.clientOfferRepo = clientOfferRepo;
        }

        public override bool IsRequiredProp(string prop)
        {
            return prop != "Id" && prop != "AdminPassword";
        }

        public override ClientDTO ToDTO(Client client)
        {
            if (client.User == null)
            {
                this.entityRepo.FillUserProp(client);
            }
            return new ClientDTO()
            {
                Id = client.Id,
                Surname = client.User.Surname,
                FirstName = client.User.FirstName,
                Email = client.User.Email,
                Password = client.User.Password,
                ConnectionId = client.ConnectionId,
                AdminPassword = client.AdminPassword
            };
        }

        public override Client DTOToEntity(int? id, int userId, ClientDTO dto)
        {
            Client client = new Client()
            {
                UserId = userId,
                ConnectionId = dto.ConnectionId,
                AdminPassword = dto.AdminPassword
            };
            if (id != null)
            {
                client.Id = (int)id;
            }
            return client;
        }

        public override void Delete(int id)
        {
            int userId = this.Find(id).UserId;
            this.accountRepo.DeleteClient(id);
            this.commercialLinkRepo.NullifyClient(id);
            this.clientOfferRepo.NullifyClient(id);
            this.entityRepo.Delete(id);
            this.userRepo.Delete(userId);
        }
    }
}
