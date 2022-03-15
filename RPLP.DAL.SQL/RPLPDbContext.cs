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

        public DbSet<Classroom_SQLDTO> Classrooms { get; set; }
        public DbSet<Administrator_SQLDTO> Administrators { get; set; }
        public DbSet<Assignment_SQLDTO> Assignments { get; set; }
        public DbSet<Comment_SQLDTO> Comments { get; set; }
        public DbSet<Organisation_SQLDTO> Organisations { get; set; }
        public DbSet<Repository_SQLDTO> Repositories { get; set; }
        public DbSet<Student_SQLDTO> Students { get; set; }
        public DbSet<Teacher_SQLDTO> Teachers { get; set; }
    }
}