using Microsoft.EntityFrameworkCore;
using RplpAvecBD.Model;

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
