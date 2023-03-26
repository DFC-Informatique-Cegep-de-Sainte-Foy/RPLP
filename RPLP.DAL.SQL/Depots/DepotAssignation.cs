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
                Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
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
                SingleOrDefault(assignation => assignation.Status > 0 && assignation.StudentId == p_studentId && assignation.RepositoryId == p_repositoryId);

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
            if (p_id < 0)
            {
                Logging.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "DepotAssignation - GetAssignationsByAssignmentID - p_assignmentId passé en paramêtre est hors des limites", 0));
            }

            List<Assignation> assignations = new List<Assignation>();

            Assignment_SQLDTO? assignmentResult = this._context.Assignments.SingleOrDefault(assignment => assignment.Id == p_assignmentId && assignment.Active);

            if (assignmentResult is null)
            {
                Logging.Journal(new Log("Assignations", $"DepotAssignation - Method - GetAssignationsByAssignmentID(int p_assignmentId) - Return List<Assignation> - assignmentResult est null", 0));
            }
            else
            {
                Classroom_SQLDTO? classroomResult = this._context.Classrooms.SingleOrDefault(classroom => classroom.Active && classroom.Name == assignmentResult.ClassroomName);

                if (classroomResult is null)
                {
                    Logging.Journal(new Log("Assignations", $"DepotAssignation - Method - GetAssignationsByAssignmentID(int p_assignmentId) - Return List<Assignation> - classroomResult est null", 0));
                }
                else
                {
                    List<Repository_SQLDTO> repositoriesResult = this._context.Repositories.Where(repository => repository.OrganisationName == classroomResult.OrganisationName && repository.FullName.StartsWith(assignmentResult.Name)).ToList();

                    if (repositoriesResult is null)
                    {
                        Logging.Journal(new Log("Assignations", $"DepotAssignation - Method - GetAssignationsByAssignmentID(int p_assignmentId) - Return List<Assignation> - repositoriesResult est null", 0));
                    }
                    else
                    {
                        foreach (Repository_SQLDTO repos in repositoriesResult)
                        {

                            assignations.AddRange(this.GetAssignationsByRepositoryID(repos.Id));
                        }
                    }
                }
            }

            return assignations;
        }

        public List<Assignation> GetAssignationsByStudentUsername(string p_studentUsername)
        {
            if (string.IsNullOrWhiteSpace(p_studentUsername))
            {
                Logging.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "DepotAssignation - GetAssignationsByStudentUsername - p_studentUsername passé en paramêtre est vide", 0));
            }

            List<Assignation> assignations = new List<Assignation>();

            Student_SQLDTO? studentResult = this._context.Students.SingleOrDefault(student => student.Username == p_studentUsername);

            if (studentResult == null)
            {
                Logging.Journal(new Log("Assignation", $"DepotAssignation - Method - GetAssignationsByStudentUsername(string p_studentUsername) - List<Assignation> - studentResult est null", 0));
            }

            List<Assignation_SQLDTO> assignationsResult = this._context.Assignation.Where(assignation => assignation.Status > 0 && assignation.StudentId == studentResult.Id).ToList();

            assignations = assignationsResult.Select(assignation => assignation.ToEntity()).ToList();

            Logging.Journal(new Log("Assignation", $"DepotAssignation - Method - GetAssignationsByStudentUsername() - Return List<Assignation>"));

            return assignations;
        }

        public List<Assignation> GetAssignationsByRepositoryName(string p_repositoryName)
        {
            if (string.IsNullOrWhiteSpace(p_repositoryName))
            {
                Logging.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "DepotAssignation - GetAssignationsByRepositoryName - p_repositoryName passé en paramêtre est vide", 0));
            }

            List<Assignation> assignations = new List<Assignation>();

            Repository_SQLDTO? repositoryResult = this._context.Repository.SingleOrDefault(repository => repository.Name == p_repositoryName);

            if (studentResult == null)
            {
                Logging.Journal(new Log("Assignation", $"DepotAssignation - Method - GetAssignationsByRepositoryName(string p_repositoryName) - List<Assignation> - repositoryResult est null", 0));
            }

            List<Assignation_SQLDTO> assignationsResult = this._context.Assignation.Where(assignation => assignation.Status > 0 && assignation.RepositoryId == repositoryResult.Id).ToList();

            assignations = assignationsResult.Select(assignation => assignation.ToEntity()).ToList();

            Logging.Journal(new Log("Assignation", $"DepotAssignation - Method - GetAssignationsByRepositoryName() - Return List<Assignation>"));

            return assignations;
        }

        public Assignation GetAssignationByStudentAndRepositoryNames(string p_studentUsername, string p_repositoryName)
        {
            if (string.IsNullOrWhiteSpace(p_studentUsername))
            {
                Logging.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "DepotAssignation - GetAssignationByStudentAndRepositoryNames - p_studentUsername passé en paramêtre est vide", 0));
            }

            if (string.IsNullOrWhiteSpace(p_repositoryName))
            {
                Logging.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "DepotAssignation - GetAssignationByStudentAndRepositoryNames - p_repositoryName passé en paramêtre vide", 0));
            }

            Assignation_SQLDTO assignationResult = this._context.Assignation.
                SingleOrDefault(assignation => assignation.Status > 0 && assignation.Username == p_studentUsername && assignation.Name == p_repositoryName);

            if (assignationResult == null)
            {
                Logging.Journal(new Log("Assignation", $"DepotAssignation - Method -  GetAssignationByStudentAndRepositoryNames - Return Assignation - assignationResult est null", 0));

                return null;
            }

            Assignation assignation = assignationResult.ToEntity();

            Logging.Journal(new Log("Assignation", $"DepotAssignation - Method - GetAssignationByStudentAndRepositoryNames() - Return Assignation"));

            return assignation;
        }
        
        public List<Assignation> GetAssignationsByAssignmentName(string p_assignmentName)
        {
            if (string.IsNullOrWhiteSpace(p_assignmentName))
            {
                Logging.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "DepotAssignation - GetAssignationsByAssignmentName - p_assignmentName passé en paramêtre est vide", 0));
            }

            List<Assignation> assignations = new List<Assignation>();

            Assignment_SQLDTO? assignmentResult = this._context.Assignments.SingleOrDefault(assignment => assignment.Name == p_assignmentName && assignment.Active);

            if (assignmentResult is null)
            {
                Logging.Journal(new Log("Assignations", $"DepotAssignation - Method - GetAssignationsByAssignmentName(string p_assignmentName) - Return List<Assignation> - assignmentResult est null", 0));
            }
            else
            {
                assignations = this.GetAssignationsByAssignmentID(assignmentResult.Id);
            }

            return assignations;
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

                Logging.Journal(new Log("Assignation", $"DepotAssignation - Method - DeleteAssignation(Assignation p_assignation) - Void - delete Assignation"));
            }
            else
            {
                Logging.Journal(new Log("Assignation", $"DepotAssignation - Method - DeleteAssignation(Assignation p_assignation) - Void - assignationResult est null", 0));
            }
        }
    }
}
