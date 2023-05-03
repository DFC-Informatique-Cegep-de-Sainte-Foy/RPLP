using RPLP.ENTITES;

namespace RPLP.MVC.Models
{
    public class AllocationViewModel
    {
        public string Id { get; set; }
        public string RepositoryName { get; set; }
        public string? StudentName { get; set; }
        public string? TeacherName { get; set; }
        public int Status { get; set; }

        public AllocationViewModel()
        {
            ;
        }

        public AllocationViewModel(string p_id, string p_repositoryName, string? StudentName, string? TeacherName, int Status)
        {
            this.Id = p_id;
            this.RepositoryName = p_repositoryName;
            this.StudentName = StudentName;
            this.TeacherName = TeacherName;
            this.Status = Status;
        }
    }
}
