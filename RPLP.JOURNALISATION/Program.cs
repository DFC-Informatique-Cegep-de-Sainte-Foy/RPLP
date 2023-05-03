// See https://aka.ms/new-console-template for more information

using RPLP.JOURNALISATION;

ConsummerLogs consummerLogs = new ConsummerLogs();
consummerLogs.DeclareQueue();

while (true)
{
    consummerLogs.Listen();
}