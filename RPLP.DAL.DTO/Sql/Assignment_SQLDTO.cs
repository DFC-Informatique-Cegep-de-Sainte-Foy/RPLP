using RPLP.ENTITES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.DAL.DTO.Sql
{
    public class Assignment_SQLDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ClassroomId { get; set; }
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
            this.ClassroomId = p_assignment.ClassroomId;
            this.Description = p_assignment.Description;
            this.DeliveryDeadline = p_assignment.DeliveryDeadline;
            this.DistributionDate = p_assignment.DistributionDate;
            this.Active = true;
        }

        public Assignment ToEntity()
        {
            if (this.DeliveryDeadline != null)
            {
                return new Assignment(this.Id, this.Name, this.ClassroomId, this.Description, this.DeliveryDeadline, this.DistributionDate);
            }

            return new Assignment(this.Id, this.Name, this.ClassroomId, this.Description, this.DistributionDate);
        }
    }
}
