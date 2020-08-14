using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace RplpAvecBD.Model
{
    [Table("Professeur")]
    public class Professeur
    {
        [Key]
        [Required]
        public int id { get; set; }

        [StringLength(45)]
        [Display(Name = "Nom")]
        public string nom { get; set; }

        //[Required]
        [StringLength(45)]
        [Display(Name = "Courriel")]
        public string courriel { get; set; }

        [StringLength(40)]
        [Display(Name = "API Key")]
        [RegularExpression(@"^[a-zA-Z0-9]{40}$", ErrorMessage = "Le code ne respect pas le bon format !")]
        public string apiKey { get; set; }

    }
}
