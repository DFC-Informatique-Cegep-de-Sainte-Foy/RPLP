using Microsoft.EntityFrameworkCore;
using RPLP.DAL.SQL;
using RPLP.DAL.SQL.Depots;
using RPLP.JOURNALISATION;
using RPLP.ENTITES.InterfacesDepots;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using RPLP.SERVICES.InterfacesDepots;

try
{
    var builder = WebApplication.CreateBuilder(args);
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    // Add services to the container.

    builder.Services.AddControllers();
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    builder.Services.AddDbContext<RPLPDbContext>(
    options => options.UseSqlServer(connectionString).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

    builder.Services.AddScoped<IDepotClassroom, DepotClassroom>();
    builder.Services.AddScoped<IDepotAdministrator, DepotAdministrator>();
    builder.Services.AddScoped<IDepotAssignment, DepotAssignment>();
    builder.Services.AddScoped<IDepotComment, DepotComment>();
    builder.Services.AddScoped<IDepotOrganisation, DepotOrganisation>();
    builder.Services.AddScoped<IDepotAllocation, DepotAllocation>();
    builder.Services.AddScoped<IDepotRepository, DepotRepository>();
    builder.Services.AddScoped<IDepotStudent, DepotStudent>();
    builder.Services.AddScoped<IDepotTeacher, DepotTeacher>();
    builder.Services.AddScoped<IVerificatorForDepot, VerificatorForDepot>();
    builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
 

    var app = builder.Build();

    // Configure the HTTP request pipeline.
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseAuthentication();
    app.UseAuthorization();

    app.MapControllers();

    app.Run();

}
catch (Exception exception)
{
    RPLP.JOURNALISATION.Logging.Journal(new Log(exception.ToString(), exception.StackTrace.ToString().Replace(System.Environment.NewLine, "."),
             "Projet - RPLP.API - Erreur récupérer dans le try/catch central", 0));
}

