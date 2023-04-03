using RPLP.ENTITES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.DAL.DTO.Json
{
    public class Allocations_JSONDTO
    {
        public List<Allocation> Pairs { get; set; }
        public Classroom _classroom { get; set; }
        public List<Repository> _repositories { get; set;  }

        public int Status
        {
            get
            {
                int status = int.MaxValue;
                foreach (Allocation allocation in this.Pairs)
                {
                    if (allocation.Status < status)
                    {
                        status = allocation.Status;
                    }
                }

                return status;
            }
        }

        public Allocations_JSONDTO()
        {
            
        }

        public Allocations_JSONDTO(Allocations p_allocations)
        {
            this.Pairs = p_allocations.Pairs;
            this._classroom = p_allocations._classroom;
            this._repositories = p_allocations._repositories;
        }
    }
}
