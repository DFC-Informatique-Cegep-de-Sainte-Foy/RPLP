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
    public class DepotAssignment : IDepotAssignment
    {
        private readonly RPLPDbContext _context;

        public DepotAssignment()
        {
            this._context = new RPLPDbContext(new DbContextOptionsBuilder<RPLPDbContext>().UseSqlServer("Server=rplp.db; Database=RPLP; User Id=sa; password=Cad3pend86!").Options);
            //this._context = new RPLPDbContext(new DbContextOptionsBuilder<RPLPDbContext>().UseSqlServer("Server=localhost,1433; Database=RPLP; User Id=sa; password=Cad3pend86!").Options);
        }

        public DepotAssignment(RPLPDbContext p_context)
        {
            if (p_context == null)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                               "DepotAssignment - DepotAssignment(RPLPDbContext p_context) - p_context de type RPLPDbContext passé en paramètre est null", 0));
            }

            this._context = p_context;
        }

        public List<Assignment> GetAssignments()
        {
            RPLP.JOURNALISATION.Logging.Journal(new Log("Assignments", $"DepotAssignment - Method - GetAssignments() - Return List<Assignment>"));
            
            return this._context.Assignments.Where(assignment => assignment.Active)
                                            .Select(assignment => assignment.ToEntity()).ToList();
        }

        public Assignment GetAssignmentById(int p_id)
        {
            if (p_id < 0)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "DepotAssignment - GetAssignmentById - p_id passé en paramêtre est hors des limites", 0));
            }

            Assignment assignment = this._context.Assignments.FirstOrDefault(assignment => assignment.Id == p_id && assignment.Active)
                                                             .ToEntity();

            if (assignment == null)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log("Assignments", $"DepotAssignment - Method - GetAssignmentById(int p_id) - Return Assignment - assignment est null",0));

                return new Assignment();
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log("Assignments", $"DepotAssignment - Method - GetAssignmentById(int p_id) - Return Assignment - assignment != null"));
            }

            return assignment;
        }

        public Assignment GetAssignmentByName(string p_assignmentName)
        {
            if (string.IsNullOrWhiteSpace(p_assignmentName))
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotAssignment - GetAssignmentByName - p_assignmentName passé en paramètre est vide", 0));
            }

            Assignment assignment = this._context.Assignments
                                                .FirstOrDefault(assignment => assignment.Name == p_assignmentName && assignment.Active)
                                                .ToEntity();

            if (assignment == null)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log("Assignments", $"DepotAssignment - Method - GetAssignmentByName(string p_assignmentName) - Return Assignment - assignment est null",0));

                return new Assignment();
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log("Assignments", $"DepotAssignment - Method - GetAssignmentByName(string p_assignmentName) - Return Assignment - assignment != null"));
            }

            return assignment;
        }

        public List<Assignment> GetAssignmentsByClassroomName(string p_classroomName)
        {
            if (string.IsNullOrWhiteSpace(p_classroomName))
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotAssignment - GetAssignmentsByClassroomName - p_classroomName passé en paramètre est vide", 0));
            }

            List<Assignment> assignments = this._context.Assignments
                .Where(assignment => assignment.ClassroomName == p_classroomName && assignment.Active)
                .Select(s => s.ToEntity())
                .ToList();

            if (assignments == null)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                               "DepotAssignment - GetAssignmentsByClassroomName(string p_classroomName) - assignments est null", 0));
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log("Assignments", $"DepotAssignment - Method - GetAssignmentsByClassroomName(string p_classroomName) - Return List<Assignment> {assignments[0].Name} - assignments != null"));
            }
            
            return assignments;
        }

        public void UpsertAssignment(Assignment p_assignment)
        {
            if (p_assignment == null)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotAssignment - UpsertAssignment - p_assignment passé en paramètre est null", 0));
            }

            Assignment_SQLDTO assignmentResult = this._context.Assignments.Where(assignment => assignment.Active)
                                                                          .FirstOrDefault(assignment => assignment.Id == p_assignment.Id);
            if (assignmentResult != null)
            {
                assignmentResult.Name = p_assignment.Name;
                assignmentResult.ClassroomName = p_assignment.ClassroomName;
                assignmentResult.Description = p_assignment.Description;
                assignmentResult.DeliveryDeadline = p_assignment.DeliveryDeadline;

                this._context.Update(assignmentResult);
                this._context.SaveChanges();

                RPLP.JOURNALISATION.Logging.Journal(new Log("Assignments", $"DepotAssignment - Method - UpsertAssignment(Assignment p_assignment) - Void - Update Assignment"));
            }
            else
            {
                Assignment_SQLDTO assignmentDTO = new Assignment_SQLDTO();
                assignmentDTO.Name = p_assignment.Name;
                assignmentDTO.Description = p_assignment.Description;
                assignmentDTO.ClassroomName = p_assignment.ClassroomName;
                assignmentDTO.DistributionDate = p_assignment.DistributionDate;
                assignmentDTO.DeliveryDeadline = p_assignment.DeliveryDeadline;
                assignmentDTO.Active = true;

                this._context.Assignments.Add(assignmentDTO);
                this._context.SaveChanges();

                RPLP.JOURNALISATION.Logging.Journal(new Log("Assignments", $"DepotAssignment - Method - UpsertAssignment(Assignment p_assignment) - Void - Add Assignment"));
            }
        }

        public void DeleteAssignment(string p_assignmentName)
        {
            if (string.IsNullOrWhiteSpace(p_assignmentName))
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotAssignment - DeleteAssignment - p_assignmentName passé en paramètre est vide", 0));
            }

            Assignment_SQLDTO assignmentResult = this._context.Assignments.Where(assignment => assignment.Active)
                                                                          .FirstOrDefault(assignment => assignment.Name == p_assignmentName);
            if (assignmentResult != null)
            {
                assignmentResult.Active = false;

                this._context.Update(assignmentResult);
                this._context.SaveChanges();

                RPLP.JOURNALISATION.Logging.Journal(new Log("Assignments", $"DepotAssignment - Method - DeleteAssignment(string p_assignmentName) - Void - Delete Assignment"));
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotAssignment - DeleteAssignment(string p_assignmentName) - assignmentResult est null", 0));
            }
        }  
    }
}
