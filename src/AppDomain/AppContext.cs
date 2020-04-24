using Microsoft.EntityFrameworkCore;

namespace fidelizPlus_back.AppDomain
{
    public partial class AppContext : DbContext
    {
        public AppContext() : base()
        { }

        public AppContext(DbContextOptions<AppContext> options) : base(options)
        { }

        public virtual DbSet<Client> Client { get; set; }
        public virtual DbSet<ClientAccount> ClientAccount { get; set; }
        public virtual DbSet<ClientOffer> ClientOffer { get; set; }
        public virtual DbSet<Comment> Comment { get; set; }
        public virtual DbSet<CommercialLink> CommercialLink { get; set; }
        public virtual DbSet<Offer> Offer { get; set; }
        public virtual DbSet<Purchase> Purchase { get; set; }
        public virtual DbSet<Trader> Trader { get; set; }
        public virtual DbSet<TraderAccount> TraderAccount { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>(entity =>
            {
                entity.ToTable("client");

                entity.HasIndex(e => e.ConnectionId)
                    .HasName("connection_id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.UserId)
                    .HasName("fk_client_user1_idx");

                entity.HasIndex(e => e.AccountId)
                    .HasName("fk_client_client_account1_idx");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AdminPassword)
                    .HasColumnName("admin_password")
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.ConnectionId)
                    .IsRequired()
                    .HasColumnName("connection_id")
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.AccountId).HasColumnName("client_account_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Client)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_client_user1");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Client)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_client_client_account1");
            });

            modelBuilder.Entity<ClientAccount>(entity =>
            {
                entity.ToTable("client_account");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Balance)
                    .HasColumnName("balance")
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.ExternalAccount)
                    .IsRequired()
                    .HasColumnName("external_account")
                    .HasColumnType("varchar(500)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");
            });

            modelBuilder.Entity<ClientOffer>(entity =>
            {
                entity.ToTable("client_offer");

                entity.HasIndex(e => e.ClientId)
                    .HasName("fk_client_offer_client1_idx");

                entity.HasIndex(e => e.OfferId)
                    .HasName("fk_client_offer_offer1_idx");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ClientId).HasColumnName("client_id");

                entity.Property(e => e.OfferId).HasColumnName("offer_id");

                entity.Property(e => e.ReceivedCount).HasColumnName("received_count");

                entity.Property(e => e.UsedCount).HasColumnName("used_Count");

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.ClientOffer)
                    .HasForeignKey(d => d.ClientId)
                    .HasConstraintName("fk_client_offer_client1");

                entity.HasOne(d => d.Offer)
                    .WithMany(p => p.ClientOffer)
                    .HasForeignKey(d => d.OfferId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_client_offer_offer1");
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToTable("comment");

                entity.HasIndex(e => e.CommercialLinkId)
                    .HasName("fk_comment_commercial_link1_idx");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CommercialLinkId).HasColumnName("commercial_link_id");

                entity.Property(e => e.CreationTime)
                    .HasColumnName("creation_time")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Rating).HasColumnName("rating");

                entity.Property(e => e.Text)
                    .IsRequired()
                    .HasColumnName("text")
                    .HasColumnType("varchar(500)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.HasOne(d => d.CommercialLink)
                    .WithMany(p => p.Comment)
                    .HasForeignKey(d => d.CommercialLinkId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_comment_commercial_link1");
            });

            modelBuilder.Entity<CommercialLink>(entity =>
            {
                entity.ToTable("commercial_link");

                entity.HasIndex(e => e.ClientId)
                    .HasName("fk_commercial_link_client1_idx");

                entity.HasIndex(e => e.TraderId)
                    .HasName("fk_commercial_link_trader1_idx");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ClientId).HasColumnName("client_id");

                entity.Property(e => e.TraderId).HasColumnName("trader_id");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.HasOne(d => d.Client)
                    .WithMany(p => p.CommercialLink)
                    .HasForeignKey(d => d.ClientId)
                    .HasConstraintName("fk_commercial_link_client1");

                entity.HasOne(d => d.Trader)
                    .WithMany(p => p.CommercialLink)
                    .HasForeignKey(d => d.TraderId)
                    .HasConstraintName("fk_commercial_link_trader1");
            });

            modelBuilder.Entity<Offer>(entity =>
            {
                entity.ToTable("offer");

                entity.HasIndex(e => e.TraderId)
                    .HasName("fk_offer_trader1_idx");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ContentPath)
                    .IsRequired()
                    .HasColumnName("content_path")
                    .HasColumnType("varchar(200)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.EndTime)
                    .HasColumnName("end_time")
                    .HasColumnType("datetime");

                entity.Property(e => e.StartTime)
                    .HasColumnName("start_time")
                    .HasColumnType("datetime");

                entity.Property(e => e.TraderId).HasColumnName("trader_id");

                entity.HasOne(d => d.Trader)
                    .WithMany(p => p.Offer)
                    .HasForeignKey(d => d.TraderId)
                    .HasConstraintName("fk_offer_trader1");
            });

            modelBuilder.Entity<Purchase>(entity =>
            {
                entity.ToTable("purchase");

                entity.HasIndex(e => e.CommercialLinkId)
                    .HasName("fk_purchase_commercial_link1_idx");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Amount)
                    .HasColumnName("amount")
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.CommercialLinkId).HasColumnName("commercial_link_id");

                entity.Property(e => e.PayingTime)
                    .HasColumnName("paying_time")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.CommercialLink)
                    .WithMany(p => p.Purchase)
                    .HasForeignKey(d => d.CommercialLinkId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_purchase_commercial_link1");
            });

            modelBuilder.Entity<Trader>(entity =>
            {
                entity.ToTable("trader");

                entity.HasIndex(e => e.ConnectionId)
                    .HasName("connection_id_UNIQUE")
                    .IsUnique();

                entity.HasIndex(e => e.UserId)
                    .HasName("fk_trader_user1_idx");

                entity.HasIndex(e => e.AccountId)
                    .HasName("fk_trader_trader_account1_idx");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address)
                    .HasColumnName("address")
                    .HasColumnType("varchar(500)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.ConnectionId)
                    .IsRequired()
                    .HasColumnName("connection_id")
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.Label)
                    .IsRequired()
                    .HasColumnName("label")
                    .HasColumnType("varchar(500)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.LogoPath)
                    .HasColumnName("logo_path")
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.Phone)
                    .HasColumnName("phone")
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.Property(e => e.AccountId).HasColumnName("trader_account_id");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Trader)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_trader_user1");

                entity.HasOne(d => d.Account)
                    .WithMany(p => p.Trader)
                    .HasForeignKey(d => d.AccountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_trader_trader_account1");
            });

            modelBuilder.Entity<TraderAccount>(entity =>
            {
                entity.ToTable("trader_account");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Balance)
                    .HasColumnName("balance")
                    .HasColumnType("decimal(10,2)");

                entity.Property(e => e.Gni)
                    .IsRequired()
                    .HasColumnName("gni")
                    .HasColumnType("varchar(100)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CreationTime)
                    .HasColumnName("creation_time")
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasColumnName("email")
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasColumnName("first_name")
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");

                entity.Property(e => e.Surname)
                    .IsRequired()
                    .HasColumnName("surname")
                    .HasColumnType("varchar(45)")
                    .HasCharSet("utf8mb4")
                    .HasCollation("utf8mb4_unicode_ci");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
