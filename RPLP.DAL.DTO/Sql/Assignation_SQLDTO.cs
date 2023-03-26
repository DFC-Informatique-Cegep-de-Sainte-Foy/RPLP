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
        public int RepositoryId { get; set; }
        public int StudentId { get; set; }
        public int Status { get; set; }

        public Assignation_SQLDTO()
        {
            ;
        }
        public Assignation_SQLDTO(Assignation assignation)
        { 
            RepositoryId = assignation.RepositoryId;
            StudentId = assignation.StudentId;
            Status = assignation.Status;
        }
        public Assignation ToEntity()
        {
            return new Assignation(this.RepositoryId, this.StudentId, this.Status);
        }
    }
}
