using Microsoft.EntityFrameworkCore;

namespace fidelizPlus_back.LogDomain
{
    public partial class LogContext : DbContext
    {
        public LogContext()
        { }

        public LogContext(DbContextOptions<LogContext> options)
            : base(options)
        { }

        public virtual DbSet<ErrorLog> ErrorLog { get; set; }

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

                entity.Property(e => e.ThrowingTime)
                    .HasColumnName("throwing_time")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
