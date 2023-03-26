using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.ENTITES
{
    public class Assignation
    {
        public int Id { get; set; }
        public Repository Repository { get; set; }
        public Student Student { get; set; }
        public string Status { get; set; }

        public Assignation(int id, Repository repository, Student student, string status)
        {
            Id = id;
            Repository = repository;
            Student = student;
            Status = status;
        }

    }
}
