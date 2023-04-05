using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RPLP.JOURNALISATION;

namespace RPLP.ENTITES
{
    public class Allocation
    {
        public string Id { get; set; }
        public int RepositoryId { get; set; }
        public int? StudentId { get; set; }
        public int? TeacherId { get; set; }
        public int Status { get; set; }
        public Allocation()
        {
            ;
        }
        public Allocation(string id, int repositoryId, int? studentId, int? teacherId, int status)
        {
            // RPLP.JOURNALISATION.Logging.Instance.Journal(
            //     new Log($"Allocation.cs - Allocation(int repositoryId, int studentId, int status)" +
            //             $"repositoryId={repositoryId}" +
            //             $"studentId={studentId}" +
            //             $"status={status}"));
            
            Id = id;
            RepositoryId = repositoryId;
            StudentId = studentId;
            TeacherId = teacherId;
            Status = status;
        }
    }
}
