using RPLP.ENTITES;

namespace RPLP.MVC.Models
{
    public class TeacherViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public TeacherViewModel()
        {

        }

        public TeacherViewModel(Teacher p_teacher)
        {
            this.Id = p_teacher.Id;
            this.Username = p_teacher.Username;
            this.Email = p_teacher.Email;
            this.FirstName = p_teacher.FirstName;
            this.LastName = p_teacher.LastName;
        }
    }

}
