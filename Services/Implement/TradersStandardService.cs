using System.Reflection;

namespace fidelizPlus_back.Services
{
    using DTO;
    using Models;
    using Repositories;

    public class TradersStandardService : UsersStandardService<Trader, TraderDTO>, Service<TraderDTO>
    {
        private OfferRepository offerRepo;

        public TradersStandardService(
            Repository<User> userRepo,
            UserEntityRepository<Trader> entityRepo,
            CommercialLinkRepository commercialLinkRepo,
            OfferRepository offerRepo
        ) : base(userRepo, entityRepo, commercialLinkRepo)
        {
            this.offerRepo = offerRepo;
        }

        public override bool IsRequiredProp(PropertyInfo prop)
        {
            return prop.Name != "Id" && prop.Name != "Phone" && prop.Name != "LogoPath";
        }

        public override TraderDTO ToDTO(Trader trader)
        {
            if (trader.User == null)
            {
                this.entityRepo.FillUserProp(trader);
            }
            return new TraderDTO()
            {
                Id = trader.Id,
                Surname = trader.User.Surname,
                FirstName = trader.User.FirstName,
                Email = trader.User.Email,
                Password = trader.User.Password,
                ConnectionId = trader.ConnectionId,
                Label = trader.Label,
                Address = trader.Address,
                Phone = trader.Phone,
                LogoPath = trader.LogoPath
            };
        }

        public override Trader DTOToEntity(int? id, int userId, TraderDTO dto)
        {
            Trader trader = new Trader()
            {
                UserId = userId,
                ConnectionId = dto.ConnectionId,
                Label = dto.Label,
                Address = dto.Address,
                Phone = dto.Phone,
                LogoPath = dto.LogoPath
            };
            if (id != null)
            {
                trader.Id = (int)id;
            }
            return trader;
        }

        public override void Delete(int id)
        {
            int userId = this.Find(id).UserId;
            this.commercialLinkRepo.NullifyTrader(id);
            this.offerRepo.NullifyTrader(id);
            this.entityRepo.Delete(id);
            this.userRepo.Delete(userId);
        }
    }
}
