﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.ENTITES
{
    public class Assignment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ClassroomId { get; set; }
        public string Description { get; set; }
        public DateTime? DeliveryDeadline { get; set; }
        public DateTime DistributionDate { get; set; }

        public Assignment()
        {

        }

        public Assignment(int p_id, string p_name, int p_classroomId, string p_description, DateTime p_distributionDate)
        {
            this.Id = p_id;
            this.Name = p_name;
            this.ClassroomId = p_classroomId;
            this.Description = p_description;
            this.DistributionDate = p_distributionDate;
        }

        public Assignment(int p_id, string p_name, int p_classroomId,
            string p_description, DateTime? p_deliveryDeadline, DateTime p_distributionDate)
        {
            this.Id = p_id;
            this.Name = p_name;
            this.ClassroomId = p_classroomId;
            this.Description = p_description;
            this.DeliveryDeadline = p_deliveryDeadline;
            this.DistributionDate = p_distributionDate;
        }
        
        
    }
}
