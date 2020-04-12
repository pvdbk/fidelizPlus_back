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
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
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

            services.AddSingleton<Utils>();
            services.AddSingleton<FiltersHandler>();
            services.AddSingleton<BankManager>();
            services.AddSingleton<PaymentMonitor>();
            services.AddSingleton<LogService>();

            services.AddTransient<CrudRepository<User>>();
            services.AddSingleton<CrudService<User, ClientDTO>>();
            services.AddSingleton<CrudService<User, TraderDTO>>();

            services.AddTransient<UserEntityRepository<Client, ClientAccount>>();
            services.AddTransient<CrudRepository<ClientAccount>>();
            services.AddSingleton<ClientService>();

            services.AddTransient<UserEntityRepository<Trader, TraderAccount>>();
            services.AddTransient<CrudRepository<TraderAccount>>();
            services.AddSingleton<TraderService>();

            services.AddTransient<RelatedToBothRepository<Purchase>>();
            services.AddTransient<RelatedToBothRepository<Comment>>();
            services.AddTransient<ClientOfferRepository>();
            services.AddTransient<CommercialLinkRepository>();
            services.AddTransient<OfferRepository>();

            services.AddSingleton<AccountService<ClientAccount, ClientAccountDTO>>();
            services.AddSingleton<AccountService<TraderAccount, TraderAccountDTO>>();
            services.AddSingleton<CommercialLinkService>();
            services.AddSingleton<OfferService>();
            services.AddSingleton<ClientOfferService>();
            services.AddSingleton<MultiService>();
            services.AddSingleton<RelatedToBothService<Purchase, PurchaseDTO>>();

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseExceptionHandler();

            app.UseWebSockets();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UsePaymentHandler();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
