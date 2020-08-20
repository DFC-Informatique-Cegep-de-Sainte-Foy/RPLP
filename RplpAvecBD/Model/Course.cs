
namespace RplpAvecBD.Model
{
    public class Course
    {
        public int id { get; set; } //id de Cours
        public string name { get; set; } //le nome de Cours
        public string period { get; set; } //la period de Cours
                                           // public List<int> assignments { get; set; } //la liste des tous travaux dans Cours
        public bool sendReleasedSubmissionsToBack { get; set; } //Si true, les travaux évalués par graders seront envoyés à la fin de la file d'attente d'évaluation.
        public bool emailNewUsers { get; set; } //Si True, les utilisateurs recevront des e-mails les informant qu'ils ont été ajoutés à la liste de ce cours. Les nouveaux utilisateurs de codePost seront invités à créer un compte.
        public bool anonymousGradingDefault { get; set; } //S'il est défini sur True, les travaux nouvellement créées seront automatiquement mises en mode de notation anonyme.

        public int? idCoursChoisi { get; set; }

        //constructer par defaults
        public Course()
        {
            this.id = 0;
            this.name = "";
            this.period = "";
            // this.assignments = new List<int>();
            this.sendReleasedSubmissionsToBack = true;
            this.emailNewUsers = true;
            this.anonymousGradingDefault = true;
        }

        //constructer par initialisation
        public Course(int p_id, string p_name, string p_period, bool p_sendReleasedSubmissionsToBack, bool p_emailNewUsers, bool p_anonymousGradingDefault)
        {
            this.id = p_id;
            this.name = p_name;
            this.period = p_period;
            this.sendReleasedSubmissionsToBack = p_sendReleasedSubmissionsToBack;
            this.emailNewUsers = p_emailNewUsers;
            this.anonymousGradingDefault = p_anonymousGradingDefault;
        }

        public override string ToString()
        {
            return this.id + "name: "+this.name;
        }
    }
}
