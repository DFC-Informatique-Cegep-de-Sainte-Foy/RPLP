// See https://aka.ms/new-console-template for more information
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RPLP.DAL.SQL;
using RPLP.DAL.SQL.Depots;
using RPLP.SERVICES.Github;
using RPLP.ENTITES.InterfacesDepots;
using System.Timers;
using RPLP.SERVICES.InterfacesDepots;
using Unity;
using Unity.Injection;


IConfigurationRoot configuration;
IUnityContainer container = new UnityContainer();

var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
configuration = builder.Build();

var connectionString = configuration.GetConnectionString("DefaultConnection");
var token = configuration.GetRequiredSection("Token").Value;

DbContextOptionsBuilder<RPLPDbContext> optionsBuilder =
    new DbContextOptionsBuilder<RPLPDbContext>().UseSqlServer(connectionString).UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);

container.RegisterType<RPLPDbContext>(TypeLifetime.Singleton, new InjectionConstructor(new object[]
    { optionsBuilder.Options }));

container.RegisterType<IDepotClassroom, DepotClassroom>(TypeLifetime.Scoped);
container.RegisterType<IDepotOrganisation, DepotOrganisation>(TypeLifetime.Scoped);
container.RegisterType<IDepotRepository, DepotRepository>(TypeLifetime.Scoped);
container.RegisterType<IDepotAllocation, DepotAllocation>(TypeLifetime.Scoped);
container.RegisterType<IDepotStudent, DepotStudent>(TypeLifetime.Scoped);


ScriptGithubRPLP scripts = new ScriptGithubRPLP(
    container.Resolve<IDepotClassroom>(), 
    container.Resolve<IDepotRepository>(), 
    container.Resolve<IDepotOrganisation>(), 
    container.Resolve<IDepotAllocation>(),
    container.Resolve<IDepotStudent>(),
    token.ToString());

while (true)
{
    scripts.EnsureOrganisationRepositoriesAreInDB();
    scripts.ValidateAllRepositoriesHasBranch();
    Console.WriteLine("Interval");
    Thread.Sleep(3600000);
}



