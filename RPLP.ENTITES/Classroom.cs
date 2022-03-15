namespace RPLP.ENTITES
{
    public class Classroom
    {
        public int id { get; set; }
        public string name { get; set; }

        public string organisationName { get; set; }
        public List<Student> students { get; set; }
        public List<Teacher> teachers { get; set; }
        public List<Assignment> assignment { get; set; }

        public Classroom()
        {
            this.students = new List<Student>();
            this.teachers = new List<Teacher>();
            this.assignment = new List<Assignment>();
        }

        public Classroom(int p_id, string p_name, string p_organisationName, List<Student> p_students, List<Teacher> p_teachers, List<Assignment> p_assignment)
        {
            this.id = p_id;
            this.name = p_name;
            this.organisationName = p_organisationName;
            this.students = p_students;
            this.teachers = p_teachers;
            this.assignment = p_assignment;
        }
    }
}