using Microsoft.EntityFrameworkCore;
using RPLP.DAL.DTO.Sql;
using RPLP.ENTITES;
using RPLP.JOURNALISATION;
using RPLP.ENTITES.InterfacesDepots;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.DAL.SQL.Depots
{
    public class DepotAssignment : IDepotAssignment
    {
        private readonly RPLPDbContext _context;

        public DepotAssignment(RPLPDbContext p_context)
        {
            if (p_context == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                               "DepotAssignment - DepotAssignment(RPLPDbContext p_context) - p_context de type RPLPDbContext passé en paramètre est null", 0));
            }

            this._context = p_context;
        }

        public List<Assignment> GetAssignments()
        {
            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Assignments", $"DepotAssignment - Method - GetAssignments() - Return List<Assignment>"));
            List<Assignment_SQLDTO> assigmentsInDb =
                this._context.Assignments.AsNoTracking().Where(assignment => assignment.Active).ToList();
            assigmentsInDb.ForEach(a=>a.Classroom = this._context.Classrooms.AsNoTracking().SingleOrDefault(cl => cl.Id == a.ClassroomId));
            
            return assigmentsInDb.Select(assignment => assignment.ToEntity()).ToList();
        }
        
        public List<Assignment> GetAssignmentsInactives()
        {
            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Assignments", $"DepotAssignment - Method - GetAssignments() - Return List<Assignment>"));
            List<Assignment_SQLDTO> assigmentsInDb =
                this._context.Assignments.AsNoTrackingWithIdentityResolution().Where(assignment => !assignment.Active).ToList();
            assigmentsInDb.ForEach(a=>a.Classroom = this._context.Classrooms
                .AsNoTrackingWithIdentityResolution()
                .SingleOrDefault(cl => cl.Id == a.ClassroomId));
            
            return assigmentsInDb.Select(assignment => assignment.ToEntity()).ToList();
        }

        public Assignment GetAssignmentById(int p_id)
        {
            if (p_id < 0)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "DepotAssignment - GetAssignmentById - p_id passé en paramêtre est hors des limites", 0));
            }

            Assignment assignment = this._context.Assignments.AsNoTracking().SingleOrDefault(assignment => assignment.Id == p_id && assignment.Active)
                                                             .ToEntity();

            if (assignment == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Assignments", $"DepotAssignment - Method - GetAssignmentById(int p_id) - Return Assignment - assignment est null",0));

                return new Assignment();
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Assignments", $"DepotAssignment - Method - GetAssignmentById(int p_id) - Return Assignment - assignment != null"));
            }

            return assignment;
        }

        public Assignment GetAssignmentByName(string p_assignmentName)
        {
            if (string.IsNullOrWhiteSpace(p_assignmentName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotAssignment - GetAssignmentByName - p_assignmentName passé en paramètre est vide", 0));
            }

            Assignment assignment = this._context.Assignments
                                                .AsNoTracking().SingleOrDefault(assignment => assignment.Name == p_assignmentName && assignment.Active)
                                                .ToEntity();

            if (assignment == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Assignments", $"DepotAssignment - Method - GetAssignmentByName(string p_assignmentName) - Return Assignment - assignment est null",0));

                return new Assignment();
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Assignments", $"DepotAssignment - Method - GetAssignmentByName(string p_assignmentName) - Return Assignment - assignment != null"));
            }

            return assignment;
        }

        public List<Assignment> GetAssignmentsByClassroomName(string p_classroomName)
        {
            if (string.IsNullOrWhiteSpace(p_classroomName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotAssignment - GetAssignmentsByClassroomName - p_classroomName passé en paramètre est vide", 0));
            }

            int classroomId = this._context.Classrooms.AsNoTracking().SingleOrDefault(c => c.Name == p_classroomName).Id;

            List<Assignment_SQLDTO> assignmentsDTO = this._context.Assignments
                .Where(assignment => assignment.Classroom.Id == classroomId && assignment.Active)
                .ToList();
            assignmentsDTO.ForEach(ass=>ass.Classroom = this._context.Classrooms.AsNoTracking().SingleOrDefault(cl=>cl.Id==classroomId));

            List<Assignment> assignments = assignmentsDTO.Select(ass => ass.ToEntity()).ToList();

            if (assignments == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                               "DepotAssignment - GetAssignmentsByClassroomName(string p_classroomName) - assignments est null", 0));
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Assignments", $"DepotAssignment - Method - GetAssignmentsByClassroomName(string p_classroomName) - Return List<Assignment> COunt: {assignments.Count} - assignments != null"));
            }
            
            return assignments;
        }

        public void UpsertAssignment(Assignment p_assignment)
        {
            if (this._context.ChangeTracker != null)
            {
                this._context.ChangeTracker.Clear();
            }
            
            if (p_assignment == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotAssignment - UpsertAssignment - p_assignment passé en paramètre est null", 0));
            }

            Assignment_SQLDTO assignmentResult = this._context.Assignments
                .AsNoTrackingWithIdentityResolution()
                .Where(assignment => assignment.Active)
                .SingleOrDefault(assignment => assignment.Id == p_assignment.Id);
            
            if (assignmentResult != null)
            {
                assignmentResult.Name = p_assignment.Name;
                assignmentResult.Classroom = new Classroom_SQLDTO(p_assignment.Classroom);
                assignmentResult.Description = p_assignment.Description;
                assignmentResult.DeliveryDeadline = p_assignment.DeliveryDeadline;

                if (this._context.ChangeTracker != null)
                {
                    this._context.Entry<Classroom_SQLDTO>(assignmentResult.Classroom).State = EntityState.Unchanged;
                }

                this._context.Update(assignmentResult);
                this._context.SaveChanges();

                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Assignments", $"DepotAssignment - Method - UpsertAssignment(Assignment p_assignment) - Void - Update Assignment"));
            }
            else
            {
                Assignment_SQLDTO assignmentDTO = new Assignment_SQLDTO();
                assignmentDTO.Name = p_assignment.Name;
                assignmentDTO.Description = p_assignment.Description;
                assignmentDTO.Classroom = new Classroom_SQLDTO(p_assignment.Classroom);
                assignmentDTO.DistributionDate = p_assignment.DistributionDate;
                assignmentDTO.DeliveryDeadline = p_assignment.DeliveryDeadline;
                assignmentDTO.Active = true;

                if (this._context.ChangeTracker != null)
                {
                    this._context.Entry<Classroom_SQLDTO>(assignmentDTO.Classroom).State = EntityState.Unchanged;
                }

                this._context.Assignments.Add(assignmentDTO);
                this._context.SaveChanges();

                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Assignments", $"DepotAssignment - Method - UpsertAssignment(Assignment p_assignment) - Void - Add Assignment"));
            }
        }

        public void DeleteAssignment(string p_assignmentName)
        {
            if (string.IsNullOrWhiteSpace(p_assignmentName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotAssignment - DeleteAssignment - p_assignmentName passé en paramètre est vide", 0));
            }

            Assignment_SQLDTO assignmentResult = this._context.Assignments
                .AsNoTrackingWithIdentityResolution()
                .Where(assignment => assignment.Active)
                .SingleOrDefault(assignment => assignment.Name == p_assignmentName);
            
            if (assignmentResult != null)
            {
                
                assignmentResult.Classroom =
                    this._context.Classrooms.AsNoTracking().SingleOrDefault(c => c.Id == assignmentResult.ClassroomId);
                
                assignmentResult.Active = false;

                if (this._context.ChangeTracker != null)
                {
                    this._context.Entry<Classroom_SQLDTO>(assignmentResult.Classroom).State = EntityState.Unchanged;
                }

                this._context.Update(assignmentResult);
                this._context.SaveChanges();

                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Assignments", $"DepotAssignment - Method - DeleteAssignment(string p_assignmentName) - Void - Delete Assignment"));
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotAssignment - DeleteAssignment(string p_assignmentName) - assignmentResult est null", 0));
            }
        }

        public void ReactivateAssignment(string p_assignmentName, int p_assignmentId)
        {
            if (this._context.ChangeTracker != null)
            {
                this._context.ChangeTracker.Clear();
            }
            
            if (string.IsNullOrWhiteSpace(p_assignmentName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotAssignment - ReactivateAssignment - assignmentName passé en paramêtre est invalide", 0));
            }
                
            if (p_assignmentId <=0)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotAssignment - ReactivateAssignment - assignmentId passé en paramêtre est invalide", 0));
            }
            
            Assignment_SQLDTO assignmentResult = this._context.Assignments
                .AsNoTrackingWithIdentityResolution()
                .Where(ass => !ass.Active)
                .FirstOrDefault(assignment => assignment.Id == p_assignmentId && assignment.Name == p_assignmentName);
            
            if (assignmentResult != null)
            {
                assignmentResult.Classroom =
                    this._context.Classrooms.AsNoTracking().SingleOrDefault(c => c.Id == assignmentResult.ClassroomId);
                
                assignmentResult.Active = true;
                this._context.Entry<Classroom_SQLDTO>(assignmentResult.Classroom).State = EntityState.Unchanged;

                this._context.Update(assignmentResult);
                this._context.SaveChanges();

                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Assignments", $"DepotAssignment - Method - ReactivateAssignment(Assignment p_assignment) - Void - Reactivate Assignment"));
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotAssignment - ReactivateAssignment(Assignment p_assignment) - assignmentResult est null", 0));
            }
            
        }
    }
}
