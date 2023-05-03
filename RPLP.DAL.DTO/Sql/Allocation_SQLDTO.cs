using RPLP.ENTITES;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.DAL.DTO.Sql
{
    public class Allocation_SQLDTO
    {
        public string Id { get; set; }
        public int RepositoryId { get; set; }
        public int? StudentId { get; set; }
        public int? TeacherId { get; set; }
        public int Status { get; set; }

        public Allocation_SQLDTO()
        {
            ;
        }
        public Allocation_SQLDTO(Allocation assignation)
        { 
            Id = assignation.Id;
            RepositoryId = assignation.RepositoryId;
            StudentId = assignation.StudentId;
            TeacherId = assignation.TeacherId;
            Status = assignation.Status;
        }
        public Allocation ToEntity()
        {
            return new Allocation(this.Id, this.RepositoryId, this.StudentId, this.TeacherId, this.Status);
        }
    }
}
