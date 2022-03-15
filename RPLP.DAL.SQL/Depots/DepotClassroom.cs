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
    public class DepotClassroom : IDepotClassroom
    {
        private readonly RPLPDbContext _context;

        public DepotClassroom()
        {
            this._context = new RPLPDbContext();
        }

        public Classroom GetClassroomById(int p_id)
        {
            return this._context.Classrooms.Where(classroom => classroom.Id == p_id).Select(classroom => classroom.ToEntity()).FirstOrDefault();
        }

        public Classroom GetClassroomByName(string p_name)
        {
            return this._context.Classrooms.Where(classroom => classroom.Name == p_name).Select(classroom => classroom.ToEntity()).FirstOrDefault();
        }

        public List<Classroom> GetClassrooms()
        {
            return this._context.Classrooms.Select(classroom => classroom.ToEntity()).ToList();
        }

        public void UpsertClassroom(Classroom p_classroom)
        {
            Classroom classroomResult = this._context.Classrooms.Where(classroom => classroom.Id == p_classroom.Id).Select(classroom => classroom.ToEntity()).FirstOrDefault();

            if (classroomResult != null)
            {
                classroomResult.Name = p_classroom.Name;
                this._context.Update(classroomResult);
                this._context.SaveChanges();
            }
            else
            {
                Classroom_SQLDTO classDTO = new Classroom_SQLDTO();
                classDTO.Name = p_classroom.Name;

                this._context.Classrooms.Add(classDTO);
                this._context.SaveChanges();
            }
        }
    }
}
