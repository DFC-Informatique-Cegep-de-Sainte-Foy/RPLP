namespace RPLP.MVC.Models
{
    public class AssignmentViewModel
    {
        public string Name { get; set; }

        public AssignmentViewModel(string p_name)
        {
            Name = p_name.Trim();
        }
    }
}
