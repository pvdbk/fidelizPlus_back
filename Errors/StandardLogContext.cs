using Microsoft.EntityFrameworkCore;

namespace fidelizPlus_back.Errors
{
    public partial class StandardLogContext : DbContext, LogContext
    {
        public StandardLogContext()
        {
        }

        public StandardLogContext(DbContextOptions<StandardLogContext> options)
            : base(options)
        {
        }

        public virtual DbSet<ErrorLog> ErrorLog { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseMySql("server=localhost;port=2306;user=root;password=root;database=log", x => x.ServerVersion("10.4.8-mariadb"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ErrorLog>(entity =>
            {
                entity.ToTable("error_log");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Content)
                    .IsRequired()
                    .HasColumnName("content")
                    .HasColumnType("varchar(500)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
