using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace fidelizPlus_back
{
    using DTO;
    using Models;
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
            services.AddSingleton(typeof(Context), typeof(AppContext));
            services.AddSingleton(typeof(CrudRepository<User>), typeof(CrudStandardRepository<User>));
            services.AddSingleton(typeof(UserEntityRepository<Client>), typeof(UserEntityStandardRepository<Client>));
            services.AddSingleton(typeof(UserEntityRepository<Trader>), typeof(UserEntityStandardRepository<Trader>));
            services.AddSingleton(typeof(PurchaseCommentRepository<Purchase>), typeof(PurchaseCommentStandardRepository<Purchase>));
            services.AddSingleton(typeof(PurchaseCommentRepository<Comment>), typeof(PurchaseCommentStandardRepository<Comment>));
            services.AddSingleton(typeof(ClientAccountRepository), typeof(ClientAccountStandardRepository));
            services.AddSingleton(typeof(TraderAccountRepository), typeof(TraderAccountStandardRepository));
            services.AddSingleton(typeof(ClientOfferRepository), typeof(ClientOfferStandardRepository));
            services.AddSingleton(typeof(CommercialLinkRepository), typeof(CommercialLinkStandardRepository));
            services.AddSingleton(typeof(OfferRepository), typeof(OfferStandardRepository));
            services.AddSingleton(typeof(ClientService), typeof(ClientStandardService));
            services.AddSingleton(typeof(TraderService), typeof(TraderStandardService));
            services.AddSingleton(typeof(CrudService<Client, ClientDTO>), typeof(UserStandardService<Client, ClientDTO>));
            services.AddSingleton(typeof(CrudService<Trader, TraderDTO>), typeof(UserStandardService<Trader, TraderDTO>));
            services.AddSingleton(typeof(FiltersHandler), typeof(StandardFiltersHandler));
            services.AddSingleton(typeof(Utils), typeof(StandardUtils));
            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
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
