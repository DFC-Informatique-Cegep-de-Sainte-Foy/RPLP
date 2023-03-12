// See https://aka.ms/new-console-template for more information

using RPLP.JOURNALISATION;

Thread.Sleep(5000);

ConsummerLogs consummerLogs = new ConsummerLogs();
consummerLogs.DeclareQueue();

while (true)
{
    consummerLogs.Listen();
}