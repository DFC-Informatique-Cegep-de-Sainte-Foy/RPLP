using System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureAD.UI;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RplpAvecBD.Data;
using Microsoft.AspNetCore.Http;

namespace RplpAvecBD
{
    public class Startup
    {
        public static bool estProfesseurConnecte = false;

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

            services.AddAuthorization(option =>
            {
                option.AddPolicy("estProfesseur", p =>
                {
                    // c0d32534-918b-44bd-a2c9-b21e292e6cf7 - InformatiqueDFC
                    // 955bcf43-9a4b-40de-9ddd-72aba52d3669 - o365 - staff
                    // a95951b6-21c0-49ad-8b5d-2bc3d8d61a1d - prof-inf-dfc
                    // 20d9f47c-74dc-4bd1-a214-3a80f2a66bd1 - Prof informatique
                    p.RequireClaim("groups", "d799decc-9064-425a-8db9-c68931b5a469");
                    estProfesseurConnecte = true;
                });

                option.AddPolicy("estEtudiant", p =>
                {
                    p.RequireClaim("groups", "76d2a1f1-fa8a-4a15-8ada-2724d74ad571");
                    estProfesseurConnecte = true;
                });
            });


            // Ajouter RplpContext
            services.AddDbContext<RplpContext>
            (o => o.UseSqlServer(Configuration.GetConnectionString("RplpDb")));


            // Ajouter session
            services.AddSession(options =>
            {
                options.Cookie.IsEssential = true; // make the session cookie Essential
            });

            services.AddDistributedMemoryCache();

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });



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

            // Utiliser session
            app.UseSession();

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
