using Microsoft.EntityFrameworkCore;
using RPLP.DAL.DTO.Sql;
using RPLP.ENTITES;
using RPLP.SERVICES.InterfacesDepots;
using System;
using System.Collections.Generic;
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
            this._context = p_context;
        }

        public List<Assignment> GetAssignments()
        {
            return this._context.Assignments.Where(assignment => assignment.Active)
                                            .Select(assignment => assignment.ToEntity()).ToList();
        }

        public Assignment GetAssignmentById(int p_id)
        {
            Assignment assignment = this._context.Assignments.FirstOrDefault(assignment => assignment.Id == p_id && assignment.Active)
                                                             .ToEntity();

            if (assignment == null)
                return new Assignment();

            return assignment;
        }

        public Assignment GetAssignmentByName(string p_assignmentName)
        {
            Assignment assignment = this._context.Assignments
                                                .FirstOrDefault(assignment => assignment.Name == p_assignmentName && assignment.Active)
                                                .ToEntity();

            if (assignment == null)
                return new Assignment();

            return assignment;
        }

        public List<Assignment> GetAssignmentsByClassroomName(string p_classroomName)
        {
            List<Assignment> assignments = this._context.Assignments
                .Where(assignment => assignment.ClassroomName == p_classroomName && assignment.Active)
                .Select(s => s.ToEntity())
                .ToList();

            return assignments;
        }

        public void UpsertAssignment(Assignment p_assignment)
        {
            Assignment_SQLDTO assignmentResult = this._context.Assignments.Where(assignment => assignment.Active)
                                                                          .FirstOrDefault(assignment => assignment.Id == p_assignment.Id);
            if (assignmentResult != null)
            {
                assignmentResult.Name = p_assignment.Name;
                assignmentResult.Description = p_assignment.Description;
                assignmentResult.DeliveryDeadline = p_assignment.DeliveryDeadline;

                this._context.Update(assignmentResult);
                this._context.SaveChanges();
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
            }
        }

        public void DeleteAssignment(string p_assignmentName)
        {
            Assignment_SQLDTO assignmentResult = this._context.Assignments.Where(assignment => assignment.Active)
                                                                          .FirstOrDefault(assignment => assignment.Name == p_assignmentName);
            if (assignmentResult != null)
            {
                assignmentResult.Active = false;

                this._context.Update(assignmentResult);
                this._context.SaveChanges();
            }
        }
    }
}
