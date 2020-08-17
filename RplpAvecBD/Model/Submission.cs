using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RplpAvecBD.Model
{
    public class Submission
    {
        //proprietes
        public string assignment { get; set; }
        public List<string> students { get; set; }
        //public bool isFinalized { get; set; }


        //constructer par initialisation
        public Submission(string p_assignment, List<string> p_students)
        {
            this.assignment = p_assignment;
            this.students = p_students;
            //this.isFinalized = true;

        }
    }
}
