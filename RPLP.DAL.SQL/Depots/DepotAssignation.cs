using Microsoft.EntityFrameworkCore;
using RPLP.JOURNALISATION;
using RPLP.SERVICES.InterfacesDepots;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.DAL.SQL.Depots
{
    public class DepotAssignation : IDepotAssignation
    {
        private readonly RPLPDbContext _context;

        public DepotAssignation()
        {
            this._context = new RPLPDbContext(new DbContextOptionsBuilder<RPLPDbContext>().UseSqlServer("Server=rplp.db; Database=RPLP; User Id=sa; password=Cad3pend86!").Options);
        }

        public DepotAssignation(RPLPDbContext p_context)
        {
            if (p_context == null)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                               "DepotAssignation - DepotAssignation(RPLPDbContext p_context) - p_context de type RPLPDbContext passé en paramètre est null", 0));
            }

            _context = p_context;
        }

        public List<Assignation> GetAssignations()
        {
            throw new NotImplementedException();
        }

        public List<Assignation> GetAssignationsByStudentId(int p_studentId)
        {
            throw new NotImplementedException();
        }

        public List<Assignation> GetAssignationsByRepositoryID(int p_repositoryId)
        {
            throw new NotImplementedException();
        }

        public Assignation GetAssignationByStudentAndRepositoryIDs(int p_studentId, int p_repositoryId)
        {
            throw new NotImplementedException();
        }

        public List<Assignation> GetAssignationsByAssignmentID(int p_assignmentId)
        {
            throw new NotImplementedException();
        }

        public List<Assignation> GetAssignationsByStudentUsername(string p_studentUsername)
        {
            throw new NotImplementedException();
        }

        public List<Assignation> GetAssignationsByRepositoryName(string p_repositoryName)
        {
            throw new NotImplementedException();
        }

        public Assignation GetAssignationByStudentAndRepositoryNames(string p_studentUsername, string p_repositoryName)
        {
            throw new NotImplementedException();
        }
        
        public List<Assignation> GetAssignationsByAssignmentName(string p_assignmentName)
        {
            throw new NotImplementedException();
        }

        public void UpsertAssignation(Assignation p_assignation)
        {
            throw new NotImplementedException();
        }

        public void DeleteAssignation(Assignation p_assignation)
        {
            throw new NotImplementedException();
        }
    }
}
