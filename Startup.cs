using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace fidelizPlus_back
{
    using AppModel;
    using DTO;
    using LogModel;
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
            services.AddSingleton(typeof(Utils), typeof(StandardUtils));
            services.AddSingleton(typeof(FiltersHandler), typeof(StandardFiltersHandler));
            services.AddSingleton(typeof(BankManager), typeof(StandardBankManager));
            services.AddSingleton(typeof(AppContext), typeof(StandardAppContext));
            services.AddSingleton(typeof(LogContext), typeof(StandardLogContext));

            services.AddSingleton(typeof(CrudRepository<User>), typeof(CrudStandardRepository<User>));
            services.AddSingleton(typeof(CrudService<User, ClientDTO>), typeof(CrudStandardService<User, ClientDTO>));
            services.AddSingleton(typeof(CrudService<User, TraderDTO>), typeof(CrudStandardService<User, TraderDTO>));

            services.AddSingleton(typeof(UserEntityRepository<Client>), typeof(UserEntityStandardRepository<Client, ClientAccount>));
            services.AddSingleton(typeof(CrudRepository<ClientAccount>), typeof(CrudStandardRepository<ClientAccount>));
            services.AddSingleton(typeof(ClientService), typeof(ClientStandardService));

            services.AddSingleton(typeof(UserEntityRepository<Trader>), typeof(UserEntityStandardRepository<Trader, TraderAccount>));
            services.AddSingleton(typeof(CrudRepository<TraderAccount>), typeof(CrudStandardRepository<TraderAccount>));
            services.AddSingleton(typeof(TraderService), typeof(TraderStandardService));

            services.AddSingleton(typeof(PurchaseXorCommentRepository<Purchase>), typeof(PurchaseXorCommentStandardRepository<Purchase>));
            services.AddSingleton(typeof(PurchaseXorCommentRepository<Comment>), typeof(PurchaseXorCommentStandardRepository<Comment>));
            services.AddSingleton(typeof(ClientOfferRepository), typeof(ClientOfferStandardRepository));
            services.AddSingleton(typeof(CommercialLinkRepository), typeof(CommercialLinkStandardRepository));
            services.AddSingleton(typeof(OfferRepository), typeof(OfferStandardRepository));

            services.AddSingleton(typeof(CrudService<ClientAccount, ClientAccountDTO>), typeof(AccountStandardService<ClientAccount, ClientAccountDTO>));
            services.AddSingleton(typeof(CrudService<TraderAccount, TraderAccountDTO>), typeof(AccountStandardService<TraderAccount, TraderAccountDTO>));
            services.AddSingleton(typeof(CommercialLinkService), typeof(CommercialLinkStandardService));
            services.AddSingleton(typeof(OfferService), typeof(OfferStandardService));
            services.AddSingleton(typeof(ClientOfferService), typeof(ClientOfferStandardService));
            services.AddSingleton(typeof(ClientAndTraderService), typeof(ClientAndTraderStandardService));

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseExceptionMiddleware();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
