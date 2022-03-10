namespace RPLP.ENTITES
{
    public class Classroom
    {
        public int id { get; set; }
        public string name { get; set; }
        public List<Student> students { get; set; }
        public List<Teacher> teachers { get; set; }
        public List<Assignment> assignment { get; set; }

        public Classroom()
        {
            this.students = new List<Student>();
            this.teachers = new List<Teacher>();
            this.assignment = new List<Assignment>();
        }

        public Classroom(int id, string name, List<Student> students, List<Teacher> teachers, List<Assignment> assignment)
        {
            this.id = id;
            this.name = name;
            this.students = students;
            this.teachers = teachers;
            this.assignment = assignment;
        }
    }
}