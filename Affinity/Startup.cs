﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Affinity.Models;
using Microsoft.EntityFrameworkCore;
using FirebaseAdmin;
using RestSharp;
using Newtonsoft.Json.Linq;
using Google.Apis.Auth.OAuth2;
using System.Diagnostics;
using System.IO;
using System;
using Microsoft.Extensions.Hosting;

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

            var builder = new PostgreSqlConnectionStringBuilder(dbUrlProperty.Value.ToString())
            {
                Pooling = true,
                TrustServerCertificate = true,
                SslMode = SslMode.Require
            };

            services.AddEntityFrameworkNpgsql()
                    .AddDbContext<AffinityDbcontext>(options => options.UseNpgsql(builder.ConnectionString, providerOptions => providerOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)));

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
