using RPLP.ENTITES;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace RPLP.DAL.DTO.Sql
{
    public class Assignment_SQLDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ClassroomId { get; set; }
        [ForeignKey("ClassroomId")]
        public Classroom_SQLDTO Classroom { get; set; }
        public string Description { get; set; }
        public DateTime? DeliveryDeadline { get; set; }
        public DateTime DistributionDate { get; set; }
        public bool Active { get; set; }

        public Assignment_SQLDTO()
        {

        }

        public Assignment_SQLDTO(Assignment p_assignment)
        {
            this.Id = p_assignment.Id;
            this.Name = p_assignment.Name;
            this.Classroom = new Classroom_SQLDTO(p_assignment.Classroom);
            this.Description = p_assignment.Description;
            this.DeliveryDeadline = p_assignment.DeliveryDeadline;
            this.DistributionDate = p_assignment.DistributionDate;
            this.Active = true;
        }

        public Assignment ToEntity()
        {
            if (this.DeliveryDeadline != null)
            {
                return new Assignment(this.Id, this.Name, new Classroom(this.Id, this.Name, this.Classroom.Organisation.ToEntity()), this.Description, this.DeliveryDeadline, this.DistributionDate);
            }

            return new Assignment(this.Id, this.Name, new Classroom(this.Id, this.Name, this.Classroom.Organisation.ToEntity()), this.Description, this.DistributionDate);
        }
    }
}
