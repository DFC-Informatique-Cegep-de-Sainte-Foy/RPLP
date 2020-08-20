using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RplpAvecBD.Model
{
    public class Assignment
    {
        //les proprietes
        //public int id { get; set; } //id d'assignment
        [Required(ErrorMessage = "Obligatoire !")]
        [Display(Name = "Nom du travail")]
        public string name { get; set; } // le nom d'assignment (travail)

        [Required(ErrorMessage = "Obligatoire !")]
        [Display(Name = "Points")]
        public int? points { get; set; } //le number de points pour assignment(travail) 
        public int course { get; set; } //l'id de Cours dans lequel cet assignement est create
        public bool isReleased { get; set; } // if True est publie pour faire de revu
        public bool isVisible { get; set; } //If True, visible pour les etudiants

        //constructer par initialisation
        public Assignment(string p_name, int p_points, int p_course, bool p_isReleased, bool p_isVisible)
        {
            //this.id = p_id;
            this.name = p_name;
            this.points = p_points;
            this.course = p_course;
            this.isReleased = p_isReleased;
            this.isVisible = p_isVisible;
        }

        public Assignment()
        {

        }
    }
}
