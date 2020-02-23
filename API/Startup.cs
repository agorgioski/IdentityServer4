using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using IdentityServer4.AccessTokenValidation;
using System;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace API
{
    public class Startup
    {
        private readonly string LocalHostAllowance = "LocalHostAllowance";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            services.AddSingleton<ICaseRepository, CaseRepository>(
                s => new CaseRepository(Configuration.GetConnectionString("ObukaDB"))
            );
            services.AddSingleton<IUserStore, UserStore>(
                s => new UserStore(Configuration.GetConnectionString("ObukaDB"))
            );

            // services.AddSingleton<IActionDescriptorCollectionProvider, ActionDescriptorCollectionProvider>();

            services.AddCors(options =>
            {
                options.AddPolicy(LocalHostAllowance,
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:5001");
                    });
            });

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
                .AddIdentityServerAuthentication(options =>
                {
                    options.Authority = "http://localhost:5000";

                    options.ApiName = "caseApi";
                    // options.ApiSecret = "the_secret";
                    options.RequireHttpsMetadata = false;
                    
                    options.EnableCaching = true;
                    options.CacheDuration = TimeSpan.FromMinutes(10); // that's the default
                })
                ;
            services.AddAuthorization(options =>
            {
                options.AddPolicy("admin", policyAdmin =>
                {
                    policyAdmin.RequireClaim("role", "admin");
                });
                options.AddPolicy("user", policyUser =>
                {
                    policyUser.RequireClaim("role", "user");
                });
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseAuthentication();

            app.UseHttpsRedirection();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "Default",
                    template: "{controller}/{action}/{id?}"
                );
            });

            app.UseCors(LocalHostAllowance);
        }
    }
}