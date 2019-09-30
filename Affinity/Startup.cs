﻿using System;
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

namespace Affinity
{
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
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            //Set's the urls to be lowercase easier for the user!
            services.AddRouting(other => other.LowercaseUrls = true);

            services.AddAuthentication().AddJwtBearer(options =>
            {
                options.Authority = "https://securetoken.google.com/my-firebase-project";
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = "https://securetoken.google.com/my-firebase-project",
                    ValidateAudience = true,
                    ValidAudience = "my-firebase-project",
                    ValidateLifetime = true
                };
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
                // A header that gives an added layer of security that helps to detect and mitigate certain types of attacks, including XSS and data injection attacks
                // These atacks are used for everything from data theft to site defacement to distribution of malware. 
                // context.Response.Headers.Add("Content-Security-Policy", "default-src 'self'; child-src 'none'; frame-ancestors 'none'; frame-src 'none';");

                //Need to remove these in the IIS or IIS Express config file manually since this wont remove it.
                //I dont have access to modify content in the C: drive so this will be a low vulnerability when scanned (Dynamic scans only)
                context.Response.Headers.Remove("Server");
                context.Response.Headers.Remove("X-Powered-By");

                await next();
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
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
