namespace RPLP.ENTITES
{
    public class Classroom
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string OrganisationName { get; set; }
        public List<Student> Students { get; set; }
        public List<Teacher> Teachers { get; set; }
        public List<Assignment> Assignments { get; set; }

        public Classroom()
        {
            this.Students = new List<Student>();
            this.Teachers = new List<Teacher>();
            this.Assignments = new List<Assignment>();
        }

        public Classroom(int p_id, string p_name, string p_organisationName, List<Student> p_students, List<Teacher> p_teachers, List<Assignment> p_assignments)
        {
            this.Id = p_id;
            this.Name = p_name;
            this.OrganisationName = p_organisationName;
            this.Students = p_students;
            this.Teachers = p_teachers;
            this.Assignments = p_assignments;
        }
    }
}