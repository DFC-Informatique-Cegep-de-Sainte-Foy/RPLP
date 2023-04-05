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
    public class DepotAllocation : IDepotAllocation
    {
        private readonly RPLPDbContext _context;

        public DepotAllocation(RPLPDbContext p_context)
        {
            if (p_context is null)
            {
                Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                               "DepotAllocation - DepotAllocation(RPLPDbContext p_context) - p_context de type RPLPDbContext passé en paramètre est null", 0));
                throw new ArgumentNullException(nameof(p_context));
            }

            _context = p_context;
        }

        public List<Allocation> GetAllocations()
        {
            List<Allocation_SQLDTO> allocationsResult = this._context.Allocations.Where(allocation => allocation.Status > 0).ToList();

            List<Allocation> allocations = allocationsResult.Select(allocation => allocation.ToEntity()).ToList();

            Logging.Instance.Journal(new Log("Allocation", $"DepotAllocation - Method - GetAllocations() - Return List<Allocation>"));

            return allocations;
        }

        public List<Allocation> GetAllocationsByStudentId(int p_studentId)
        {
            if (p_studentId <= 0)
            {
                Logging.Instance.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "DepotAllocation - GetAllocationsByStudentId - p_studentId passé en paramêtre est hors des limites", 0));
                throw new ArgumentOutOfRangeException(nameof(p_studentId));
            }

            List<Allocation_SQLDTO> allocationsResult = this._context.Allocations.Where(allocation => allocation.Status > 0 && allocation.StudentId == p_studentId).ToList();

            List<Allocation> allocations = allocationsResult.Select(allocation => allocation.ToEntity()).ToList();

            Logging.Instance.Journal(new Log("Allocation", $"DepotAllocation - Method - GetAllocationsByStudentId() - Return List<Allocation>"));

            return allocations;
        }

        public List<Allocation> GetAllocationsByRepositoryID(int p_repositoryId)
        {
            if (p_repositoryId <= 0)
            {
                Logging.Instance.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "DepotAllocation - GetAllocationsByRepositoryID - p_repositoryId passé en paramêtre est hors des limites", 0));
                throw new ArgumentOutOfRangeException(nameof(p_repositoryId));
            }

            List<Allocation_SQLDTO> allocationsResult = this._context.Allocations.Where(allocation => allocation.Status > 0 && allocation.RepositoryId == p_repositoryId).ToList();

            List<Allocation> allocations = allocationsResult.Select(allocation => allocation.ToEntity()).ToList();

            Logging.Instance.Journal(new Log("Allocation", $"DepotAllocation - Method - GetAllocationsByRepositoryID() - Return List<Allocation>"));

            return allocations;
        }

        public Allocation GetAllocationByStudentAndRepositoryIDs(int p_studentId, int p_repositoryId)
        {
            if (p_studentId <= 0)
            {
                Logging.Instance.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "DepotAllocation - GetAllocationByStudentAndRepositoryIDs - p_studentId passé en paramêtre est hors des limites", 0));
                throw new ArgumentOutOfRangeException(nameof(p_studentId));
            }

            if (p_repositoryId <= 0)
            {
                Logging.Instance.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "DepotAllocation - GetAllocationByStudentAndRepositoryIDs - p_repositoryId passé en paramêtre est hors des limites", 0));
                throw new ArgumentOutOfRangeException(nameof(p_repositoryId));
            }

            try
            {
                Allocation_SQLDTO? allocationResult = this._context.Allocations.
                SingleOrDefault(allocation => allocation.Status > 0 && allocation.StudentId == p_studentId && allocation.RepositoryId == p_repositoryId);

                if (allocationResult is null)
                {
                    Logging.Instance.Journal(new Log("Allocation", $"DepotAllocation - Method -  GetAllocationByStudentAndRepositoryIDs - Return Allocation - allocationResult est null", 0));

                    return null;
                }

                Allocation allocation = allocationResult.ToEntity();

                Logging.Instance.Journal(new Log("Allocation", $"DepotAllocation - Method - GetAllocationByStudentAndRepositoryIDs() - Return List<Allocation>"));

                return allocation;
            }
            catch (InvalidOperationException e)
            {
                Logging.Instance.Journal(new Log(e.ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "DepotAllocation - GetAllocationByStudentAndRepositoryIDs - _context.Allocations.SingleOrDefault trouve plus qu'un élément", 0));
                return null;
            }
        }

        public List<Allocation> GetAllocationsByAssignmentID(int p_assignmentId)
        {
            if (p_assignmentId <= 0)
            {
                Logging.Instance.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "DepotAllocation - GetAllocationsByAssignmentID - p_assignmentId passé en paramêtre est hors des limites", 0));
                throw new ArgumentOutOfRangeException(nameof(p_assignmentId));
            }

            List<Allocation> allocations = new List<Allocation>();

            Assignment_SQLDTO? assignmentResult = this._context.Assignments.SingleOrDefault(assignment => assignment.Id == p_assignmentId && assignment.Active);

            if (assignmentResult is null)
            {
                Logging.Instance.Journal(new Log("Allocations", $"DepotAllocation - Method - GetAllocationsByAssignmentID(int p_assignmentId) - Return List<Allocation> - assignmentResult est null", 0));
            }
            else
            {
                Classroom_SQLDTO? classroomResult = this._context.Classrooms.SingleOrDefault(classroom => classroom.Active && classroom.Name == assignmentResult.ClassroomName);

                if (classroomResult is null)
                {
                    Logging.Instance.Journal(new Log("Allocations", $"DepotAllocation - Method - GetAllocationsByAssignmentID(int p_assignmentId) - Return List<Allocation> - classroomResult est null", 0));
                }
                else
                {
                    List<Repository_SQLDTO> repositoriesResult = this._context.Repositories.Where(repository => repository.OrganisationName.ToLower() == classroomResult.OrganisationName.ToLower() && repository.Name.ToLower().StartsWith(assignmentResult.Name.ToLower())).ToList();

                    if (repositoriesResult is null)
                    {
                        Logging.Instance.Journal(new Log("Allocations", $"DepotAllocation - Method - GetAllocationsByAssignmentID(int p_assignmentId) - Return List<Allocation> - repositoriesResult est null", 0));
                    }
                    else
                    {
                        foreach (Repository_SQLDTO repos in repositoriesResult)
                        {

                            allocations.AddRange(this.GetAllocationsByRepositoryID(repos.Id));
                        }
                    }
                }
            }
            return allocations;
        }

        public List<Allocation> GetAllocationsByStudentUsername(string p_studentUsername)
        {
            if (string.IsNullOrWhiteSpace(p_studentUsername))
            {
                Logging.Instance.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "DepotAllocation - GetAllocationsByStudentUsername - p_studentUsername passé en paramêtre est vide", 0));
            }

            List<Allocation> allocations = new List<Allocation>();

            Student_SQLDTO? studentResult = this._context.Students.SingleOrDefault(student => student.Username == p_studentUsername);

            if (studentResult is null)
            {
                Logging.Instance.Journal(new Log("Allocation", $"DepotAllocation - Method - GetAllocationsByStudentUsername(string p_studentUsername) - List<Allocation> - studentResult est null", 0));
            }
            else
            {
                List<Allocation_SQLDTO> allocationsResult = this._context.Allocations.Where(allocation => allocation.Status > 0 && allocation.StudentId == studentResult.Id).ToList();

                allocations = allocationsResult.Select(allocation => allocation.ToEntity()).ToList();
            }

            Logging.Instance.Journal(new Log("Allocation", $"DepotAllocation - Method - GetAllocationsByStudentUsername() - Return List<Allocation>"));

            return allocations;
        }

        public List<Allocation> GetAllocationsByRepositoryName(string p_repositoryName)
        {
            if (string.IsNullOrWhiteSpace(p_repositoryName))
            {
                Logging.Instance.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "DepotAllocation - GetAllocationsByRepositoryName - p_repositoryName passé en paramêtre est vide", 0));
            }

            List<Allocation> allocations = new List<Allocation>();

            Repository_SQLDTO? repositoryResult = this._context.Repositories.SingleOrDefault(repository => repository.Name == p_repositoryName);

            if (repositoryResult is null)
            {
                Logging.Instance.Journal(new Log("Allocation", $"DepotAllocation - Method - GetAllocationsByRepositoryName(string p_repositoryName) - List<Allocation> - repositoryResult est null", 0));
            }
            else
            {
                List<Allocation_SQLDTO> allocationsResult = this._context.Allocations.Where(allocation => allocation.Status > 0 && allocation.RepositoryId == repositoryResult.Id).ToList();

                allocations = allocationsResult.Select(allocation => allocation.ToEntity()).ToList();
            }

            Logging.Instance.Journal(new Log("Allocation", $"DepotAllocation - Method - GetAllocationsByRepositoryName() - Return List<Allocation>"));

            return allocations;
        }

        public Allocation? GetAllocationByStudentAndRepositoryNames(string p_studentUsername, string p_repositoryName)
        {
            if (string.IsNullOrWhiteSpace(p_studentUsername))
            {
                Logging.Instance.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "DepotAllocation - GetAllocationByStudentAndRepositoryNames - p_studentUsername passé en paramêtre est vide", 0));
            }

            if (string.IsNullOrWhiteSpace(p_repositoryName))
            {
                Logging.Instance.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "DepotAllocation - GetAllocationByStudentAndRepositoryNames - p_repositoryName passé en paramêtre vide", 0));
            }
            try
            {

                Repository_SQLDTO? repositoryResult = this._context.Repositories.SingleOrDefault(repository => repository.Name == p_repositoryName && repository.Active);

                if (repositoryResult is null)
                {
                    Logging.Instance.Journal(new Log("Allocation", $"DepotAllocation - Method -  GetAllocationByStudentAndRepositoryNames - Return Allocation - repositoryResult est null", 0));

                    return null;
                }

                Student_SQLDTO? studentResult = this._context.Students.FirstOrDefault(student => student.Username == p_studentUsername && student.Active);

                if (studentResult is null)
                {
                    Logging.Instance.Journal(new Log("Allocation", $"DepotAllocation - Method -  GetAllocationByStudentAndRepositoryNames - Return Allocation - studentResult est null", 0));

                    return null;
                }

                Allocation_SQLDTO? allocationResult = this._context.Allocations.
                    SingleOrDefault(allocation => allocation.Status > 0 && allocation.StudentId == studentResult.Id && allocation.RepositoryId == repositoryResult.Id);

                if (allocationResult is null)
                {
                    Logging.Instance.Journal(new Log("Allocation", $"DepotAllocation - Method -  GetAllocationByStudentAndRepositoryNames - Return Allocation - allocationResult est null", 0));

                    return null;
                }

                Allocation allocation = allocationResult.ToEntity();

                Logging.Instance.Journal(new Log("Allocation", $"DepotAllocation - Method - GetAllocationByStudentAndRepositoryNames() - Return Allocation"));

                return allocation;
            }
            catch(InvalidOperationException e)
            {
                Logging.Instance.Journal(new Log(e.ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "DepotAllocation - GetAllocationByStudentAndRepositoryIDs - _context.<DbSet>.SingleOrDefault trouve plus qu'un élément", 0));
                return null;
            }

        }

        public List<Allocation> GetAllocationsByAssignmentName(string p_assignmentName)
        {
            if (string.IsNullOrWhiteSpace(p_assignmentName))
            {
                Logging.Instance.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "DepotAllocation - GetAllocationsByAssignmentName - p_assignmentName passé en paramêtre est vide", 0));

                throw new ArgumentNullException(nameof(p_assignmentName));
            }

            List<Allocation> allocations = new List<Allocation>();

            try
            {
                Assignment_SQLDTO? assignmentResult = this._context.Assignments.SingleOrDefault(assignment => assignment.Name == p_assignmentName && assignment.Active);

                if (assignmentResult is null)
                {
                    Logging.Instance.Journal(new Log("Allocations", $"DepotAllocation - Method - GetAllocationsByAssignmentName(string p_assignmentName) - Return List<Allocation> - assignmentResult est null", 0));
                }
                else
                {
                    allocations = this.GetAllocationsByAssignmentID(assignmentResult.Id);
                }
                Logging.Instance.Journal(new Log($"GetAllocationsByAssignmentName(string p_assignmentName) {p_assignmentName}" +
                                        $"allocations.Count: {allocations.Count}"));
                
            }
            catch (InvalidOperationException e)
            {
                Logging.Instance.Journal(new Log(e.ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "DepotAllocation - GetAllocationsByAssignmentName - _context.Assignments.SingleOrDefault trouve plus qu'un élément", 0));
            }

            return allocations;
        }

        public void UpsertAllocation(Allocation p_allocation)
        {
            if (p_allocation is null)
            {
                Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotAllocation - UpsertAllocation - p_allocation passé en paramètre est null", 0));

                throw new ArgumentNullException(nameof(p_allocation));
            }

            try
            {
                Allocation_SQLDTO? allocationResult = this._context.Allocations.
                      //SingleOrDefault(allocation => allocation.Status > 0 && allocation.StudentId == p_allocation.StudentId && allocation.RepositoryId == p_allocation.RepositoryId);
                      SingleOrDefault(allocation => allocation.Id == p_allocation.Id && allocation.StudentId == p_allocation.StudentId && allocation.RepositoryId == p_allocation.RepositoryId);

                if (allocationResult is not null)
                {
                    allocationResult.StudentId = p_allocation.StudentId;
                    allocationResult.RepositoryId = p_allocation.RepositoryId;
                    allocationResult.Status = p_allocation.Status;

                    // this._context.Update(allocationResult);
                    this._context.SaveChanges();

                    Logging.Instance.Journal(new Log("Allocation", $"DepotAllocation - Method - UpsertAllocation(Allocation p_allocation) - Void - Update Allocation"));
                }
                else
                {
                    Logging.Instance.Journal(new Log("UpsertAllocation - 4 "));
                    Allocation_SQLDTO allocation = new Allocation_SQLDTO();
                    allocation.StudentId = p_allocation.StudentId;
                    allocation.RepositoryId = p_allocation.RepositoryId;
                    allocation.Status = p_allocation.Status; ;

                    this._context.Allocations.Add(allocation);
                    this._context.SaveChanges();

                    Logging.Instance.Journal(new Log("Allocation", $"DepotAllocation - Method - UpsertAllocation(Allocation p_allocation) - Void - Add Allocation"));
                }
            }
            catch (InvalidOperationException e)
            {
                Logging.Instance.Journal(new Log(e.ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "DepotAllocation - UpsertAllocation - _context.Allocations.SingleOrDefault trouve plus qu'un élément", 0));
                throw e;
            }
        }

        public void DeleteAllocation(Allocation p_allocation)
        {
            if (p_allocation is null)
            {
                Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotAllocation - DeleteAllocation - p_allocation passé en paramètre est null", 0));

                throw new ArgumentNullException(nameof(p_allocation));
            }

            try
            {
                Allocation_SQLDTO? allocationResult = this._context.Allocations.
                        //SingleOrDefault(allocation => allocation.Status > 0 && allocation.StudentId == p_allocation.StudentId && allocation.RepositoryId == p_allocation.RepositoryId);
                        SingleOrDefault(allocation => allocation.Id == p_allocation.Id && allocation.StudentId == p_allocation.StudentId && allocation.RepositoryId == p_allocation.RepositoryId);

                if (allocationResult is not null)
                {
                    allocationResult.Status = 0;

                    //this._context.Update(allocationResult);
                    this._context.SaveChanges();

                    Logging.Instance.Journal(new Log("Allocation", $"DepotAllocation - Method - DeleteAllocation(Allocation p_allocation) - Void - delete Allocation"));
                }
                else
                {
                    Logging.Instance.Journal(new Log("Allocation", $"DepotAllocation - Method - DeleteAllocation(Allocation p_allocation) - Void - allocationResult est null", 0));
                }
            }
            catch (InvalidOperationException e)
            {
                Logging.Instance.Journal(new Log(e.ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                        "DepotAllocation - DeleteAllocation - _context.Allocations.SingleOrDefault trouve plus qu'un élément", 0));
                throw e;
            }
        }

        public void UpsertAllocationsBatch(List<Allocation> p_allocations)
        {
            if (p_allocations is null)
            {
                Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotAllocation - UpsertAllocationsBatch - p_allocations passé en paramètre est null", 0));

                throw new ArgumentNullException(nameof(p_allocations));
            }

            foreach (Allocation a in p_allocations)
            {
                if (a is null)
                {
                    Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                         "DepotAllocation - UpsertAllocationsBatch - un des éléments de p_allocations passé en paramètre est null", 0));

                    throw new ArgumentNullException(nameof(a));
                }


                Allocation_SQLDTO? allocationResult = this._context.Allocations.
                       // SingleOrDefault(allocation => allocation.Id == a.Id);
                        SingleOrDefault(allocation => allocation.Id == a.Id && allocation.StudentId == a.StudentId && allocation.RepositoryId == a.RepositoryId);

                if (allocationResult is not null)
                {
                    allocationResult.StudentId = a.StudentId;
                    allocationResult.RepositoryId = a.RepositoryId;
                    allocationResult.Status = a.Status;
                    //this._context.Update(allocationResult);
                    Logging.Instance.Journal(new Log("Allocation", $"DepotAllocation - Method - UpsertAllocationsBatch(List<Allocation> p_allocations) - Void - Update Allocations"));
                }
                else
                {
                    Allocation_SQLDTO allocation = new Allocation_SQLDTO()
                    {
                        Id = a.Id,
                        RepositoryId = a.RepositoryId,
                        StudentId = a.StudentId,
                        TeacherId = a.TeacherId,
                        Status = a.Status
                    };

                    try
                    {
                        this._context.Allocations.Add(allocation);
                    }
                    catch (Exception e)
                    {
                        Logging.Instance.Journal(new Log($"Exception e:{e.Message}"));
                    }


                    Logging.Instance.Journal(new Log("Allocation", $"DepotAllocation - Method - UpsertAllocationsBatch(List<Allocation> p_allocations) - Void - Add Allocations"));
                }
            }

            this._context.SaveChanges();


            if (VerificationOfAllocationInDB(p_allocations))
            {
                SetAllocationAfterVerification(p_allocations);
            }
        }

        private bool VerificationOfAllocationInDB(List<Allocation> p_allocations)
        {
            foreach (Allocation allocation in p_allocations)
            {
                if (!GetAllocations().Select(p => p).Where(m => m.Id == allocation.Id).Any())
                {
                    return false;
                }
            }

            return true;
        }

        private void SetAllocationAfterVerification(List<Allocation> p_allocations)
        {
            Logging.Instance.Journal(new Log($"{p_allocations.Count()}"));

            foreach (Allocation allocation in p_allocations)
            {
                allocation.Status = 2;
                UpsertAllocation(allocation);
            }

        }

        public void SetAllocationAfterCreation(Allocation p_allocation)
        {
            p_allocation.Status = 3;
            UpsertAllocation(p_allocation);
        }

        public List<Allocation> GetSelectedAllocationsByAllocationID(List<Allocation> p_allocations)
        {
            List<Allocation> allocationsResult = new List<Allocation>();

            foreach (Allocation allocation in p_allocations)
            {
                foreach (Allocation allocationDB in GetAllocations())
                {
                    if (allocation.Id == allocationDB.Id)
                    {
                        allocationsResult.Add(allocationDB);
                    }
                }
            }

            return allocationsResult;
        }

        public void DeleteAllocationsBatch(List<Allocation> p_allocations)
        {
            if (p_allocations is null)
            {
                Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotAllocation - DeleteAllocationsBatch - p_allocations passé en paramètre est null", 0));

                throw new ArgumentNullException(nameof(p_allocations));
            }

            foreach (Allocation a in p_allocations)
            {

                if (a is null)
                {
                    Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                         "DepotAllocation - DeleteAllocationsBatch - un des éléments de p_allocations passé en paramètre est null", 0));

                    throw new ArgumentNullException(nameof(a));
                }

                Allocation_SQLDTO? allocationResult = this._context.Allocations.
                        //SingleOrDefault(allocation => allocation.Status > 0 && allocation.StudentId == a.StudentId && allocation.RepositoryId == a.RepositoryId);
                        SingleOrDefault(allocation => allocation.Id == a.Id && allocation.StudentId == a.StudentId && allocation.RepositoryId == a.RepositoryId);

                if (allocationResult is not null)
                {
                    allocationResult.Status = 0;

                    //this._context.Update(allocationResult);

                    Logging.Instance.Journal(new Log("Allocation", $"DepotAllocation - Method - DeleteAllocationsBatch(List<Allocation> p_allocations) - Void - delete Allocations"));
                }
                else
                {
                    Logging.Instance.Journal(new Log("Allocation", $"DepotAllocation - Method - DeleteAllocationsBatch(List<Allocation> p_allocations) - Void - allocationResult est null", 0));
                }
            }

            this._context.SaveChanges();
        }
    }
}
