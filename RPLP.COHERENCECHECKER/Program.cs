﻿// See https://aka.ms/new-console-template for more information
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



