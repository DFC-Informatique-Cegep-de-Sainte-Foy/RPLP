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
    public class DepotTeacher : IDepotTeacher
    {
        private readonly RPLPDbContext _context;

        public DepotTeacher(RPLPDbContext p_context)
        {
            if (p_context == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                               "DepotTeacher - DepotTeacher(RPLPDbContext p_context) - p_context de type RPLPDbContext passé en paramètre est null", 0));
            }

            this._context = p_context;
        }

        public List<Teacher> GetTeachers()
        {
            List<Teacher_SQLDTO> teachersResult = this._context.Teachers.Where(teacher => teacher.Active)
                                                                        .Include(teacher => teacher.Classes.Where(classroom => classroom.Active)).ToList();

            List<Teacher> teachers = teachersResult.Select(teacher => teacher.ToEntityWithoutList()).ToList();

            if (teachers == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                               "DepotTeacher - GetTeachers - la liste teachers de type Teacher assignée à partir de la méthode teachersResult.Select(teacher => teacher.ToEntityWithoutList()).ToList(); est null", 0));
            }

            for (int i = 0; i < teachersResult.Count; i++)
            {
                if (teachersResult[i].Id == teachers[i].Id && teachersResult[i].Classes.Count >= 1)
                    teachers[i].Classes = teachersResult[i].Classes.Select(classroom => classroom.ToEntityWithoutList()).ToList();
            }

            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Teacher", $"DepotTeacher - Method - GetTeachers() - Return List<Teacher> Count:{teachers.Count}"));

            return teachers;
        }

        public List<Teacher> GetDeactivatedTeachers()
        {
            List<Teacher_SQLDTO> teachersResult = this._context.Teachers.Where(teacher => !teacher.Active)
                                                                        .Include(teacher => teacher.Classes.Where(classroom => classroom.Active)).ToList();

            List<Teacher> teachers = teachersResult.Select(teacher => teacher.ToEntityWithoutList()).ToList();

            if (teachers == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                               "DepotTeacher - GetDeactivatedTeachers - la liste teachers de type Teacher assignée à partir de la méthode teachersResult.Select(teacher => teacher.ToEntityWithoutList()).ToList(); est null", 0));
            }

            for (int i = 0; i < teachersResult.Count; i++)
            {
                if (teachersResult[i].Id == teachers[i].Id && teachersResult[i].Classes.Count >= 1)
                    teachers[i].Classes = teachersResult[i].Classes.Select(classroom => classroom.ToEntityWithoutList()).ToList();
            }

            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Teacher", $"DepotTeacher - Method - GetDeactivatedTeachers() - Return List<Teacher>"));

            return teachers;
        }

        public Teacher GetTeacherByEmail(string p_teacherEmail)
        {
            if (string.IsNullOrWhiteSpace(p_teacherEmail))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotTeacher - GetTeacherByEmail - p_teacherEmail passé en paramètre est vide", 0));
            }

            Teacher_SQLDTO teacherResult = this._context.Teachers
                            .Include(teacher => teacher.Classes.Where(classroom => classroom.Active))
                            .FirstOrDefault(teacher => teacher.Email == p_teacherEmail && teacher.Active);

            

            if (teacherResult == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Teacher - Classroom", $"DepotTeacher - Method - GetTeacherByEmail(string p_teacherEmail) - Return Teacher - teacherResult est null",0));

                return null;
            }
                
            Teacher teacher = teacherResult.ToEntityWithoutList();

            if (teacherResult.Classes.Count >= 1)
            {
                List<Classroom> classes = teacherResult.Classes.Select(classroom => classroom.ToEntityWithoutList()).ToList();
                teacher.Classes = classes;

                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Teacher - Classroom", $"DepotTeacher - Method - GetTeacherByEmail(string p_teacherEmail) - Return Teacher - teacherResult.Classes.Count >= 1"));
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Teacher - Classroom", $"DepotTeacher - Method - GetTeacherByEmail(string p_teacherEmail) - Return Teacher - teacherResult.Classes.Count liste vide",0));
            }
            return teacher;
        }

        public Teacher GetTeacherById(int p_id)
        {
            if (p_id < 0)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "DepotTeacher - GetTeacherById - p_id passé en paramêtre est hors des limites", 0));
            }

            Teacher_SQLDTO teacherResult = this._context.Teachers
                .Include(teacher => teacher.Classes.Where(classroom => classroom.Active))
                .FirstOrDefault(teacher => teacher.Id == p_id && teacher.Active);

            if (teacherResult == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Teacher - Classroom", $"DepotTeacher - Method - GetTeacherById(int p_id) - Return Teacher - teacherResult est null",0));

                return null;
            }
               
            Teacher teacher = teacherResult.ToEntityWithoutList();

            if (teacherResult.Classes.Count >= 1)
            {
                List<Classroom> classes = teacherResult.Classes.Select(classroom => classroom.ToEntityWithoutList()).ToList();
                teacher.Classes = classes;

                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Teacher - Classroom", $"DepotTeacher - Method - GetTeacherById(int p_id) - Return Teacher - teacherResult.Classes.Count >= 1"));
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Teacher - Classroom", $"DepotTeacher - Method - GetTeacherById(int p_id) - Return Teacher - teacherResult.Classes.Count est vide", 0));
            }
            return teacher;
        }

        public Teacher GetTeacherByUsername(string p_teacherUsername)
        {
            if (string.IsNullOrWhiteSpace(p_teacherUsername))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotTeacher - GetTeacherByUsername - p_teacherUsername passé en paramètre est vide", 0));
            }

            Teacher_SQLDTO teacherResult = this._context.Teachers.Include(teacher => teacher.Classes.Where(classroom => classroom.Active))
                                                                 .FirstOrDefault(teacher => teacher.Username == p_teacherUsername && teacher.Active);

            if (teacherResult == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Teacher - Classroom", $"DepotTeacher - Method - GetTeacherByUsername(string p_teacherUsername) - Return Teacher - teacherResult est null",0));
                return null;
            }
                
            Teacher teacher = teacherResult.ToEntityWithoutList();

            if (teacherResult.Classes.Count >= 1)
            {
                List<Classroom> classes = teacherResult.Classes.Select(classroom => classroom.ToEntityWithoutList()).ToList();
                teacher.Classes = classes;

                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Teacher - Classroom", $"DepotTeacher - Method - GetTeacherByUsername(string p_teacherUsername) - Return Teacher - teacherResult.Classes.Count >= 1"));
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Teacher - Classroom", $"DepotTeacher - Method - GetTeacherByUsername(string p_teacherUsername) - Return Teacher - teacherResult.Classes.Count est vide",0));
            }

            return teacher;
        }

        public List<Classroom> GetTeacherClasses(string p_teacherUsername)
        {
            if (string.IsNullOrWhiteSpace(p_teacherUsername))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotTeacher - GetTeacherClasses - p_teacherUsername passé en paramètre est vide", 0));
            }

            List<Classroom> classes = new List<Classroom>();
            Teacher_SQLDTO teacher = this._context.Teachers.Include(teacher => teacher.Classes.Where(classroom => classroom.Active))
                                                           .FirstOrDefault(teacher => teacher.Username == p_teacherUsername && teacher.Active);


            if (teacher != null)
            {
                if (teacher.Classes.Count >= 1)
                {
                    foreach (Classroom_SQLDTO classroom in teacher.Classes)
                    {
                        classes.Add(classroom.ToEntityWithoutList());
                    }
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Teacher - Classroom", $"DepotTeacher - Method - GetTeacherClasses(string p_teacherUsername) - Return List<Classroom> - teacher.Classes.Count >= 1"));

                    return classes;
                }
                else
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Teacher - Classroom", $"DepotTeacher - Method - GetTeacherClasses(string p_teacherUsername) - Return List<Classroom> - teacher.Classes.Count est vide",0));
                }
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Teacher - Classroom", $"DepotTeacher - Method - GetTeacherClasses(string p_teacherUsername) - Return List<Classroom> - teacher est null",0));
            }

            return new List<Classroom>();
        }

        public List<Classroom> GetTeacherClassesInOrganisation(string p_teacherUsername, string p_organisationName)
        {
            if (string.IsNullOrWhiteSpace(p_teacherUsername))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotTeacher - GetTeacherClassesInOrganisation - p_teacherUsername passé en paramètre est vide", 0));
            }
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotTeacher - GetTeacherClassesInOrganisation - p_organisationName passé en paramètre est vide", 0));
            }

            List<Classroom> databaseClasses = this._context.Classrooms
                .Where(c =>
                c.Teachers.FirstOrDefault(t => t.Username == p_teacherUsername) != null &&
                c.OrganisationName == p_organisationName &&
                c.Active)
                .Select(c => c.ToEntityWithoutList())
                .ToList();

            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Teacher - Classroom", $"DepotTeacher - Method - GetTeacherClassesInOrganisation(string p_teacherUsername, string p_organisationName) - Return List<Classroom>"));

            return databaseClasses;
        }

        public List<Classroom> GetTeacherClassesInOrganisationByEmail(string p_teacherEmail, string p_organisationName)
        {
            if (string.IsNullOrWhiteSpace(p_teacherEmail))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotTeacher - GetTeacherClassesInOrganisationByEmail - p_teacherEmail passé en paramètre est vide", 0));
            }
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotTeacher - GetTeacherClassesInOrganisationByEmail - p_organisationName passé en paramètre est vide", 0));
            }

            List<Classroom> databaseClasses = this._context.Classrooms
                .Where(c =>
                c.Teachers.FirstOrDefault(t => t.Email == p_teacherEmail) != null &&
                c.OrganisationName == p_organisationName &&
                c.Active)
                .Select(c => c.ToEntityWithoutList())
                .ToList();

            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Teacher - Classroom", $"DepotTeacher - Method - GetTeacherClassesInOrganisationByEmail(string p_teacherEmail, string p_organisationName) - Return List<Classroom>"));

            return databaseClasses;
        }

        public List<Organisation> GetTeacherOrganisations(string p_teacherUsername)
        {
            if (string.IsNullOrWhiteSpace(p_teacherUsername))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotTeacher - GetTeacherOrganisations - p_teacherUsername passé en paramètre est vide", 0));
            }

            List<Organisation> organisations = new List<Organisation>();
            List<Classroom_SQLDTO> classrooms = new List<Classroom_SQLDTO>();

            Teacher_SQLDTO? teacher = this._context.Teachers.Include(teacher => teacher.Classes.Where(classroom => classroom.Active))
                                                           .FirstOrDefault(teacher => teacher.Username == p_teacherUsername && teacher.Active);

            classrooms = teacher.Classes;

            HashSet<string> organisationNames = classrooms.Select(c => c.OrganisationName).ToHashSet();

            foreach (string organisationName in organisationNames)
            {
                Organisation_SQLDTO? organisationToAdd = this._context.Organisations.FirstOrDefault(o => o.Name == organisationName);

                if (organisationToAdd != null)
                {
                    organisations.Add(organisationToAdd.ToEntity());
                }
            }

            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Teacher - Classroom - Organisation", $"DepotTeacher - Method - GetTeacherOrganisations(string p_teacherUsername) - Return List<Organisation>"));

            return organisations;
        }

        public void AddClassroomToTeacher(string p_teacherUsername, string p_classroomName)
        {
            if (string.IsNullOrWhiteSpace(p_teacherUsername))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotTeacher - AddClassroomToTeacher - p_teacherUsername passé en paramètre est vide", 0));
            }
            if (string.IsNullOrWhiteSpace(p_classroomName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotTeacher - AddClassroomToTeacher - p_classroomName passé en paramètre est vide", 0));
            }

            Classroom_SQLDTO classroomResult = this._context.Classrooms
                .FirstOrDefault(classroom => classroom.Name == p_classroomName && classroom.Active);

            if (classroomResult != null)
            {
                Teacher_SQLDTO teacherResult = this._context.Teachers
                    .SingleOrDefault(teacher => teacher.Username == p_teacherUsername && teacher.Active);
                if (teacherResult != null)
                {
                    teacherResult.Classes.Add(classroomResult);

                    this._context.Update(teacherResult);
                    this._context.SaveChanges();

                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Teacher - Classroom", $"DepotTeacher - Method - AddClassroomToTeacher(string p_teacherUsername, string p_classroomName) - Void - add classroom teacher"));
                }
                else
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Teacher - Classroom", $"DepotTeacher - Method - AddClassroomToTeacher(string p_teacherUsername, string p_classroomName) - Void - teacherResult est null",0));
                }
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Teacher - Classroom", $"DepotTeacher - Method - AddClassroomToTeacher(string p_teacherUsername, string p_classroomName) - Void - classroomResult est null", 0));
            }
        }

        public void RemoveClassroomFromTeacher(string p_teacherUsername, string p_classroomName)
        {
            if (string.IsNullOrWhiteSpace(p_teacherUsername))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotTeacher - RemoveClassroomFromTeacher - p_teacherUsername passé en paramètre est vide", 0));
            }
            if (string.IsNullOrWhiteSpace(p_classroomName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotTeacher - RemoveClassroomFromTeacher - p_classroomName passé en paramètre est vide", 0));
            }

            Classroom_SQLDTO classroomResult = this._context.Classrooms
                .FirstOrDefault(classroom => classroom.Name == p_classroomName && classroom.Active);
            if (classroomResult != null)
            {
                Teacher_SQLDTO teacherResult = this._context.Teachers
                    .Include(t => t.Classes)
                    .SingleOrDefault(teacher => teacher.Username == p_teacherUsername && teacher.Active);

                if (teacherResult != null)
                {
                    teacherResult.Classes.Remove(classroomResult);
                    this._context.Update(teacherResult);
                    this._context.SaveChanges();

                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Teacher - Classroom", $"DepotTeacher - Method - RemoveClassroomFromTeacher(string p_teacherUsername, string p_classroomName) - Void - remove classroom from teacher"));
                }
                else
                {
                    RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Teacher - Classroom", $"DepotTeacher - Method - RemoveClassroomFromTeacher(string p_teacherUsername, string p_classroomName) - Void - teacherResult est null",0));
                }
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Teacher - Classroom", $"DepotTeacher - Method - RemoveClassroomFromTeacher(string p_teacherUsername, string p_classroomName) - Void - classroomResult est null", 0));
            }
        }

        public void UpsertTeacher(Teacher p_teacher)
        {
            if(p_teacher == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotTeacher - UpsertTeacher - p_teacher passé en paramètre est null", 0));
            }

            List<Classroom_SQLDTO> classes = new List<Classroom_SQLDTO>();
            VerificatorForDepot verificator = new VerificatorForDepot(this._context);

            if (p_teacher.Classes.Count >= 1)
            {
                foreach (Classroom classroom in p_teacher.Classes)
                {
                    classes.Add(new Classroom_SQLDTO(classroom));
                }
            }

            Teacher_SQLDTO teacherResult = this._context.Teachers
                .AsNoTracking()
                .SingleOrDefault(teacher => teacher.Id == p_teacher.Id && teacher.Active);

            if (teacherResult != null)
            {
                if (!teacherResult.Active)
                {
                    throw new ArgumentException("Deleted accounts cannot be updated.");
                }

                teacherResult.Username = p_teacher.Username;
                teacherResult.FirstName = p_teacher.FirstName;
                teacherResult.LastName = p_teacher.LastName;
                teacherResult.Email = p_teacher.Email;

                this._context.Update(teacherResult);
                this._context.SaveChanges();

                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Teacher - Classroom", $"DepotTeacher - Method - UpsertTeacher(Teacher p_teacher) - Void - Update Teacher"));
            }
            else
            {
                if ((teacherResult != null && teacherResult.Username != p_teacher.Username &&
                verificator.CheckUsernameTaken(p_teacher.Username)) ||
                teacherResult == null && verificator.CheckUsernameTaken(p_teacher.Username))
                {
                    throw new ArgumentException("Username already taken.");
                }

                if ((teacherResult != null && teacherResult.Email != p_teacher.Email && verificator.CheckEmailTaken(p_teacher.Email)) ||
                    teacherResult == null && verificator.CheckEmailTaken(p_teacher.Email))
                {
                    throw new ArgumentException("Email already in use.");
                }

                Teacher_SQLDTO teacher = new Teacher_SQLDTO();
                teacher.Username = p_teacher.Username;
                teacher.FirstName = p_teacher.FirstName;
                teacher.LastName = p_teacher.LastName;
                teacher.Email = p_teacher.Email;
                teacher.Classes = classes;
                teacher.Active = true;

                this._context.Teachers.Add(teacher);
                this._context.SaveChanges();

                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Teacher - Classroom", $"DepotTeacher - Method - UpsertTeacher(Teacher p_teacher) - Void - Add Teacher"));
            }
        }

        public void DeleteTeacher(string p_teacherUsername)
        {
            if (string.IsNullOrWhiteSpace(p_teacherUsername))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotTeacher - DeleteTeacher - p_teacherUsername passé en paramètre est vide", 0));
            }

            Teacher_SQLDTO teacherResult = this._context.Teachers
                .FirstOrDefault(teacher => teacher.Username == p_teacherUsername && teacher.Active);

            if (teacherResult != null)
            {
                teacherResult.Active = false;

                this._context.Update(teacherResult);
                this._context.SaveChanges();

                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Teacher", $"DepotTeacher - Method - DeleteTeacher(string p_teacherUsername) - Void - delete teacher"));
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Teacher", $"DepotTeacher - Method - DeleteTeacher(string p_teacherUsername) - Void - teacherResult est null",0));
            }
        }

        public void ReactivateTeacher(string p_teacherUsername)
        {
            if (string.IsNullOrWhiteSpace(p_teacherUsername))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotTeacher - ReactivateTeacher - p_teacherUsername passé en paramètre est vide", 0));
            }

            Teacher_SQLDTO teacherResult = this._context.Teachers
               .FirstOrDefault(teacher => teacher.Username == p_teacherUsername && !teacher.Active);

            if (teacherResult != null)
            {
                teacherResult.Active = true;

                this._context.Update(teacherResult);
                this._context.SaveChanges();

                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Teacher", $"DepotTeacher - Method - ReactivateTeacher(string p_teacherUsername) - Void - reactive teacher"));
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Teacher", $"DepotTeacher - Method - ReactivateTeacher(string p_teacherUsername) - Void - teacherResult est null",0));
            }
        }
    }
}
