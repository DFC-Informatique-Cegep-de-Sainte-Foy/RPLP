using Microsoft.EntityFrameworkCore;
using RPLP.DAL.DTO.Sql;
using RPLP.ENTITES;
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
            List<Assignation_SQLDTO> assignationsResult = this._context.Assignation.Where(assignation => assignation.Status > 0).ToList();

            List<Assignation> assignations = assignationsResult.Select(assignation => assignation.ToEntity()).ToList();

            Logging.Journal(new Log("Assignation", $"DepotAssignation - Method - GetAssignations() - Return List<Assignation>"));

            return assignations;
        }

        public List<Assignation> GetAssignationsByStudentId(int p_studentId)
        {
            if (p_studentId < 0)
            {
                Logging.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "DepotAssignation - GetAssignationsByStudentId - p_studentId passé en paramêtre est hors des limites", 0));
            }

            List<Assignation_SQLDTO> assignationsResult = this._context.Assignation.Where(assignation => assignation.Status > 0 && assignation.StudentId == p_studentId).ToList();

            List<Assignation> assignations = assignationsResult.Select(assignation => assignation.ToEntity()).ToList();

            Logging.Journal(new Log("Assignation", $"DepotAssignation - Method - GetAssignationsByStudentId() - Return List<Assignation>"));

            return assignations;
        }

        public List<Assignation> GetAssignationsByRepositoryID(int p_repositoryId)
        {
            if (p_repositoryId < 0)
            {
                Logging.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "DepotAssignation - GetAssignationsByRepositoryID - p_repositoryId passé en paramêtre est hors des limites", 0));
            }

            List<Assignation_SQLDTO> assignationsResult = this._context.Assignation.Where(assignation => assignation.Status > 0 && assignation.RepositoryId == p_repositoryId).ToList();

            List<Assignation> assignations = assignationsResult.Select(assignation => assignation.ToEntity()).ToList();

            Logging.Journal(new Log("Assignation", $"DepotAssignation - Method - GetAssignationsByRepositoryID() - Return List<Assignation>"));

            return assignations;
        }

        public Assignation GetAssignationByStudentAndRepositoryIDs(int p_studentId, int p_repositoryId)
        {
            if (p_studentId < 0)
            {
                Logging.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "DepotAssignation - GetAssignationByStudentAndRepositoryIDs - p_studentId passé en paramêtre est hors des limites", 0));
            }

            if (p_repositoryId < 0)
            {
                Logging.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "DepotAssignation - GetAssignationByStudentAndRepositoryIDs - p_repositoryId passé en paramêtre est hors des limites", 0));
            }

            Assignation_SQLDTO assignationResult = this._context.Assignation.
                FirstOrDefault(assignation => assignation.Status > 0 && assignation.StudentId == p_studentId && assignation.RepositoryId == p_repositoryId);

            if (assignationResult == null)
            {
                Logging.Journal(new Log("Assignation", $"DepotAssignation - Method -  GetAssignationByStudentAndRepositoryIDs - Return Assignation - assignationResult est null", 0));

                return null;
            }

            Assignation assignation = assignationResult.ToEntity();

            Logging.Journal(new Log("Assignation", $"DepotAssignation - Method - GetAssignationByStudentAndRepositoryIDs() - Return List<Assignation>"));

            return assignation;
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
            if (p_assignation == null)
            {
                Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotAssignation - UpsertAssignation - p_assignation passé en paramètre est null", 0));
            }

            Assignation_SQLDTO assignationResult = this._context.Assignation.
                SingleOrDefault(assignation => assignation.Status > 0 && assignation.StudentId == p_assignation.StudentId && assignation.RepositoryId == p_assignation.RepositoryId);

            if (assignationResult != null)
            {
                assignationResult.StudentId = p_assignation.StudentId;
                assignationResult.RepositoryId = p_assignation.RepositoryId;
                assignationResult.Status = p_assignation.Status;

                this._context.Update(assignationResult);
                this._context.SaveChanges();

                Logging.Journal(new Log("Assignation", $"DepotAssignation - Method - UpsertAssignation(Assignation p_assignation) - Void - Update Assignation"));
            }
            else
            {
                Assignation_SQLDTO assignation = new Repository_SQLDTO();
                assignation.StudentId = p_assignation.StudentId;
                assignation.RepositoryId = p_assignation.RepositoryId;
                assignation.Status = p_assignation.Status; ;

                this._context.Assignations.Add(assignation);
                this._context.SaveChanges();

                Logging.Journal(new Log("Assignation", $"DepotAssignation - Method - UpsertAssignation(Assignation p_assignation) - Void - Add Assignation"));
            }
        }

        public void DeleteAssignation(Assignation p_assignation)
        {
            if (p_assignation == null)
            {
                Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotAssignation - DeleteAssignation - p_assignation passé en paramètre est null", 0));
            }

            Assignation_SQLDTO assignationResult = this._context.Assignation.
                SingleOrDefault(assignation => assignation.Status > 0 && assignation.StudentId == p_assignation.StudentId && assignation.RepositoryId == p_assignation.RepositoryId);

            if (assignationResult != null)
            {
                assignationResult.Status = 0;

                this._context.Update(assignationResult);
                this._context.SaveChanges();

                .Logging.Journal(new Log("Assignation", $"DepotAssignation - Method - DeleteAssignation(Assignation p_assignation) - Void - delete Assignation"));
            }
            else
            {
                Logging.Journal(new Log("Assignation", $"DepotAssignation - Method - DeleteAssignation(Assignation p_assignation) - Void - assignationResult est null", 0));
            }
        }
    }
}
