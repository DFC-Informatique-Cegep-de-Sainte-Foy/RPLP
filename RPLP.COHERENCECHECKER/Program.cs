﻿// See https://aka.ms/new-console-template for more information
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RPLP.DAL.SQL;
using RPLP.DAL.SQL.Depots;
using RPLP.SERVICES.Github;
using RPLP.SERVICES.InterfacesDepots;
using System.Data;
using System.Timers;
using Unity;
using Unity.Builder;
using Unity.Injection;

// à supprimer après test
//IUnityContainer container = new UnityContainer().AddExtension(new Diagnostic());
//IConfiguration configuration = container.Resolve<IConfiguration>(); 

IConfigurationRoot configuration;
IUnityContainer container = new UnityContainer();

var builder = new ConfigurationBuilder()
        .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
configuration = builder.Build();

var connectionString = configuration.GetConnectionString("DefaultConnection");
var token = configuration.GetSection("Token");

DbContextOptionsBuilder <RPLPDbContext> optionsBuilder =
    new DbContextOptionsBuilder<RPLPDbContext>().UseSqlServer(connectionString);

container.RegisterType<RPLPDbContext>(TypeLifetime.Singleton, new InjectionConstructor(new object[]
    { optionsBuilder.Options }));

container.RegisterType<IDepotClassroom, DepotClassroom>(TypeLifetime.Scoped);
container.RegisterType<IDepotOrganisation, DepotOrganisation>(TypeLifetime.Scoped);
container.RegisterType<IDepotRepository, DepotRepository>(TypeLifetime.Scoped);


ScriptGithubRPLP scripts = new ScriptGithubRPLP(container.Resolve<IDepotClassroom>(), container.Resolve<IDepotRepository>(), container.Resolve<IDepotOrganisation>(), token.ToString());

while(true)
{
    scripts.EnsureOrganisationRepositoriesAreInDB();
    Console.WriteLine("Interval");
    Thread.Sleep(3600000);
}



