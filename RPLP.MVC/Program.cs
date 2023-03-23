using Auth0.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using RPLP.DAL.SQL;
using RPLP.DAL.SQL.Depots;
using RPLP.JOURNALISATION;
using RPLP.SERVICES.InterfacesDepots;
using System.Diagnostics;
using System;
using System.Net;

try
{
    var builder = WebApplication.CreateBuilder(args);
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

    // Add services to the container.
    builder.Services.AddControllersWithViews();
    builder.Services.AddHttpsRedirection(options =>
    {
        options.RedirectStatusCode = (int)HttpStatusCode.TemporaryRedirect;
        options.HttpsPort = 443;
    });
    builder.Services.AddAuth0WebAppAuthentication(options =>
    {
        options.Domain = builder.Configuration["Auth0:Domain"];
        options.ClientId = builder.Configuration["Auth0:ClientId"];
        options.Scope = "openid profile email";
    });
    builder.Services.AddDbContext<RPLPDbContext>(
    options => options.UseSqlServer(connectionString));

    builder.Services.AddScoped<IDepotClassroom, DepotClassroom>();
    builder.Services.AddScoped<IDepotAdministrator, DepotAdministrator>();
    builder.Services.AddScoped<IDepotAssignment, DepotAssignment>();
    builder.Services.AddScoped<IDepotComment, DepotComment>();
    builder.Services.AddScoped<IDepotOrganisation, DepotOrganisation>();
    builder.Services.AddScoped<IDepotRepository, DepotRepository>();
    builder.Services.AddScoped<IDepotStudent, DepotStudent>();
    builder.Services.AddScoped<IDepotTeacher, DepotTeacher>();
    builder.Services.AddScoped<IVerificatorForDepot, VerificatorForDepot>();
    builder.Services.AddSingleton<IConfiguration>(builder.Configuration);

    

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
    }

    app.UseStaticFiles();

    app.UseRouting();

    app.UseHttpsRedirection();
    app.UseHsts();

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
    app.Run();

}
catch (Exception exception)
{
    RPLP.JOURNALISATION.Logging.Journal(new Log(exception.ToString(), exception.StackTrace.ToString().Replace(System.Environment.NewLine, "."),
         "Projet - RPLP.MVC - Erreur récupérer dans le try/catch central", 0));
}

