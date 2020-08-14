using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RplpAvecBD.Model
{
    public class Unclaim
    {
        
        [RegularExpression(@"^\d{5,}$", ErrorMessage = "Le code ne respect pas le bon format !")]
        [Required(ErrorMessage = "Le champ ne doit pas être vide !")]
        [Display(Name = "Code étudiant :")]
        public string codeEtudiant { get; set; }

        [Required(ErrorMessage = "Il faut sélectionner un professeur !")]
        public int idProfesseur { get; set; }

    }
}
