using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace fidelizPlus_back
{
    using AppDomain;
    using DTO;
    using LogDomain;
    using Repositories;
    using Services;

    public class Startup
    {
        public IConfiguration Configuration { get; }
        private string MyAllowSpecificOrigins => "myAllowSpecificOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(
                    name: MyAllowSpecificOrigins,
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:3000");
                    }
                );
            });

            services.AddDbContext<AppContext>(options =>
            {
                options.UseMySql(
                    Configuration.GetConnectionString("AppConnection"),
                    x => x.ServerVersion("10.4.8-mariadb")
                );
            });
            services.AddDbContext<LogContext>(options =>
            {
                options.UseMySql(
                    Configuration.GetConnectionString("LogConnection"),
                    x => x.ServerVersion("10.4.8-mariadb")
                );
            });

            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(1);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddSingleton<BankManager>();
            services.AddSingleton<PaymentMonitor>();
            services.AddScoped<LogService>();

            services.AddScoped<CrudRepository<User>>();
            services.AddScoped<CrudService<User, ClientDTO>>();
            services.AddScoped<CrudService<User, TraderDTO>>();

            services.AddScoped<UserEntityRepository<Client, ClientAccount>>();
            services.AddScoped<CrudRepository<ClientAccount>>();
            services.AddScoped<ClientService>();

            services.AddScoped<UserEntityRepository<Trader, TraderAccount>>();
            services.AddScoped<CrudRepository<TraderAccount>>();
            services.AddScoped<TraderService>();

            services.AddScoped<RelatedToBothRepository<Purchase>>();
            services.AddScoped<RelatedToBothRepository<Comment>>();
            services.AddScoped<ClientOfferRepository>();
            services.AddScoped<CommercialLinkRepository>();
            services.AddScoped<OfferRepository>();

            services.AddScoped<AccountService<ClientAccount, ClientAccountDTO>>();
            services.AddScoped<AccountService<TraderAccount, TraderAccountDTO>>();
            services.AddScoped<CommercialLinkService>();
            services.AddScoped<OfferService>();
            services.AddScoped<ClientOfferService>();
            services.AddScoped<MultiService>();
            services.AddScoped<RelatedToBothService<Purchase, PurchaseDTO>>();

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseExceptionHandler();

            app.UseWebSockets();

            app.UseRouting();

            app.UseCors(MyAllowSpecificOrigins);

            app.UseAuthorization();

            app.UsePaymentHandler();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
