﻿using RPLP.ENTITES;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.DAL.DTO.Sql
{
    public class Classroom_SQLDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrganisationId { get; set; }
        
        [ForeignKey("OrganisationId")]
        public Organisation_SQLDTO Organisation { get; set; }
        public List<Student_SQLDTO> Students { get; set; }
        public List<Teacher_SQLDTO> Teachers { get; set; }
        public List<Assignment_SQLDTO> Assignments { get; set; }
        public bool Active { get; set; }

        public Classroom_SQLDTO()
        {
            this.Students = new List<Student_SQLDTO>();
            this.Teachers = new List<Teacher_SQLDTO>();
            this.Assignments = new List<Assignment_SQLDTO>();
            this.Organisation = new Organisation_SQLDTO();
        }

        public Classroom_SQLDTO(Classroom classroom)
        {
            List<Student_SQLDTO> students = new List<Student_SQLDTO>();
            List<Teacher_SQLDTO> teachers = new List<Teacher_SQLDTO>();
            List<Assignment_SQLDTO> assignments = new List<Assignment_SQLDTO>();

            this.Id = classroom.Id;
            this.Name = classroom.Name;
            this.Organisation = new Organisation_SQLDTO(classroom.Organisation);

            if (classroom.Students.Count >= 1)
            {
                foreach (Student student in classroom.Students)
                {
                    students.Add(new Student_SQLDTO(student));
                }
            }
            if (classroom.Teachers.Count >= 1)
            {
                foreach (Teacher teacher in classroom.Teachers)
                {
                    teachers.Add(new Teacher_SQLDTO(teacher));
                }
            }
            if (classroom.Assignments.Count >= 1)
            {
                foreach (Assignment assignment in classroom.Assignments)
                {
                    assignments.Add(new Assignment_SQLDTO(assignment));
                }
            }

            this.Students = students;
            this.Teachers = teachers;
            this.Assignments = assignments;

            if (this.Assignments.Count > 0)
            {
                foreach (Assignment_SQLDTO assignment in this.Assignments)
                {
                    assignment.Classroom = this;
                }
            }
            this.Active = true;
        }

        public Classroom ToEntity()
        {
            return new Classroom(this.Id,
                this.Name,
                this.Organisation.ToEntity(),
                this.Students.Select(student => student.ToEntity()).ToList(),
                this.Teachers.Select(teacher => teacher.ToEntity()).ToList(),
                this.Assignments.Select(assignment => assignment.ToEntity()).ToList());
        }

        public Classroom ToEntityWithoutList()
        {
            return new Classroom(this.Id, this.Name, this.Organisation.ToEntity(), new List<Student>(), new List<Teacher>(), new List<Assignment>());
        }
    }
}
