using RPLP.ENTITES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.SERVICES.InterfacesDepots
{
    public interface IDepotStudent
    {
        public List<Student> GetStudents();
        public Student GetStudentById(int id);
        public void UpsertStudent(Student student);
    }
}
