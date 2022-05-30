using RPLP.ENTITES;

namespace RPLP.MVC.Models
{
    public class AdministratorViewModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }

        public AdministratorViewModel()
        {

        }

        public AdministratorViewModel(Administrator p_admin)
        {
            this.Id = p_admin.Id;
            this.Username = p_admin.Username;
            this.Token = p_admin.Token;
            this.FirstName = p_admin.FirstName;
            this.LastName = p_admin.LastName;
            this.Email = p_admin.Email;
        }
    }
}
