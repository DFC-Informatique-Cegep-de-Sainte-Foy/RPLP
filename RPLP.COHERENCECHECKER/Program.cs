// See https://aka.ms/new-console-template for more information
using RPLP.DAL.SQL.Depots;
using RPLP.SERVICES.Github;
using System.Timers;

string token = "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";
ScriptGithubRPLP scripts = new ScriptGithubRPLP(new DepotClassroom(), new DepotRepository(), new DepotOrganisation(), token);

while(true)
{
    scripts.EnsureOrganisationRepositoriesAreInDB();
    Console.WriteLine("Interval");
    Thread.Sleep(3600000);
}



