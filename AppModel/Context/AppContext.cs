using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace fidelizPlus_back.AppModel
{
    public interface AppContext
    {
        public DbSet<ClientAccount> ClientAccount { get; set; }
        public DbSet<TraderAccount> TraderAccount { get; set; }
        public DbSet<Client> Client { get; set; }
        public DbSet<ClientOffer> ClientOffer { get; set; }
        public DbSet<Comment> Comment { get; set; }
        public DbSet<CommercialLink> CommercialLink { get; set; }
        public DbSet<Offer> Offer { get; set; }
        public DbSet<Trader> Trader { get; set; }
        public DbSet<User> User { get; set; }

        public DbSet<T> Set<T>() where T : class;

        public int SaveChanges();

        public EntityEntry Entry(object entity);
    }
}
