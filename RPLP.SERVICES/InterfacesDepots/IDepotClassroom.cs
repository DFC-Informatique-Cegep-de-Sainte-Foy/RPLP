using RPLP.ENTITES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.SERVICES.InterfacesDepots
{
    public interface IDepotClassroom
    {
        public List<Classroom> GetClassrooms();
        public Classroom GetClassroomById(int id);
        public Classroom GetClassroomByName(string name);
        public void UpsertClassroom(Classroom classroom);
    }
}
