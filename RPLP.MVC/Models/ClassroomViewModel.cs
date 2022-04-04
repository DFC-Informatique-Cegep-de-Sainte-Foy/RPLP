namespace RPLP.MVC.Models
{
    public class ClassroomViewModel
    {
        public string Name { get; set; }

        public ClassroomViewModel(string p_name)
        {
            Name = p_name.Trim();
        }
    }
}
