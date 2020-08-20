using System.ComponentModel.DataAnnotations;

namespace RplpAvecBD.Model
{
    public class Assignment
    {
        //les proprietes
        //public int id { get; set; } //id d'assignment
        [Required(ErrorMessage = "Le nom ne doit pas être vide !")]
        [Display(Name = "Nom du travail")]
        public string name { get; set; } // le nom d'assignment (travail)

        [Required(ErrorMessage = "Obligatoire !")]
        [Display(Name = "Points")]
        public int? points { get; set; } //le number de points pour assignment(travail) 
        public int course { get; set; } //l'id de Cours dans lequel cet assignement est create
        public bool isReleased { get; set; } // if True est publie pour faire de revu
        public bool isVisible { get; set; } //If True, visible pour les etudiants
        public bool liveFeedbackMode { get; set; } // Si True, les étudiants pourront voir leur contenu et leurs commentaires avant qu'il ne soit terminé ou publié.
        public bool hideGrades { get; set; }//Si True, les étudiants ne pourront pas voir leurs notes pour ce devoir.


        //constructer par initialisation
        public Assignment(string p_name, int p_points, int p_course, bool p_isReleased, bool p_isVisible, bool p_liveFeedbackMode, bool p_hideGrades)
        {
            //this.id = p_id;
            this.name = p_name;
            this.points = p_points;
            this.course = p_course;
            this.isReleased = p_isReleased;
            this.isVisible = p_isVisible;
            this.liveFeedbackMode = p_liveFeedbackMode;
            this.hideGrades = p_hideGrades;
        }

        public Assignment()
        {

        }
    }
}
