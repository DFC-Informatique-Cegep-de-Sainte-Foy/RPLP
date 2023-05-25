using Microsoft.EntityFrameworkCore;
using RPLP.DAL.DTO.Sql;

namespace RPLP.DAL.SQL
{
    public class RPLPDbContext : DbContext
    {
        public RPLPDbContext()
        {
            ;
        }
        public RPLPDbContext(DbContextOptions<RPLPDbContext> options) :
            base(options)
        {
            Database.Migrate();
        }
        
        // protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        // {
        //     if (!optionsBuilder.IsConfigured)
        //     {
        //         optionsBuilder.UseSqlServer("Server=localhost,1433;Database=RPLP;User Id=sa;password=Cad3pend86!");
        //     }
        // }

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
                .HasKey(all => all.Id);
        }

        public virtual DbSet<Classroom_SQLDTO> Classrooms { get; set; }
        public virtual DbSet<Administrator_SQLDTO> Administrators { get; set; }
        public virtual DbSet<Assignment_SQLDTO> Assignments { get; set; }
        public virtual DbSet<Allocation_SQLDTO> Allocations { get; set; }
        public virtual DbSet<Comment_SQLDTO> Comments { get; set; }
        public virtual DbSet<Organisation_SQLDTO> Organisations { get; set; }
        public virtual DbSet<Repository_SQLDTO> Repositories { get; set; }
        public virtual DbSet<Student_SQLDTO> Students { get; set; }
        public virtual DbSet<Teacher_SQLDTO> Teachers { get; set; }
    }
}