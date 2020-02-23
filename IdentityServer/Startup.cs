using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

using IdentityServer4;
using IdentityServer4.Stores;
using Microsoft.Extensions.Configuration;

namespace IdentityServer
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddProfileService<UserProfileService>()
                .AddInMemoryClients(ConfigureIdentityServer.GetClients())
                .AddInMemoryIdentityResources(ConfigureIdentityServer.GetIdentityResources())
                .AddInMemoryApiResources(ConfigureIdentityServer.GetApis())
                ;

            services.AddSingleton<IUserStore, UserStore>(
                s => new UserStore(Configuration.GetConnectionString("ObukaDB"))
            );

            services.AddTransient<IPersistedGrantStore, PersistedGrantStore>(
                s => new PersistedGrantStore(Configuration.GetConnectionString("ObukaDB"))
            );

            services.AddAuthentication()
                .AddGoogle("Google", options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.ClientId = "434483408261-55tc8n0cs4ff1fe21ea8df2o443v2iuc.apps.googleusercontent.com";
                    options.ClientSecret = "3gcoTrEDPPJ0ukn_aYYT6PWo";
                })
                ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseIdentityServer(); // includes a call to UseAuthentication

            app.UseStaticFiles();
            app.UseMvcWithDefaultRoute();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
