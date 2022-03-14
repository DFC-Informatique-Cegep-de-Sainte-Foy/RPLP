﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.ENTITES
{
    public class Teacher
    {
        public int id { get; set; }
        public string username { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public List<Classroom> classes { get; set; }

        public Teacher()
        {
            this.classes = new List<Classroom>();
        }
    }
}
