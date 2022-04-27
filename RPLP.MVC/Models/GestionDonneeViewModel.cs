namespace RPLP.MVC.Models
{
    public class GestionDonneeViewModel
    {
        public string RoleType { get; set; }
        public List<AdministratorViewModel> Administrators { get; set; }
        public List<OrganisationViewModel> Organisations { get; set; }
        public List<TeacherViewModel> Teachers { get; set; }

        public GestionDonneeViewModel()
        {
            Administrators = new List<AdministratorViewModel>();
            Organisations = new List<OrganisationViewModel>();
            Teachers = new List<TeacherViewModel>();
        }
    }
}
