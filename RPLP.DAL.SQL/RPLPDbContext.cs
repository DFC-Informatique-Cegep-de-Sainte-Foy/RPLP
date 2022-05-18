using Microsoft.EntityFrameworkCore;
using RPLP.DAL.DTO.Sql;

namespace RPLP.DAL.SQL
{
    public class RPLPDbContext : DbContext
    {
        public RPLPDbContext()
        {
            Database.Migrate();
        }

        public RPLPDbContext(DbContextOptions<RPLPDbContext> options) :
            base(options)
        {
            Database.Migrate();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=rplp.db; Database=RPLP; User Id=sa; password=Cad3pend86!");
                //optionsBuilder.UseSqlServer("Server=localhost,1433;Database=RPLP;User Id=sa;password=Cad3pend86!");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Administrator_SQLDTO>()
                .HasIndex(a => a.Email)
                .IsUnique();

            modelBuilder.Entity<Administrator_SQLDTO>()
                .HasIndex(a => a.Username)
                .IsUnique();

            modelBuilder.Entity<Student_SQLDTO>()
                .HasIndex(a => a.Email)
                .IsUnique();

            modelBuilder.Entity<Student_SQLDTO>()
                .HasIndex(a => a.Username)
                .IsUnique();

            modelBuilder.Entity<Teacher_SQLDTO>()
                .HasIndex(a => a.Email)
                .IsUnique();

            modelBuilder.Entity<Teacher_SQLDTO>()
                .HasIndex(a => a.Username)
                .IsUnique();
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