﻿using RPLP.ENTITES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.SERVICES.InterfacesDepots
{
    public interface IDepotTeacher
    {
        public List<Teacher> GetTeachers();
        public Teacher GetTeacherById(int p_id);
        public Teacher GetTeacherByName(string p_teacherUsername);
        public List<Classroom> GetTeacherClasses(string p_teacherUsername);
        public void UpsertTeacher(Teacher p_teacher);
        public void DeleteTeacher(string p_teacherUsername);
    }
}
