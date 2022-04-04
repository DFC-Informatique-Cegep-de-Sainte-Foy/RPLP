namespace RPLP.MVC.Models
{
    public class PeerReviewViewModel
    {
        public List<OrganisationViewModel> Organisations { get; set; }
        public List<AssignmentViewModel> Assignment { get; set; }
        public List<ClassroomViewModel> Classrooms { get; set; }

        public PeerReviewViewModel()
        {
            Organisations = new List<OrganisationViewModel>();
            Assignment = new List<AssignmentViewModel>();
            Classrooms = new List<ClassroomViewModel>();
        }
    }


}
