using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RplpAvecBD.Data;
using Microsoft.EntityFrameworkCore.SqlServer;


namespace RplpAvecBD
{
    public class Startup
    {
        public static bool estProfesseur = false;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthentication(AzureADDefaults.AuthenticationScheme)
                .AddAzureAD(options => Configuration.Bind("AzureAd", options));

            //services.AddAuthorization(option =>
            //{
            //    option.AddPolicy("estProfesseur", p =>
            //    {
            //        p.RequireClaim("groups", "955bcf43-9a4b-40de-9ddd-72aba52d3669");
            //        estProfesseur = true;
            //    });

            //    option.AddPolicy("estEtudiant", p =>
            //    {
            //        p.RequireClaim("groups", "76d2a1f1-fa8a-4a15-8ada-2724d74ad571");
            //        estProfesseur = false;
            //    });
            //});

            services.AddDbContext<RplpContext>
            (o => o.UseSqlServer(Configuration.GetConnectionString("RplpDb")));

            services.AddControllersWithViews(options =>
            {
                var policy = new AuthorizationPolicyBuilder()
                    .RequireAuthenticatedUser()
                    .Build();
                options.Filters.Add(new AuthorizeFilter(policy));
            });
            services.AddRazorPages();
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRewriter(
            new RewriteOptions().Add(
                context => {
                    if (context.HttpContext.Request.Path == "/AzureAD/Account/SignedOut")
                    { context.HttpContext.Response.Redirect("/Home"); }
                })
             );

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });
        }
    }
}
