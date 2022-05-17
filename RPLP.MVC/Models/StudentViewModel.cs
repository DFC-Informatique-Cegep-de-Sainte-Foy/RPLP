namespace RPLP.MVC.Models
{
    public class StudentViewModel
    {
        public string Name { get; set; }

        public StudentViewModel(string p_name)
        {
            this.Name = p_name.Trim();
        }
    }
}
