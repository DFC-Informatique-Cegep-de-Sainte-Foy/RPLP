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
        public string description { get; set; }
        public DateTime deliveryDeatline { get; set; }
        public DateTime distributionDate { get; set; }

        public Assignment()
        {

        }
    }
}
