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
        #region Ctor
        [Fact]
        public void Ctor_DbContextNull_ExceptionThrownAndLoggingCalled()
        {
            // Arrange

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

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();

            // Act
            DepotAllocation da = new DepotAllocation(context.Object);

            // Assert
            logMock.VerifyNoOtherCalls();
        }
        #endregion

        #region GetAllocations
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
        #endregion

        #region GetAllocationsByStudentId
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
        #endregion

        #region GetAllocationsByRepositoryID
        [Fact]
        public void GetAllocationsByRepositoryID_ParamNegativeInt_ExceptionThrown()
        {
            // Arrange

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            DepotAllocation da = new DepotAllocation(context.Object);

            int repositoryId = -2;

            // Act Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => { List<Allocation> la = da.GetAllocationsByRepositoryID(repositoryId); });
            logMock.Verify(l => l.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void GetAllocationsByRepositoryID_Param0_ExceptionThrown()
        {
            // Arrange

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            DepotAllocation da = new DepotAllocation(context.Object);

            int repositoryId = 0;

            // Act Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => { List<Allocation> la = da.GetAllocationsByRepositoryID(repositoryId); });
            logMock.Verify(l => l.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void GetAllocationsByRepositoryID_List5With3SearchedRepository1Inactive_ReturnsListOf2()
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
                    RepositoryId = 2,
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
                    Id= "r1s1t0",
                    RepositoryId = 1,
                    StudentId = 1,
                    TeacherId = null,
                    Status = 0
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            int repositoryId = 1;

            // Act
            List<Allocation> la = da.GetAllocationsByRepositoryID(repositoryId);

            // Assert
            Assert.NotNull(la);
            Assert.Equal(2, la.Count);
            Assert.Contains(la, a => a.Id == "r1s2t0");
            Assert.Contains(la, a => a.Id == "r1s4t0");
            Assert.DoesNotContain(la, a => a.Id == "r1s1t0");
            logMock.Verify(l => l.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void GetAllocationsByRepositoryID_List4With0SearchedRepository_ReturnsEmptyList()
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

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            int repositoryId = 5;

            // Act
            List<Allocation> la = da.GetAllocationsByRepositoryID(repositoryId);

            // Assert
            Assert.NotNull(la);
            Assert.Empty(la);
            logMock.Verify(l => l.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void GetAllocationsByRepositoryID_NoAllocationsDB_ReturnsEmptyList()
        {
            // Arrange
            List<Allocation_SQLDTO> allocationDTO = new List<Allocation_SQLDTO>();

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            int repositoryId = 5;

            // Act
            List<Allocation> la = da.GetAllocationsByRepositoryID(repositoryId);

            // Assert
            Assert.NotNull(la);
            Assert.Empty(la);
            logMock.Verify(l => l.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }
        #endregion

        #region GetAllocationByStudentAndRepositoryIDs
        [Fact]
        public void GetAllocationByStudentAndRepositoryIDs_ParamStudentIdNegativeInt_ExceptionThrown()
        {
            // Arrange

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            DepotAllocation da = new DepotAllocation(context.Object);

            int studentId = -2;
            int repositoryId = 2;

            // Act Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => { Allocation? a = da.GetAllocationByStudentAndRepositoryIDs(studentId, repositoryId); });
            logMock.Verify(l => l.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void GetAllocationByStudentAndRepositoryIDs_ParamRepositoryIdNegativeInt_ExceptionThrown()
        {
            // Arrange

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            DepotAllocation da = new DepotAllocation(context.Object);

            int studentId = 2;
            int repositoryId = -2;

            // Act Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => { Allocation a = da.GetAllocationByStudentAndRepositoryIDs(studentId, repositoryId); });
            logMock.Verify(l => l.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void GetAllocationByStudentAndRepositoryIDs_ParamStudentId0_ExceptionThrown()
        {
            // Arrange

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            DepotAllocation da = new DepotAllocation(context.Object);

            int studentId = 0;
            int repositoryId = 2;

            // Act Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => { Allocation a = da.GetAllocationByStudentAndRepositoryIDs(studentId, repositoryId); });
            logMock.Verify(l => l.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void GetAllocationByStudentAndRepositoryIDs_ParamRepositoryId0_ExceptionThrown()
        {
            // Arrange

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            DepotAllocation da = new DepotAllocation(context.Object);

            int studentId = 2;
            int repositoryId = 0;

            // Act Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => { Allocation a = da.GetAllocationByStudentAndRepositoryIDs(studentId, repositoryId); });
            logMock.Verify(l => l.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void GetAllocationByStudentAndRepositoryIDs_List5WithSearchedAllocationPresent_ReturnsAllocation()
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
                    RepositoryId = 2,
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
                    Id= "r1s1t0",
                    RepositoryId = 1,
                    StudentId = 1,
                    TeacherId = null,
                    Status = 0
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            int studentId = 4;
            int repositoryId = 1;

            string expectedAllocationId = "r1s4t0";

            // Act
            Allocation a = da.GetAllocationByStudentAndRepositoryIDs(studentId, repositoryId);

            // Assert
            Assert.NotNull(a);
            Assert.Equal(studentId, a.StudentId);
            Assert.Equal(repositoryId, a.RepositoryId);
            Assert.Equal(expectedAllocationId, a.Id);
            logMock.Verify(l => l.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void GetAllocationByStudentAndRepositoryIDs_List5WithSearchedAllocationNotPresent_ReturnsNull()
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
                    RepositoryId = 2,
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
                    Id= "r1s1t0",
                    RepositoryId = 1,
                    StudentId = 1,
                    TeacherId = null,
                    Status = 0
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            int studentId = 4;
            int repositoryId = 2;

            // Act
            Allocation? a = da.GetAllocationByStudentAndRepositoryIDs(studentId, repositoryId);

            // Assert
            Assert.Null(a);
            logMock.Verify(l => l.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void GetAllocationByStudentAndRepositoryIDs_List5WithSearchedAllocationPresentButInactive_ReturnsNull()
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
                    RepositoryId = 2,
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
                    Id= "r1s1t0",
                    RepositoryId = 1,
                    StudentId = 1,
                    TeacherId = null,
                    Status = 0
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            int studentId = 1;
            int repositoryId = 1;

            // Act
            Allocation? a = da.GetAllocationByStudentAndRepositoryIDs(studentId, repositoryId);

            // Assert
            Assert.Null(a);
            logMock.Verify(l => l.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void GetAllocationByStudentAndRepositoryIDs_List5WithSearchedAllocationPresentAndActiveTwice_ReturnsNull()
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
                    RepositoryId = 2,
                    StudentId = null,
                    TeacherId = 1,
                    Status = 3
                },
                new Allocation_SQLDTO
                {
                    Id= "r2s2t0",
                    RepositoryId = 1,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 3
                },
                new Allocation_SQLDTO
                {
                    Id= "r1s1t0",
                    RepositoryId = 1,
                    StudentId = 1,
                    TeacherId = null,
                    Status = 0
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            int studentId = 2;
            int repositoryId = 1;

            // Act
            Allocation? a = da.GetAllocationByStudentAndRepositoryIDs(studentId, repositoryId);

            // Assert
            Assert.Null(a);
            logMock.Verify(l => l.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void GetAllocationByStudentAndRepositoryIDs_List5WithSearchedAllocationPresentTwiceOneInactive_ReturnsActiveAllocation()
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
                    RepositoryId = 2,
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
                    Id= "r1s1t0",
                    RepositoryId = 1,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 0
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            int studentId = 2;
            int repositoryId = 1;

            string expectedAllocationId = "r1s2t0";

            // Act
            Allocation? a = da.GetAllocationByStudentAndRepositoryIDs(studentId, repositoryId);

            // Assert
            Assert.NotNull(a);
            Assert.Equal(studentId, a.StudentId);
            Assert.Equal(repositoryId, a.RepositoryId);
            Assert.Equal(expectedAllocationId, a.Id);
            logMock.Verify(l => l.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }
        #endregion

        #region GetAllocationsByAssignmentID
        [Fact]
        public void GetAllocationsByAssignmentID_ParamAssignmentIdNegativeInt_ExceptionThrown()
        {
            // Arrange
            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            DepotAllocation da = new DepotAllocation(context.Object);

            int p_assignmentId = -2;

            // Act Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => { List<Allocation> a = da.GetAllocationsByAssignmentID(p_assignmentId); });
            logMock.Verify(l => l.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void GetAllocationsByAssignmentID_List5AllocationsAllInAssignment_ReturnsListOf5()
        {
            // Arrange
            List<Allocation_SQLDTO> allocationsDTO = new List<Allocation_SQLDTO>
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
                    Id= "r2s0t1",
                    RepositoryId = 2,
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
                    Id= "r1s1t0",
                    RepositoryId = 1,
                    StudentId = 1,
                    TeacherId = null,
                    Status = 2
                }
            };

            List<Assignment_SQLDTO> assignmentsDTO = new List<Assignment_SQLDTO>()
            {
                new Assignment_SQLDTO()
                {
                    Id = 1,
                    Name = "Assigmnent1",
                    ClassroomName = "Classroom2",
                    DistributionDate = System.DateTime.Now,
                    Description = "Review a partner\'s code",
                    DeliveryDeadline = System.DateTime.Now.AddDays(1),
                    Active = true
                },
                new Assignment_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2",
                    ClassroomName = "Classroom2",
                    DistributionDate = System.DateTime.Now,
                    Description = "Review another partner\'s code",
                    DeliveryDeadline = System.DateTime.Now.AddDays(1),
                    Active = true
                }
            };

            List<Classroom_SQLDTO> classroomsDTO = new List<Classroom_SQLDTO>()
            {
                new Classroom_SQLDTO()
                {
                    Id = 1,
                    Name = "Classroom1",
                    OrganisationName = "Organisation1",
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },

                new Classroom_SQLDTO()
                {
                    Id = 2,
                    Name = "Classroom2",
                    OrganisationName = "Organisation1",
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                }
           };

            List<Repository_SQLDTO> repositoriesDTO = new List<Repository_SQLDTO>()
            {
                new Repository_SQLDTO()
                {
                    Id = 1,
                    Name = "Assigmnent2-ThPaquet",
                    FullName = "Organisation1/Assigmnent2-ThPaquet",
                    OrganisationName = "Organisation1",
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2-ikeameatbol",
                    FullName = "Organisation1/Assigmnent2-BACenComm",
                    OrganisationName = "Organisation1",
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 3,
                    Name = "Assigmnent2-BACenComm",
                    FullName = "Organisation1/Assigmnent2-BACenComm",
                    OrganisationName = "Organisation1",
                    Active = false
                },
                new Repository_SQLDTO()
                {
                    Id = 4,
                    Name = "Assigmnent1-2151215",
                    FullName = "Organisation1/Assigmnent1-2151215",
                    OrganisationName = "Organisation1",
                    Active = true
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationsDTO);
            context.Setup(x => x.Assignments).ReturnsDbSet(assignmentsDTO);
            context.Setup(x => x.Classrooms).ReturnsDbSet(classroomsDTO);
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            int assignmentId = 2;

            // Act
            List<Allocation> la = da.GetAllocationsByAssignmentID(assignmentId);

            // Assert
            Assert.NotNull(la);
            Assert.Equal(5, la.Count);
            Assert.Contains(la, a => a.Id == "r1s2t0");
            Assert.Contains(la, a => a.Id == "r1s4t0");
            Assert.Contains(la, a => a.Id == "r2s0t1");
            Assert.Contains(la, a => a.Id == "r2s2t0");
            Assert.Contains(la, a => a.Id == "r1s1t0");
            logMock.Verify(l => l.Journal(It.IsAny<Log>()), Times.Exactly(2));
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void GetAllocationsByAssignmentID_List5AllocationsNoneInAssignment_ReturnsEmptyList()
        {
            // Arrange
            List<Allocation_SQLDTO> allocationsDTO = new List<Allocation_SQLDTO>
            {
                new Allocation_SQLDTO
                {
                    Id= "r4s2t0",
                    RepositoryId = 4,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 3
                },
                new Allocation_SQLDTO
                {
                    Id= "r4s4t0",
                    RepositoryId = 4,
                    StudentId = 4,
                    TeacherId = null,
                    Status = 3
                },
                new Allocation_SQLDTO
                {
                    Id= "r4s0t1",
                    RepositoryId = 4,
                    StudentId = null,
                    TeacherId = 1,
                    Status = 3
                },
                new Allocation_SQLDTO
                {
                    Id= "r5s2t0",
                    RepositoryId = 5,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 3
                },
                new Allocation_SQLDTO
                {
                    Id= "r5s1t0",
                    RepositoryId = 5,
                    StudentId = 1,
                    TeacherId = null,
                    Status = 2
                }
            };

            List<Assignment_SQLDTO> assignmentsDTO = new List<Assignment_SQLDTO>()
            {
                new Assignment_SQLDTO()
                {
                    Id = 1,
                    Name = "Assigmnent1",
                    ClassroomName = "Classroom2",
                    DistributionDate = System.DateTime.Now,
                    Description = "Review a partner\'s code",
                    DeliveryDeadline = System.DateTime.Now.AddDays(1),
                    Active = true
                },
                new Assignment_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2",
                    ClassroomName = "Classroom2",
                    DistributionDate = System.DateTime.Now,
                    Description = "Review another partner\'s code",
                    DeliveryDeadline = System.DateTime.Now.AddDays(1),
                    Active = true
                }
            };

            List<Classroom_SQLDTO> classroomsDTO = new List<Classroom_SQLDTO>()
            {
                new Classroom_SQLDTO()
                {
                    Id = 1,
                    Name = "Classroom1",
                    OrganisationName = "Organisation1",
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },

                new Classroom_SQLDTO()
                {
                    Id = 2,
                    Name = "Classroom2",
                    OrganisationName = "Organisation1",
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                }
           };

            List<Repository_SQLDTO> repositoriesDTO = new List<Repository_SQLDTO>()
            {
                new Repository_SQLDTO()
                {
                    Id = 1,
                    Name = "Assigmnent2-ThPaquet",
                    FullName = "Organisation1/Assigmnent2-ThPaquet",
                    OrganisationName = "Organisation1",
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2-ikeameatbol",
                    FullName = "Organisation1/Assigmnent2-BACenComm",
                    OrganisationName = "Organisation1",
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 3,
                    Name = "Assigmnent2-BACenComm",
                    FullName = "Organisation1/Assigmnent2-BACenComm",
                    OrganisationName = "Organisation1",
                    Active = false
                },
                new Repository_SQLDTO()
                {
                    Id = 4,
                    Name = "Assigmnent1-2151215",
                    FullName = "Organisation1/Assigmnent1-2151215",
                    OrganisationName = "Organisation1",
                    Active = true
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationsDTO);
            context.Setup(x => x.Assignments).ReturnsDbSet(assignmentsDTO);
            context.Setup(x => x.Classrooms).ReturnsDbSet(classroomsDTO);
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            int assignmentId = 2;

            // Act
            List<Allocation> la = da.GetAllocationsByAssignmentID(assignmentId);

            // Assert
            Assert.NotNull(la);
            Assert.Empty(la);
            logMock.Verify(l => l.Journal(It.IsAny<Log>()), Times.Exactly(2));
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void GetAllocationsByAssignmentID_List5AllocationsNoRepositoryInAssignment_ReturnsEmptyList()
        {
            // Arrange
            List<Allocation_SQLDTO> allocationsDTO = new List<Allocation_SQLDTO>
            {
                new Allocation_SQLDTO
                {
                    Id= "r4s2t0",
                    RepositoryId = 4,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 3
                },
                new Allocation_SQLDTO
                {
                    Id= "r4s4t0",
                    RepositoryId = 4,
                    StudentId = 4,
                    TeacherId = null,
                    Status = 3
                },
                new Allocation_SQLDTO
                {
                    Id= "r4s0t1",
                    RepositoryId = 4,
                    StudentId = null,
                    TeacherId = 1,
                    Status = 3
                },
                new Allocation_SQLDTO
                {
                    Id= "r5s2t0",
                    RepositoryId = 5,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 3
                },
                new Allocation_SQLDTO
                {
                    Id= "r5s1t0",
                    RepositoryId = 5,
                    StudentId = 1,
                    TeacherId = null,
                    Status = 2
                }
            };

            List<Assignment_SQLDTO> assignmentsDTO = new List<Assignment_SQLDTO>()
            {
                new Assignment_SQLDTO()
                {
                    Id = 1,
                    Name = "Assigmnent1",
                    ClassroomName = "Classroom2",
                    DistributionDate = System.DateTime.Now,
                    Description = "Review a partner\'s code",
                    DeliveryDeadline = System.DateTime.Now.AddDays(1),
                    Active = true
                },
                new Assignment_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2",
                    ClassroomName = "Classroom2",
                    DistributionDate = System.DateTime.Now,
                    Description = "Review another partner\'s code",
                    DeliveryDeadline = System.DateTime.Now.AddDays(1),
                    Active = true
                }
            };

            List<Classroom_SQLDTO> classroomsDTO = new List<Classroom_SQLDTO>()
            {
                new Classroom_SQLDTO()
                {
                    Id = 1,
                    Name = "Classroom1",
                    OrganisationName = "Organisation1",
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },

                new Classroom_SQLDTO()
                {
                    Id = 2,
                    Name = "Classroom2",
                    OrganisationName = "Organisation1",
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                }
           };

            List<Repository_SQLDTO> repositoriesDTO = new List<Repository_SQLDTO>()
            {
                new Repository_SQLDTO()
                {
                    Id = 1,
                    Name = "Assigmnent2-ThPaquet",
                    FullName = "Organisation1/Assigmnent2-ThPaquet",
                    OrganisationName = "Organisation1",
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2-ikeameatbol",
                    FullName = "Organisation1/Assigmnent2-BACenComm",
                    OrganisationName = "Organisation1",
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 3,
                    Name = "Assigmnent2-BACenComm",
                    FullName = "Organisation1/Assigmnent2-BACenComm",
                    OrganisationName = "Organisation1",
                    Active = false
                },
                new Repository_SQLDTO()
                {
                    Id = 4,
                    Name = "Assigmnent2-2151215",
                    FullName = "Organisation1/Assigmnent2-2151215",
                    OrganisationName = "Organisation1",
                    Active = true
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationsDTO);
            context.Setup(x => x.Assignments).ReturnsDbSet(assignmentsDTO);
            context.Setup(x => x.Classrooms).ReturnsDbSet(classroomsDTO);
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            int assignmentId = 1;

            // Act
            List<Allocation> la = da.GetAllocationsByAssignmentID(assignmentId);

            // Assert
            Assert.NotNull(la);
            Assert.Empty(la);
            logMock.Verify(l => l.Journal(It.IsAny<Log>()), Times.Never);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void GetAllocationsByAssignmentID_List5AllocationsClassroomInactiveInAssignment_ReturnsEmptyList()
        {
            // Arrange
            List<Allocation_SQLDTO> allocationsDTO = new List<Allocation_SQLDTO>
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
                    Id= "r2s1t0",
                    RepositoryId = 2,
                    StudentId = 1,
                    TeacherId = null,
                    Status = 2
                }
            };

            List<Assignment_SQLDTO> assignmentsDTO = new List<Assignment_SQLDTO>()
            {
                new Assignment_SQLDTO()
                {
                    Id = 1,
                    Name = "Assigmnent1",
                    ClassroomName = "Classroom3",
                    DistributionDate = System.DateTime.Now,
                    Description = "Review a partner\'s code",
                    DeliveryDeadline = System.DateTime.Now.AddDays(1),
                    Active = true
                },
                new Assignment_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2",
                    ClassroomName = "Classroom2",
                    DistributionDate = System.DateTime.Now,
                    Description = "Review another partner\'s code",
                    DeliveryDeadline = System.DateTime.Now.AddDays(1),
                    Active = true
                }
            };

            List<Classroom_SQLDTO> classroomsDTO = new List<Classroom_SQLDTO>()
            {
                new Classroom_SQLDTO()
                {
                    Id = 1,
                    Name = "Classroom1",
                    OrganisationName = "Organisation1",
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },

                new Classroom_SQLDTO()
                {
                    Id = 2,
                    Name = "Classroom2",
                    OrganisationName = "Organisation1",
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                }
           };

            List<Repository_SQLDTO> repositoriesDTO = new List<Repository_SQLDTO>()
            {
                new Repository_SQLDTO()
                {
                    Id = 1,
                    Name = "Assigmnent2-ThPaquet",
                    FullName = "Organisation1/Assigmnent2-ThPaquet",
                    OrganisationName = "Organisation1",
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2-ikeameatbol",
                    FullName = "Organisation1/Assigmnent2-BACenComm",
                    OrganisationName = "Organisation1",
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 3,
                    Name = "Assigmnent2-BACenComm",
                    FullName = "Organisation1/Assigmnent2-BACenComm",
                    OrganisationName = "Organisation1",
                    Active = false
                },
                new Repository_SQLDTO()
                {
                    Id = 4,
                    Name = "Assigmnent2-2151215",
                    FullName = "Organisation1/Assigmnent2-2151215",
                    OrganisationName = "Organisation1",
                    Active = true
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationsDTO);
            context.Setup(x => x.Assignments).ReturnsDbSet(assignmentsDTO);
            context.Setup(x => x.Classrooms).ReturnsDbSet(classroomsDTO);
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            int assignmentId = 1;

            // Act
            List<Allocation> la = da.GetAllocationsByAssignmentID(assignmentId);

            // Assert
            Assert.NotNull(la);
            Assert.Empty(la);
            logMock.Verify(l => l.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }

        [Fact]
        public void GetAllocationsByAssignmentID_List5AllocationsAssignmentNonexistant_ReturnsEmptyList()
        {
            // Arrange
            List<Allocation_SQLDTO> allocationsDTO = new List<Allocation_SQLDTO>
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
                    Id= "r2s1t0",
                    RepositoryId = 2,
                    StudentId = 1,
                    TeacherId = null,
                    Status = 2
                }
            };

            List<Assignment_SQLDTO> assignmentsDTO = new List<Assignment_SQLDTO>()
            {
                new Assignment_SQLDTO()
                {
                    Id = 1,
                    Name = "Assigmnent1",
                    ClassroomName = "Classroom1",
                    DistributionDate = System.DateTime.Now,
                    Description = "Review a partner\'s code",
                    DeliveryDeadline = System.DateTime.Now.AddDays(1),
                    Active = true
                },
                new Assignment_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2",
                    ClassroomName = "Classroom2",
                    DistributionDate = System.DateTime.Now,
                    Description = "Review another partner\'s code",
                    DeliveryDeadline = System.DateTime.Now.AddDays(1),
                    Active = true
                }
            };

            List<Classroom_SQLDTO> classroomsDTO = new List<Classroom_SQLDTO>()
            {
                new Classroom_SQLDTO()
                {
                    Id = 1,
                    Name = "Classroom1",
                    OrganisationName = "Organisation1",
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },

                new Classroom_SQLDTO()
                {
                    Id = 2,
                    Name = "Classroom2",
                    OrganisationName = "Organisation1",
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                }
           };

            List<Repository_SQLDTO> repositoriesDTO = new List<Repository_SQLDTO>()
            {
                new Repository_SQLDTO()
                {
                    Id = 1,
                    Name = "Assigmnent2-ThPaquet",
                    FullName = "Organisation1/Assigmnent2-ThPaquet",
                    OrganisationName = "Organisation1",
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2-ikeameatbol",
                    FullName = "Organisation1/Assigmnent2-BACenComm",
                    OrganisationName = "Organisation1",
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 3,
                    Name = "Assigmnent2-BACenComm",
                    FullName = "Organisation1/Assigmnent2-BACenComm",
                    OrganisationName = "Organisation1",
                    Active = false
                },
                new Repository_SQLDTO()
                {
                    Id = 4,
                    Name = "Assigmnent2-2151215",
                    FullName = "Organisation1/Assigmnent2-2151215",
                    OrganisationName = "Organisation1",
                    Active = true
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationsDTO);
            context.Setup(x => x.Assignments).ReturnsDbSet(assignmentsDTO);
            context.Setup(x => x.Classrooms).ReturnsDbSet(classroomsDTO);
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            int assignmentId = 3;

            // Act
            List<Allocation> la = da.GetAllocationsByAssignmentID(assignmentId);

            // Assert
            Assert.NotNull(la);
            Assert.Empty(la);
            logMock.Verify(l => l.Journal(It.IsAny<Log>()), Times.Once);
            logMock.VerifyNoOtherCalls();
        }
        #endregion
    }
}
