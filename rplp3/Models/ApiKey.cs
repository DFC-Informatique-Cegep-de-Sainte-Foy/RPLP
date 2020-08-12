using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace rplp3.Models
{
    public class ApiKey
    {
        [DisplayName("clef prive")]
        [Required(ErrorMessage = "Obligatoire")]
        [RegularExpression("[a-z0-9]{40}",ErrorMessage ="ne respecte pas le format attendu (ne doit comprendre que 0-9 ou a-z minuscule")]
        public string PrivateKey {get;set;}
        
    }
}
