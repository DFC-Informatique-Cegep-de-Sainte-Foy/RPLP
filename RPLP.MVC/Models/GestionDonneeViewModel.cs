namespace RPLP.MVC.Models
{
    public class GestionDonneeViewModel
    {
        public string RoleType { get; set; }
        public List<AdministratorViewModel> Administrators { get; set; }
        public List<OrganisationViewModel> Organisations { get; set; }
        public List<OrganisationViewModel> AllOrgs { get; set; }
        public List<TeacherViewModel> Teachers { get; set; }
        public List<StudentViewModel> Students { get; set; }
        public List<AssignmentViewModel> Assignments { get; set; }
        public List<ClassroomViewModel> Classes { get; set; }
        public List<AdministratorViewModel> DeactivatedAdministrators { get; set; }
        public List<StudentViewModel> DeactivatedStudents { get; set; }
        public List<TeacherViewModel> DeactivatedTeachers { get; set; }


        public GestionDonneeViewModel()
        {
            Administrators = new List<AdministratorViewModel>();
            Organisations = new List<OrganisationViewModel>();
            Teachers = new List<TeacherViewModel>();
            Assignments = new List<AssignmentViewModel>();
            Students = new List<StudentViewModel>();
            Classes = new List<ClassroomViewModel>();
            AllOrgs = new List<OrganisationViewModel>();
            DeactivatedAdministrators = new List<AdministratorViewModel>();
            DeactivatedStudents = new List<StudentViewModel>();
            DeactivatedTeachers = new List<TeacherViewModel>();
        }
    }
}
