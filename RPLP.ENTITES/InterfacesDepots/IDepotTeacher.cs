﻿using RPLP.ENTITES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.ENTITES.InterfacesDepots
{
    public interface IDepotTeacher
    {
        public List<Teacher> GetTeachers();
        public List<Teacher> GetDeactivatedTeachers();
        public Teacher GetTeacherById(int p_id);
        public Teacher GetTeacherByEmail(string p_teacherEmail);
        public Teacher GetTeacherByUsername(string p_teacherUsername);
        public List<Organisation> GetTeacherOrganisations(string p_teacherUsername);
        public List<Classroom> GetTeacherClassesInOrganisation(string p_teacherUsername, string p_organisationName);
        public List<Classroom> GetTeacherClassesInOrganisationByEmail(string p_teacherEmail, string p_organisationName);
        public List<Classroom> GetTeacherClasses(string p_teacherUsername);
        public void UpsertTeacher(Teacher p_teacher);
        public void DeleteTeacher(string p_teacherUsername);
        public void ReactivateTeacher(string p_teacherUsername);
    }
}
