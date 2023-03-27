using Microsoft.EntityFrameworkCore;
using RPLP.DAL.DTO.Sql;

namespace RPLP.DAL.SQL
{
    public class RPLPDbContext : DbContext
    {

        public RPLPDbContext(DbContextOptions<RPLPDbContext> options) :
            base(options)
        {
            Database.Migrate();
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

            modelBuilder.Entity<Allocation_SQLDTO>()
                .HasNoKey();
        }

        public DbSet<Classroom_SQLDTO> Classrooms { get; set; }
        public DbSet<Administrator_SQLDTO> Administrators { get; set; }
        public DbSet<Assignment_SQLDTO> Assignments { get; set; }
        public DbSet<Allocation_SQLDTO> Allocations { get; set; }
        public DbSet<Comment_SQLDTO> Comments { get; set; }
        public DbSet<Organisation_SQLDTO> Organisations { get; set; }
        public DbSet<Repository_SQLDTO> Repositories { get; set; }
        public DbSet<Student_SQLDTO> Students { get; set; }
        public DbSet<Teacher_SQLDTO> Teachers { get; set; }
    }
}