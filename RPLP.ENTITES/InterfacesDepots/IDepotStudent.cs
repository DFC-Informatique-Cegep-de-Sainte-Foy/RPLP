using RPLP.ENTITES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.ENTITES.InterfacesDepots
{
    public interface IDepotStudent
    {
        public List<Student> GetStudents();
        public List<Student> GetDeactivatedStudents();
        public Student GetStudentById(int p_id);
        public Student GetStudentByUsername(string p_studentUsername);
        public List<Classroom> GetStudentClasses(string p_studentUsername);
        public void UpsertStudent(Student p_student);
        public void DeleteStudent(string p_studentUsername);
        public void ReactivateStudent(string p_studentUsername);
    }
}

