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

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(Context), typeof(AppContext));
            services.AddSingleton(typeof(Repository<User>), typeof(StandardRepository<User>));
            services.AddSingleton(typeof(UserEntityRepository<Client>), typeof(UserEntityStandardRepository<Client>));
            services.AddSingleton(typeof(UserEntityRepository<Trader>), typeof(UserEntityStandardRepository<Trader>));
            services.AddSingleton(typeof(AccountRepository), typeof(AccountStandardRepository));
            services.AddSingleton(typeof(ClientOfferRepository), typeof(ClientOfferStandardRepository));
            services.AddSingleton(typeof(CommercialLinkRepository), typeof(CommercialLinkStandardRepository));
            services.AddSingleton(typeof(OfferRepository), typeof(OfferStandardRepository));
            services.AddSingleton(typeof(Service<ClientDTO>), typeof(ClientsStandardService));
            services.AddSingleton(typeof(Service<TraderDTO>), typeof(TradersStandardService));
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
