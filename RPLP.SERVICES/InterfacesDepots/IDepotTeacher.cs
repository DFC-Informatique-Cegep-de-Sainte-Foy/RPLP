using RPLP.ENTITES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.SERVICES.InterfacesDepots
{
    public interface IDepotTeacher
    {
        public List<Teacher> GetTeachers();
        public Teacher GetTeacherById(int id);
        public void UpsertTeacher(Teacher teacher);
    }
}
