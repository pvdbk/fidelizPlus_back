using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace fidelizPlus_back
{
    using AppDomain;
    using LogDomain;
    using Repositories;
    using Services;
    using Payment;
    using Identification;

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
                        builder
                            .WithOrigins("http://localhost:3000")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
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
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.Name = "sessionId";
            });

            services.AddSingleton<Monitor>();
			services.AddScoped<Credentials>();
            services.AddScoped<CredentialsHandler>();
            services.AddScoped<LogService>();
			services.AddScoped<LogoService>();

			services.AddScoped<CrudRepository<User>>();
            services.AddScoped<CrudService<User, PrivateClient>>();
            services.AddScoped<CrudService<User, PrivateTrader>>();

            services.AddScoped<UserEntityRepository<Client, ClientAccount>>();
            services.AddScoped<ClientAccountRepository>();
            services.AddScoped<ClientService>();

            services.AddScoped<UserEntityRepository<Trader, TraderAccount>>();
            services.AddScoped<TraderAccountRepository>();
            services.AddScoped<TraderService>();

            services.AddScoped<PurchaseRepository>();
            services.AddScoped<RelatedToBothRepository<Comment>>();
            services.AddScoped<ClientOfferRepository>();
            services.AddScoped<CommercialLinkRepository>();
            services.AddScoped<OfferRepository>();

            services.AddScoped<ClientAccountService>();
            services.AddScoped<TraderAccountService>();
            services.AddScoped<CommercialLinkService>();
            services.AddScoped<OfferService>();
            services.AddScoped<ClientOfferService>();
            services.AddScoped<MultiService>();
            services.AddScoped<PurchaseService>();

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseExceptionHandler();

            app.UseWebSockets();

            app.UseRouting();

            app.UseCors(MyAllowSpecificOrigins);

            app.UseSession();

            app.UsePaymentHandler();

            app.UseSessionHandler();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
