using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RPLP.DAL.SQL;
using RPLP.DAL.SQL.Depots;
using RPLP.ENTITES.InterfacesDepots;
using RPLP.JOURNALISATION;
using RPLP.SERVICES.Github;
using RPLP.SERVICES.InterfacesDepots;
using RPLP.VALIDATIONS;
using Unity;
using Unity.Injection;

Thread.Sleep(10000);

IConfigurationRoot configuration;
IUnityContainer container = new UnityContainer();

var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

configuration = builder.Build();

//var connectionString = configuration.GetConnectionString("DefaultConnection");

var connectionString = "Server=rplp.db; Database=RPLP; User Id=sa; password=Cad3pend86!";

//RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(connectionString));

//var token = configuration.GetSection("Token");

var token = "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";

DbContextOptionsBuilder<RPLPDbContext> optionsBuilder =
    new DbContextOptionsBuilder<RPLPDbContext>().UseSqlServer(connectionString);

container.RegisterType<RPLPDbContext>(TypeLifetime.Singleton, new InjectionConstructor(new object[]
    { optionsBuilder.Options }));

container.RegisterType<IDepotClassroom, DepotClassroom>(TypeLifetime.Scoped);
container.RegisterType<IDepotOrganisation, DepotOrganisation>(TypeLifetime.Scoped);
container.RegisterType<IDepotRepository, DepotRepository>(TypeLifetime.Scoped);
container.RegisterType<IDepotAllocation, DepotAllocation>(TypeLifetime.Scoped);

ScriptGithubRPLP scripts = new ScriptGithubRPLP(container.Resolve<IDepotClassroom>(), container.Resolve<IDepotRepository>(), container.Resolve<IDepotOrganisation>(), container.Resolve<IDepotAllocation>(), token.ToString());

Consummer consummer = new Consummer(scripts);
consummer.DeclareExchange();

while (true)
{
    Thread.Sleep(100);
    consummer.Listen();
}

