﻿using RPLP.ENTITES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace RPLP.DAL.DTO.Sql
{
    [Index("Username","Email", IsUnique = true)]
    public class Student_SQLDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Matricule { get; set; }
        public List<Classroom_SQLDTO> Classes { get; set; }

        public bool IsTutor { get; set; }
        public bool Active { get; set; }

        public Student_SQLDTO()
        {
            this.Classes = new List<Classroom_SQLDTO>();
        }

        public Student_SQLDTO(Student p_student)
        {
            List<Classroom_SQLDTO> classes = new List<Classroom_SQLDTO>();

            this.Id = p_student.Id;
            this.Username = p_student.Username;
            this.FirstName = p_student.FirstName;
            this.LastName = p_student.LastName;
            this.Matricule = p_student.Matricule;
            this.IsTutor = p_student.IsTutor;
            this.Email = p_student.Email;

            if (p_student.Classes.Count >= 1)
            {
                foreach (Classroom classroom in p_student.Classes)
                {
                    classes.Add(new Classroom_SQLDTO(classroom));
                }
            }

            this.Classes = classes;
            this.Active = true;
        }

        public Student ToEntity()
        {
            return new Student(this.Id, this.Username, this.FirstName, this.LastName, this.Email, this.Matricule, this.IsTutor, this.Classes.Select(classroom => classroom.ToEntity()).ToList());
        }

        public Student ToEntityWithoutList()
        {
            return new Student(this.Id, this.Username, this.FirstName, this.LastName, this.Email, this.Matricule, this.IsTutor,new List<Classroom>());
        }
    }
}
