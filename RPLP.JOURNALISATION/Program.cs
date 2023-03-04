// See https://aka.ms/new-console-template for more information

using RPLP.JOURNALISATION;

Thread.Sleep(5000);

ConsommateurJournalisation consommateur = new ConsommateurJournalisation();
consommateur.DeclarerLaQueue();

while (true)
{
    consommateur.Ecouter();
}