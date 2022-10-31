using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Affinity.Models;
using Microsoft.EntityFrameworkCore;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using System;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace Affinity
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile("affinity-firebase-adminsdk.json")
            });
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddRazorPages();

            var connectionString = Environment.GetEnvironmentVariable("CUSTOMCONNSTR_AffinityDbcontext");
            connectionString ??= Configuration.GetConnectionString("AffinityDbcontext");

            services.AddEntityFrameworkNpgsql()
                .AddDbContext<AffinityDbcontext>(options => options.UseNpgsql(connectionString, providerOptions => providerOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)));

            //Set's the urls to be lowercase easier for the user!
            services.AddRouting(other => other.LowercaseUrls = true);
            services.AddHttpContextAccessor();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Affinity/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseRouting();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Affinity}/{action=Index}/{id?}");

                endpoints.MapControllerRoute(
                    name: "graph",
                    pattern: "{controller=Graph}/{action=GetSpecificGraph}/{token:length(8)}");
            });
        }
    }
}
