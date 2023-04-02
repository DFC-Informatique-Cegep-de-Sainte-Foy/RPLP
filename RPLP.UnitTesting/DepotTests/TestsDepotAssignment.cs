using Microsoft.EntityFrameworkCore;
using RPLP.DAL.DTO.Sql;
using RPLP.DAL.SQL;
using RPLP.DAL.SQL.Depots;
using RPLP.ENTITES;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace RPLP.UnitTesting.DepotTests
{
    //[Collection("DatabaseTests")]
    //public class TestsDepotAssignment
    //{
    //    private static readonly DbContextOptions<RPLPDbContext> options = new DbContextOptionsBuilder<RPLPDbContext>()
    //            .UseSqlServer("Server=localhost,1434; Database=RPLP; User Id=sa; password=Cad3pend86!")
    //            //.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
    //            .Options;

    //    private void DeleteAssignmentAndRelatedTablesContent()
    //    {
    //        using (var context = new RPLPDbContext(options))
    //        {
    //            context.Database.ExecuteSqlRaw("DELETE from Assignments;");
    //            context.Database.ExecuteSqlRaw("DELETE from Classrooms;");
    //        }
    //    }

    //    private void InsertAssignment(Assignment_SQLDTO p_assignment)
    //    {
    //        using (var context = new RPLPDbContext(options))
    //        {
    //            context.Assignments.Add(p_assignment);
    //            context.SaveChanges();
    //        }
    //    }

    //    private void InsertAssignments(IEnumerable<Assignment_SQLDTO> p_assignments)
    //    {
    //        using (var context = new RPLPDbContext(options))
    //        {
    //            context.Assignments.AddRange(p_assignments);
    //            context.SaveChanges();
    //        }
    //    }

    //    private void InsertReviewAssignment()
    //    {
    //        InsertAssignment(new Assignment_SQLDTO()
    //        {
    //            Name = "Review",
    //            ClassroomName = "RPLP",
    //            DistributionDate = System.DateTime.Now,
    //            Description = "Review a partner\'s code",
    //            DeliveryDeadline = System.DateTime.Now.AddDays(1),
    //            Active = true
    //        });
    //    }

    //    [Fact]
    //    public void Test_GetAssignments()
    //    {
    //        this.DeleteAssignmentAndRelatedTablesContent();

    //        List<Assignment_SQLDTO> assignments = new List<Assignment_SQLDTO>()
    //        {
    //            new Assignment_SQLDTO()
    //            {
    //                Name = "Review",
    //                ClassroomName = "RPLP",
    //                DistributionDate = System.DateTime.Now,
    //                Description = "Review a partner\'s code",
    //                DeliveryDeadline = System.DateTime.Now.AddDays(1),
    //                Active = true
    //            },
    //            new Assignment_SQLDTO()
    //            {
    //                Name = "AnotherOne",
    //                ClassroomName = "RPLP",
    //                DistributionDate = System.DateTime.Now,
    //                Description = "Review another partner\'s code",
    //                DeliveryDeadline = System.DateTime.Now.AddDays(1),
    //                Active = true
    //            }
    //        };

    //        this.InsertAssignments(assignments);

    //        using (var context = new RPLPDbContext(options))
    //        {
    //            DepotAssignment depot = new DepotAssignment(context);
    //            List<Assignment> fetchedAssignments = depot.GetAssignments();

    //            Assert.Contains(fetchedAssignments, f => f.Name == "Review");
    //            Assert.Contains(fetchedAssignments, f => f.Name == "AnotherOne");
    //            Assert.Equal(2, fetchedAssignments.Count);
    //        }

    //        this.DeleteAssignmentAndRelatedTablesContent();
    //    }

    //    [Fact]
    //    public void Test_GetAssignmentById()
    //    {
    //        this.DeleteAssignmentAndRelatedTablesContent();
    //        this.InsertReviewAssignment();

    //        using (var context = new RPLPDbContext(options))
    //        {
    //            DepotAssignment depot = new DepotAssignment(context);
    //            int assignmentId = context.Assignments
    //                .FirstOrDefault(a => a.Name == "Review").Id;
    //            Assignment assignment = depot.GetAssignmentById(assignmentId);

    //            Assert.NotNull(assignment);
    //            Assert.Equal("Review", assignment.Name);
    //        }

    //        this.DeleteAssignmentAndRelatedTablesContent();
    //    }

    //    [Fact]
    //    public void Test_GetAssignmentByName()
    //    {
    //        this.DeleteAssignmentAndRelatedTablesContent();
    //        this.InsertReviewAssignment();

    //        using (var context = new RPLPDbContext(options))
    //        {
    //            DepotAssignment depot = new DepotAssignment(context);
    //            Assignment assignment = depot.GetAssignmentByName("Review");

    //            Assert.NotNull(assignment);
    //        }

    //        this.DeleteAssignmentAndRelatedTablesContent();
    //    }

    //    [Fact]
    //    public void Test_GetAssignmentsByClassroomName()
    //    {
    //        this.DeleteAssignmentAndRelatedTablesContent();
    //        this.InsertReviewAssignment();

    //        using (var context = new RPLPDbContext(options))
    //        {
    //            DepotAssignment depot = new DepotAssignment(context);

    //            List<Assignment> assignments = depot.GetAssignmentsByClassroomName("RPLP");

    //            Assert.True(assignments.Count == 1);

    //        }

    //        this.DeleteAssignmentAndRelatedTablesContent();
    //    }

    //    [Fact]
    //    public void Test_UpsertAssignment_Inserts()
    //    {
    //        this.DeleteAssignmentAndRelatedTablesContent();

    //        using (var context = new RPLPDbContext(options))
    //        {
    //            DepotAssignment depot = new DepotAssignment(context);
    //            Assignment assignment = new Assignment()
    //            {
    //                Name = "Review",
    //                ClassroomName = "RPLP",
    //                DistributionDate = System.DateTime.Now,
    //                Description = "Review a partner\'s code",
    //                DeliveryDeadline = System.DateTime.Now.AddDays(1)
    //            };

    //            depot.UpsertAssignment(assignment);
    //        }

    //        using (var context = new RPLPDbContext(options))
    //        {
    //            Assignment_SQLDTO? assignment = context.Assignments
    //                                            .FirstOrDefault(a => a.Name == "Review");

    //            Assert.NotNull(assignment);
    //        }

    //        this.DeleteAssignmentAndRelatedTablesContent();
    //    }

    //    [Fact]
    //    public void Test_UpsertAssignment_Updates()
    //    {
    //        this.DeleteAssignmentAndRelatedTablesContent();
    //        this.InsertReviewAssignment();

    //        using (var context = new RPLPDbContext(options))
    //        {
    //            DepotAssignment depot = new DepotAssignment(context);
    //            Assignment? assignment = context.Assignments.FirstOrDefault(a => a.Name == "Review").ToEntity();

    //            assignment.Name = "Modified";

    //            depot.UpsertAssignment(assignment);
    //        }

    //        using (var context = new RPLPDbContext(options))
    //        {
    //            Assignment_SQLDTO? nonModifiedAssignment = context.Assignments
    //                                            .FirstOrDefault(a => a.Name == "Review");
    //            Assignment_SQLDTO? modifiedAssignment = context.Assignments
    //                                            .FirstOrDefault(a => a.Name == "Modified");

    //            Assert.Null(nonModifiedAssignment);
    //            Assert.NotNull(modifiedAssignment);
    //        }

    //        this.DeleteAssignmentAndRelatedTablesContent();
    //    }

    //    [Fact]
    //    public void Test_DeleteAssignment()
    //    {
    //        this.DeleteAssignmentAndRelatedTablesContent();
    //        this.InsertReviewAssignment();

    //        using (var context = new RPLPDbContext(options))
    //        {
    //            DepotAssignment depot = new DepotAssignment(context);

    //            Assert.NotNull(context.Assignments.FirstOrDefault(a => a.Name == "Review" && a.Active == true));

    //            depot.DeleteAssignment("Review");
    //        }

    //        using (var context = new RPLPDbContext(options))
    //        {
    //            Assert.Null(context.Assignments.FirstOrDefault(a => a.Name == "Review" && a.Active == true));
    //        }

    //        this.DeleteAssignmentAndRelatedTablesContent();
    //    }
    //}
}
