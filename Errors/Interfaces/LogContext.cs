using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace fidelizPlus_back.Errors
{
    public interface LogContext
    {
        public DbSet<ErrorLog> ErrorLog { get; set; }

        public DbSet<T> Set<T>() where T : class;

        public int SaveChanges();

        public EntityEntry Entry(object entity);
    }
}
