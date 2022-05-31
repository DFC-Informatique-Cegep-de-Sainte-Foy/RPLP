using RPLP.ENTITES;

namespace RPLP.MVC.Models
{
    public class StudentViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public StudentViewModel()
        {

        }

        public StudentViewModel(Student p_student)
        {
            this.Id = p_student.Id;
            this.Username = p_student.Username;
            this.FirstName = p_student.FirstName;
            this.LastName = p_student.LastName;
            this.Email = p_student.Email;

        }
        public string Matricule { get; set; }
    }
}
