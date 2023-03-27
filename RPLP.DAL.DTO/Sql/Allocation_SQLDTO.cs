using RPLP.ENTITES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.DAL.DTO.Sql
{
    public class Allocation_SQLDTO
    {
        public int RepositoryId { get; set; }
        public int StudentId { get; set; }
        public int Status { get; set; }

        public Allocation_SQLDTO()
        {
            ;
        }
        public Allocation_SQLDTO(Allocation assignation)
        { 
            RepositoryId = assignation.RepositoryId;
            StudentId = assignation.StudentId;
            Status = assignation.Status;
        }
        public Allocation ToEntity()
        {
            return new Allocation(this.RepositoryId, this.StudentId, this.Status);
        }
    }
}
