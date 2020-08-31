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
using System;

namespace RplpAvecBD
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
            services.AddAuthentication(AzureADDefaults.AuthenticationScheme)
                .AddAzureAD(options => Configuration.Bind("AzureAd", options));

            services.AddAuthorization(options =>
            {
                options.AddPolicy("estProfesseur", policy => policy.RequireRole("teacher"));
                options.AddPolicy("estEtudiant", policy => policy.RequireRole("student"));
            });

            //services.AddAuthorization(option =>
            //{
            //    option.AddPolicy("estProfesseur", p =>
            //    {
            //        options.AddPolicy("AdminAccess", policy => policy.RequireRole("teacher"));
            //        //p.RequireClaim("groups", "c0d32534-918b-44bd-a2c9-b21e292e6cf7"); // informatiqueDFC
            //        //p.RequireClaim("groups", "cfe10d5a-476c-431e-9515-268ac5f7c6b5"); // temp test teacher
            //        //p.RequireClaim("groups", "d799decc-9064-425a-8db9-c68931b5a469"); // Program4business
            //        // informatiqueDFC c0d32534-918b-44bd-a2c9-b21e292e6cf7
            //    });

            //    option.AddPolicy("estEtudiant", p =>
            //    {
            //        options.AddPolicy("AdminAccess", policy => policy.RequireRole("student"));
            //        //p.RequireClaim("groups", "76d2a1f1-fa8a-4a15-8ada-2724d74ad571"); // o365 - étudiant
            //        //p.RequireClaim("groups", "6bca3e76-f76b-4b7b-885c-014234144bfa"); // temp test etudiant
            //    });
            //});

            // Ajouter RplpContext
            services.AddDbContext<RplpContext>
            (o => o.UseSqlServer(Configuration.GetConnectionString("RplpDb")));

            // Ajouter session
            services.AddSession(options =>
            {
                options.Cookie.IsEssential = true; 
                options.IdleTimeout = TimeSpan.FromMinutes(30);
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
