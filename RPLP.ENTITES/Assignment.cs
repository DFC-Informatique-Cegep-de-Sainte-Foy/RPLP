using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.ENTITES
{
    public class Assignment
    {
        public int id { get; set; }
        public string name { get; set; }
        public string classroomName { get; set; }
        public string description { get; set; }
        public DateTime? deliveryDeatline { get; set; }
        public DateTime distributionDate { get; set; }

        public Assignment()
        {

        }

        public Assignment(int p_id, string p_name, string p_classroomName, string p_description, DateTime p_distributionDate)
        {
            this.id = p_id;
            this.name = p_name;
            this.classroomName = p_classroomName;
            this.description = p_description;
            this.distributionDate = p_distributionDate;
        }

        public Assignment(int p_id, string p_name, string p_classroomName, string p_description, DateTime p_deliveryDeadline, DateTime p_distributionDate)
        {
            this.id = p_id;
            this.name = p_name;
            this.classroomName = p_classroomName;
            this.description = p_description;
            this.deliveryDeatline = p_deliveryDeadline;
            this.distributionDate = p_distributionDate;
        }
    }
}
