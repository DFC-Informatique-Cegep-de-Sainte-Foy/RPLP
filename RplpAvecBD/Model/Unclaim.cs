﻿using System.ComponentModel.DataAnnotations;


namespace RplpAvecBD.Model
{
    public class Unclaim
    {
        
        [RegularExpression(@"^\d{5,}$", ErrorMessage = "Le code ne respect pas le bon format !")]
        [Required(ErrorMessage = "Le champ ne doit pas être vide !")]
        [Display(Name = "Code étudiant :")]
        public string codeEtudiant { get; set; }

        
        public int? idProfesseur { get; set; }

    }
}
