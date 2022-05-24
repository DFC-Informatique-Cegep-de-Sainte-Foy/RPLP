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
        public Classroom GetClassroomById(int p_id);
        public Classroom GetClassroomByName(string p_classroomName);
        public List<Student> GetStudentsByClassroomName(string p_classroomName);
        public List<Teacher> GetTeachersByClassroomName(string p_classroomName);
        public List<Assignment> GetAssignmentsByClassroomName(string p_classroomName);
        public List<Classroom> GetClassroomsByOrganisationName(string p_organisationName);
        public void AddTeacherToClassroom(string p_classroomName, string p_teacherUsername);
        public void RemoveTeacherFromClassroom(string p_classroomName, string p_teacherUsername);
        public void AddStudentToClassroom(string p_classroomName, string p_studentUsername);
        public void RemoveStudentFromClassroom(string p_classroomName, string p_studentUsername);
        public void AddAssignmentToClassroom(string p_classroomName, string p_assignmentName);
        public void RemoveAssignmentFromClassroom(string p_classroomName, string p_assignmentName);
        public void UpsertClassroom(Classroom p_classroom);
        public void DeleteClassroom(string p_classroomName);
    }
}
