using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.ENTITES
{
    public class Assignation
    {
        public int RepositoryId { get; set; }
        public int StudentId { get; set; }
        public int Status { get; set; }

        public Assignation(int repositoryId, int studentId, int status)
        {
            RepositoryId = repositoryId;
            StudentId = studentId;
            Status = status;
        }

    }
}
