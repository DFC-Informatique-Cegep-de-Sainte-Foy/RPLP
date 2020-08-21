using System.Collections.Generic;

namespace RplpAvecBD.Model
{
    public class CourseRoster
    {
        //les proprietes
        public int id { get; set; } //id le cours
        public List<string> students { get; set; } //liste des etudiants
        public List<string> graders { get; set; } //liste des evaluateurs

        //constucter par initialisation
        public CourseRoster(int p_id, List<string> p_listStudents)
        {
            this.id = p_id;
            this.students = p_listStudents;
            this.graders = p_listStudents;
        }
        public CourseRoster(int p_id)
        {
            this.id = p_id;
        }
    }
}
