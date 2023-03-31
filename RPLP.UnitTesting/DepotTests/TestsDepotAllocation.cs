using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq.EntityFrameworkCore;
using Moq;
using RPLP.DAL.DTO.Sql;
using RPLP.DAL.SQL;
using RPLP.DAL.SQL.Depots;
using RPLP.ENTITES;
using RPLP.ENTITES.InterfacesDepots;
using Xunit;
using RPLP.JOURNALISATION;
using NSubstitute;
using System.Diagnostics;

namespace RPLP.UnitTesting.DepotTests
{
    public class TestsDepotAllocation
    {
        [Fact]
        public void Ctor_DbContextNull_ExceptionThrownAndLoggingCalled()
        {
            // Arrange
            Logging.ManipulationLog = Substitute.For<IManipulationLogs>();
            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            RPLPDbContext? context = null;

            // Act Assert
            Assert.Throws<ArgumentNullException>(() => { DepotAllocation da = new DepotAllocation(context); });
            logMock.Verify(l => l.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void Ctor_DbContextOk_LoggingNotCalled()
        {
            // Arrange
            Logging.ManipulationLog = Substitute.For<IManipulationLogs>();
            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();

            // Act
            DepotAllocation da = new DepotAllocation(context.Object);

            // Assert
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void GetAllocations_List3Active1Not_ReturnsListOf3()
        {
            // Arrange
            List<Allocation_SQLDTO> allocationDTO = new List<Allocation_SQLDTO>
            {
                new Allocation_SQLDTO
                {
                    Id= "r1s2t0",
                    RepositoryId = 1,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 3
                },
                new Allocation_SQLDTO
                {
                    Id= "r1s4t0",
                    RepositoryId = 1,
                    StudentId = 4,
                    TeacherId = null,
                    Status = 1
                },
                new Allocation_SQLDTO
                {
                    Id= "r1s0t1",
                    RepositoryId = 1,
                    StudentId = null,
                    TeacherId = 1,
                    Status = 2
                },
                new Allocation_SQLDTO
                {
                    Id= "r2s6t0",
                    RepositoryId = 2,
                    StudentId = 6,
                    TeacherId = null,
                    Status = 0
                }
            };
            Logging.ManipulationLog = Substitute.For<IManipulationLogs>();
            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            // Act
            List<Allocation> la = da.GetAllocations();

            // Assert
            Assert.NotNull(la);
            Assert.Equal(3, la.Count);
            Assert.Contains(la, a => a.Id == "r1s2t0");
            Assert.DoesNotContain(la, a => a.Id == "r2s6t0");
            logMock.Verify(l => l.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void GetAllocations_AllInactive_ReturnsEmptyList()
        {
            // Arrange
            List<Allocation_SQLDTO> allocationDTO = new List<Allocation_SQLDTO>
            {
                new Allocation_SQLDTO
                {
                    Id= "r1s2t0",
                    RepositoryId = 1,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 0
                },
                new Allocation_SQLDTO
                {
                    Id= "r1s4t0",
                    RepositoryId = 1,
                    StudentId = 4,
                    TeacherId = null,
                    Status = 0
                },
                new Allocation_SQLDTO
                {
                    Id= "r1s0t1",
                    RepositoryId = 1,
                    StudentId = null,
                    TeacherId = 1,
                    Status = 0
                },
                new Allocation_SQLDTO
                {
                    Id= "r2s6t0",
                    RepositoryId = 2,
                    StudentId = 6,
                    TeacherId = null,
                    Status = 0
                }
            };
            Logging.ManipulationLog = Substitute.For<IManipulationLogs>();
            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            // Act
            List<Allocation> la = da.GetAllocations();

            // Assert
            Assert.NotNull(la);
            Assert.Empty(la);
            logMock.Verify(l => l.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void GetAllocationsByStudentId_ParamNegativeInt_ExceptionThrown()
        {
            // Arrange
            List<Allocation_SQLDTO> allocationDTO = new List<Allocation_SQLDTO>
            {
                new Allocation_SQLDTO
                {
                    Id= "r1s2t0",
                    RepositoryId = 1,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 3
                },
                new Allocation_SQLDTO
                {
                    Id= "r1s4t0",
                    RepositoryId = 1,
                    StudentId = 4,
                    TeacherId = null,
                    Status = 3
                },
                new Allocation_SQLDTO
                {
                    Id= "r1s0t1",
                    RepositoryId = 1,
                    StudentId = null,
                    TeacherId = 1,
                    Status = 3
                },
                new Allocation_SQLDTO
                {
                    Id= "r2s2t0",
                    RepositoryId = 2,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 3
                },
                new Allocation_SQLDTO
                {
                    Id= "r3s2t0",
                    RepositoryId = 2,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 0
                }
            };
            Logging.ManipulationLog = Substitute.For<IManipulationLogs>();
            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            int studentId = -2;

            // Act Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => { List<Allocation> la = da.GetAllocationsByStudentId(studentId); });
            logMock.Verify(l => l.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void GetAllocationsByStudentId_Param0_ExceptionThrown()
        {
            // Arrange
            List<Allocation_SQLDTO> allocationDTO = new List<Allocation_SQLDTO>
            {
                new Allocation_SQLDTO
                {
                    Id= "r1s2t0",
                    RepositoryId = 1,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 3
                },
                new Allocation_SQLDTO
                {
                    Id= "r1s4t0",
                    RepositoryId = 1,
                    StudentId = 4,
                    TeacherId = null,
                    Status = 3
                },
                new Allocation_SQLDTO
                {
                    Id= "r1s0t1",
                    RepositoryId = 1,
                    StudentId = null,
                    TeacherId = 1,
                    Status = 3
                },
                new Allocation_SQLDTO
                {
                    Id= "r2s2t0",
                    RepositoryId = 2,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 3
                },
                new Allocation_SQLDTO
                {
                    Id= "r3s2t0",
                    RepositoryId = 2,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 0
                }
            };
            Logging.ManipulationLog = Substitute.For<IManipulationLogs>();
            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            int studentId = 0;

            // Act Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => { List<Allocation> la = da.GetAllocationsByStudentId(studentId); });
            logMock.Verify(l => l.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void GetAllocationsByStudentId_List5With3SearchedStudent1Inactive_ReturnsListOf2()
        {
            // Arrange
            List<Allocation_SQLDTO> allocationDTO = new List<Allocation_SQLDTO>
            {
                new Allocation_SQLDTO
                {
                    Id= "r1s2t0",
                    RepositoryId = 1,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 3
                },
                new Allocation_SQLDTO
                {
                    Id= "r1s4t0",
                    RepositoryId = 1,
                    StudentId = 4,
                    TeacherId = null,
                    Status = 3
                },
                new Allocation_SQLDTO
                {
                    Id= "r1s0t1",
                    RepositoryId = 1,
                    StudentId = null,
                    TeacherId = 1,
                    Status = 3
                },
                new Allocation_SQLDTO
                {
                    Id= "r2s2t0",
                    RepositoryId = 2,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 3
                },
                new Allocation_SQLDTO
                {
                    Id= "r3s2t0",
                    RepositoryId = 2,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 0
                }
            };
            Logging.ManipulationLog = Substitute.For<IManipulationLogs>();
            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            int studentId = 2;

            // Act
            List<Allocation> la = da.GetAllocationsByStudentId(studentId);

            // Assert
            Assert.NotNull(la);
            Assert.Equal(2, la.Count);
            Assert.Contains(la, a => a.Id == "r1s2t0");
            Assert.Contains(la, a => a.Id == "r2s2t0");
            Assert.DoesNotContain(la, a => a.Id == "r3s2t0");
            logMock.Verify(l => l.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void GetAllocationsByStudentId_List4With0SearchedStudent_ReturnsEmptyList()
        {
            // Arrange
            List<Allocation_SQLDTO> allocationDTO = new List<Allocation_SQLDTO>
            {
                new Allocation_SQLDTO
                {
                    Id= "r1s2t0",
                    RepositoryId = 1,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 3
                },
                new Allocation_SQLDTO
                {
                    Id= "r1s4t0",
                    RepositoryId = 1,
                    StudentId = 4,
                    TeacherId = null,
                    Status = 3
                },
                new Allocation_SQLDTO
                {
                    Id= "r1s0t1",
                    RepositoryId = 1,
                    StudentId = null,
                    TeacherId = 1,
                    Status = 3
                },
                new Allocation_SQLDTO
                {
                    Id= "r2s2t0",
                    RepositoryId = 2,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 3
                },
                new Allocation_SQLDTO
                {
                    Id= "r3s2t0",
                    RepositoryId = 2,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 0
                }
            };
            Logging.ManipulationLog = Substitute.For<IManipulationLogs>();
            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            int studentId = 5;

            // Act
            List<Allocation> la = da.GetAllocationsByStudentId(studentId);

            // Assert
            Assert.NotNull(la);
            Assert.Empty(la);
            logMock.Verify(l => l.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void GetAllocationsByStudentId_NoAllocationsDB_ReturnsEmptyList()
        {
            // Arrange
            List<Allocation_SQLDTO> allocationDTO = new List<Allocation_SQLDTO>();
            Logging.ManipulationLog = Substitute.For<IManipulationLogs>();
            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            int studentId = 5;

            // Act
            List<Allocation> la = da.GetAllocationsByStudentId(studentId);

            // Assert
            Assert.NotNull(la);
            Assert.Empty(la);
            logMock.Verify(l => l.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }
    }
}
