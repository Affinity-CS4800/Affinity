using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Affinity.Models;
using Microsoft.EntityFrameworkCore;
using FirebaseAdmin;
using RestSharp;
using Newtonsoft.Json.Linq;
using Google.Apis.Auth.OAuth2;
using System.Diagnostics;
using System.IO;

namespace Affinity
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

            //FirebaseApp.Create(new AppOptions()
            //{
            //    Credential = GoogleCredential.FromFile("Json file"),
            //});
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

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            string API_KEY = "";
            using (StreamReader reader = new StreamReader("API_KEY.txt"))
            {
                API_KEY = reader.ReadLine();
            }

            RestClient client = new RestClient("https://api.heroku.com/");
            RestRequest req = new RestRequest("apps/affinity-cpp/config-vars");
            req.AddHeader("Accept", "application/vnd.heroku+json; version=3");
            req.AddHeader("Authorization", "Bearer " + API_KEY);
            string response = client.Execute(req).Content.ToLower();

            JObject config = JObject.Parse(response);
            JProperty dbUrlProperty = config.Property("database_url");

            Debug.WriteLine(dbUrlProperty.Value.ToString());

            //services.AddDbContext<AffinityDbcontext>(options =>
            //options.UseSqlServer(Configuration.GetConnectionString("AffinityDbcontext")));

            //Set's the urls to be lowercase easier for the user!
            services.AddRouting(other => other.LowercaseUrls = true);
            services.AddHttpContextAccessor();
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
                app.UseExceptionHandler("/Affinity/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            // This adds the appropriate headers to the http responses
            app.Use(async (context, next) =>
            {
                // Protect against XSS (Cross site scripting). The header is designed to enable the filter built into modern web browsers. This is usually enabled
                // by default but using it will enforce it. 
                context.Response.Headers.Add("X-Xss-Protection", "1; mode=block");
                // This response indicates whether or not a browser should be allowed to render a page in a <frame>, <iframe>, or <object>. Sites can use this to 
                // avoid clickjacking attacks, by ensuring that their content is not embedded into other sites. 
                context.Response.Headers.Add("X-Frame-Options", "DENY");
                // A header that is used by the server to indicate that the MIME types in Content-Type headers should not be changed and be followed.
                context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
                // This header lets a webs site tell browsers that it should only be accessed using HTTPS, instead of using HTTP.
                // max-age: The time in seconds that the browser should remmeber that a site is only to be accessed using HTTPS
                context.Response.Headers.Add("Strict-Transport-Security", "max-age=31536000");
                


                await next();
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Affinity}/{action=Index}/{id?}");
            });
        }
    }
}
