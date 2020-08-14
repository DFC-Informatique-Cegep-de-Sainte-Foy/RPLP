using Microsoft.EntityFrameworkCore;
using RplpAvecBD.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RplpAvecBD.Data
{
    public class RplpContext : DbContext
    {
        public RplpContext(DbContextOptions<RplpContext> options) : base(options)
        {
            
        }

        public DbSet<Professeur> Professeurs { get; set; }

    }
}
