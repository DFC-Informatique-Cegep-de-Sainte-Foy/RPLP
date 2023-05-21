using RPLP.ENTITES;

namespace RPLP.MVC.Models
{
    public class AllocationViewModel
    {
        public string Id { get; set; }
        public string RepositoryName { get; set; }
        public string? StudentName { get; set; }
        public string? TeacherName { get; set; }
        public string? TutorName { get; set; }
        public int Status { get; set; }

        public AllocationViewModel()
        {
            ;
        }

        public AllocationViewModel(string p_id, string p_repositoryName, string? p_studentName, string? p_teacherName,string? p_tutorName,  int p_status)
        {
            this.Id = p_id;
            this.RepositoryName = p_repositoryName;
            this.StudentName = p_studentName;
            this.TeacherName = p_teacherName;
            this.TutorName = p_tutorName;
            this.Status = p_status;
        }
    }
}
