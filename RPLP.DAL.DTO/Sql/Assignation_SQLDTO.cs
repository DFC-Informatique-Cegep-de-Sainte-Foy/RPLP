using RPLP.ENTITES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.DAL.DTO.Sql
{
    public class Assignation_SQLDTO
    {
        public int Id { get; set; }
        public Repository Repository { get; set; }
        public Student Student { get; set; }
        public string Status { get; set; }

        public Assignation_SQLDTO()
        {
            ;
        }
        public Assignation_SQLDTO(Assignation assignation)
        { 
            Id = assignation.Id;
            Repository = assignation.Repository;
            Student = assignation.Student;
            Status = assignation.Status;
        }
        public Assignation ToEntity()
        {
            return new Assignation(this.Id,this.Repository,this.Student,this.Status);
        }
    }
}
