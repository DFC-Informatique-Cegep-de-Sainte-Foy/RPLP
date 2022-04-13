namespace RPLP.MVC.Models
{
    public class OrganisationViewModel
    {
        public string Name { get; set; }

        public OrganisationViewModel(string name)
        {
            Name = name.Trim();
        }
    }
}
