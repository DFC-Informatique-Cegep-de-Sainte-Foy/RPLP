﻿using Microsoft.EntityFrameworkCore;
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
    public class DepotTeacher : IDepotTeacher
    {
        private readonly RPLPDbContext _context;

        public DepotTeacher()
        {
            this._context = new RPLPDbContext();
        }

        public DepotTeacher(RPLPDbContext p_context)
        {
            this._context = p_context;
        }

        public List<Teacher> GetTeachers()
        {
            List<Teacher_SQLDTO> teachersResult = this._context.Teachers.Where(teacher => teacher.Active)
                                                                        .Include(teacher => teacher.Classes.Where(classroom => classroom.Active)).ToList();

            List<Teacher> teachers = teachersResult.Select(teacher => teacher.ToEntityWithoutList()).ToList();

            for (int i = 0; i < teachersResult.Count; i++)
            {
                if (teachersResult[i].Id == teachers[i].Id && teachersResult[i].Classes.Count >= 1)
                    teachers[i].Classes = teachersResult[i].Classes.Select(classroom => classroom.ToEntityWithoutList()).ToList();
            }

            return teachers;
        }

        public Teacher GetTeacherByEmail(string p_teacherEmail)
        {
            Teacher_SQLDTO teacherResult = this._context.Teachers
                            .Include(teacher => teacher.Classes.Where(classroom => classroom.Active))
                            .FirstOrDefault(teacher => teacher.Email == p_teacherEmail && teacher.Active);


            if (teacherResult == null)
                return null;

            Teacher teacher = teacherResult.ToEntityWithoutList();

            if (teacherResult.Classes.Count >= 1)
            {
                List<Classroom> classes = teacherResult.Classes.Select(classroom => classroom.ToEntityWithoutList()).ToList();
                teacher.Classes = classes;
            }

            return teacher;
        }

        public Teacher GetTeacherById(int p_id)
        {
            Teacher_SQLDTO teacherResult = this._context.Teachers
                .Include(teacher => teacher.Classes.Where(classroom => classroom.Active))
                .FirstOrDefault(teacher => teacher.Id == p_id && teacher.Active);


            if (teacherResult == null)
                return null;

            Teacher teacher = teacherResult.ToEntityWithoutList();

            if (teacherResult.Classes.Count >= 1)
            {
                List<Classroom> classes = teacherResult.Classes.Select(classroom => classroom.ToEntityWithoutList()).ToList();
                teacher.Classes = classes;
            }

            return teacher;
        }

        public Teacher GetTeacherByUsername(string p_teacherUsername)
        {
            Teacher_SQLDTO teacherResult = this._context.Teachers.Include(teacher => teacher.Classes.Where(classroom => classroom.Active))
                                                                 .FirstOrDefault(teacher => teacher.Username == p_teacherUsername && teacher.Active);
            if (teacherResult == null)
                return null;

            Teacher teacher = teacherResult.ToEntityWithoutList();

            if (teacherResult.Classes.Count >= 1)
            {
                List<Classroom> classes = teacherResult.Classes.Select(classroom => classroom.ToEntityWithoutList()).ToList();
                teacher.Classes = classes;
            }

            return teacher;
        }

        public List<Classroom> GetTeacherClasses(string p_teacherUsername)
        {
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

                    return classes;
                }
            }

            return new List<Classroom>();
        }

        public List<Classroom> GetTeacherClassesInOrganisation(string p_teacherUsername, string p_organisationName)
        {
            List<Classroom> databaseClasses = this._context.Classrooms
                .Where(c =>
                c.Teachers.FirstOrDefault(t => t.Username == p_teacherUsername) != null &&
                c.OrganisationName == p_organisationName &&
                c.Active)
                .Select(c => c.ToEntityWithoutList())
                .ToList();

            return databaseClasses;
        }

        public List<Classroom> GetTeacherClassesInOrganisationByEmail(string p_teacherEmail, string p_organisationName)
        {
            List<Classroom> databaseClasses = this._context.Classrooms
                .Where(c =>
                c.Teachers.FirstOrDefault(t => t.Email == p_teacherEmail) != null &&
                c.OrganisationName == p_organisationName &&
                c.Active)
                .Select(c => c.ToEntityWithoutList())
                .ToList();

            return databaseClasses;
        }

        public List<Organisation> GetTeacherOrganisations(string p_teacherUsername)
        {
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

            return organisations;
        }

        public void AddClassroomToTeacher(string p_teacherUsername, string p_classroomName)
        {
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
                }
            }
        }

        public void RemoveClassroomFromTeacher(string p_teacherUsername, string p_classroomName)
        {
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
                }
            }
        }

        public void UpsertTeacher(Teacher p_teacher)
        {
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
            }
        }

        public void DeleteTeacher(string p_teacherUsername)
        {
            Teacher_SQLDTO teacherResult = this._context.Teachers
                .FirstOrDefault(teacher => teacher.Username == p_teacherUsername && teacher.Active);

            if (teacherResult != null)
            {
                teacherResult.Active = false;

                this._context.Update(teacherResult);
                this._context.SaveChanges();
            }
        }
    }
}
