// See https://aka.ms/new-console-template for more information
using RPLP.DAL.SQL.Depots;
using RPLP.SERVICES.Github;
using System.Timers;

string token = "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";
ScriptGithubRPLP scripts = new ScriptGithubRPLP(new DepotClassroom(), new DepotRepository(), new DepotOrganisation(), token);

scripts.EnsureOrganisationRepositoriesAreInDB();

//System.Timers.Timer timer = new System.Timers.Timer();
//timer.Elapsed += new ElapsedEventHandler(RunScriptEvent);
//timer.Interval = 1000;
//timer.AutoReset = true;
//timer.Enabled = true;

//while(true)
//{
//    Console.ReadLine();
//}

// // Specify what you want to happen when the Elapsed event is raised.
//void RunScriptEvent(object source, ElapsedEventArgs e)
//{
//    scripts.EnsureOrganisationRepositoriesAreInDB();
//    //Console.WriteLine("Interval");
//    timer.Interval = 500000;
//}



