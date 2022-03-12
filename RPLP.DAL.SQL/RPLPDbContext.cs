using Microsoft.EntityFrameworkCore;
using RPLP.DAL.DTO.Sql;

namespace RPLP.DAL.SQL
{
    public class RPLPDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=rplp.db; Database=RPLP; User Id=sa; password=Cad3pend86!")
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

        public RPLPDbContext()
        {
            Database.Migrate();
        }

        public DbSet<ClassroomDTO> Classrooms { get; set; }
    }
}