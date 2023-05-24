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
            Logging.Instance.ManipulationLog = logMock.Object;

            RPLPDbContext? context = null;

            // Act Assert
            Assert.Throws<ArgumentNullException>(() => { DepotAllocation da = new DepotAllocation(context); });
            
            
        }

        [Fact]
        public void Ctor_DbContextOk_LoggingNotCalled()
        {
            // Arrange

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();

            // Act
            DepotAllocation da = new DepotAllocation(context.Object);

            // Assert
            
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
            Logging.Instance.ManipulationLog = logMock.Object;

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
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            // Act
            List<Allocation> la = da.GetAllocations();

            // Assert
            Assert.NotNull(la);
            Assert.Empty(la);
            
            
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
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            int studentId = -2;

            // Act Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => { List<Allocation> la = da.GetAllocationsByStudentId(studentId); });
            
            
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
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            int studentId = 0;

            // Act Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => { List<Allocation> la = da.GetAllocationsByStudentId(studentId); });
            
            
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
            Logging.Instance.ManipulationLog = logMock.Object;

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
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            int studentId = 5;

            // Act
            List<Allocation> la = da.GetAllocationsByStudentId(studentId);

            // Assert
            Assert.NotNull(la);
            Assert.Empty(la);
            
            
        }

        [Fact]
        public void GetAllocationsByStudentId_NoAllocationsDB_ReturnsEmptyList()
        {
            // Arrange
            List<Allocation_SQLDTO> allocationDTO = new List<Allocation_SQLDTO>();

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            int studentId = 5;

            // Act
            List<Allocation> la = da.GetAllocationsByStudentId(studentId);

            // Assert
            Assert.NotNull(la);
            Assert.Empty(la);
            
            
        }
        #endregion

        #region GetAllocationsByRepositoryID
        [Fact]
        public void GetAllocationsByRepositoryID_ParamNegativeInt_ExceptionThrown()
        {
            // Arrange

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            DepotAllocation da = new DepotAllocation(context.Object);

            int repositoryId = -2;

            // Act Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => { List<Allocation> la = da.GetAllocationsByRepositoryID(repositoryId); });
            
            
        }

        [Fact]
        public void GetAllocationsByRepositoryID_Param0_ExceptionThrown()
        {
            // Arrange

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            DepotAllocation da = new DepotAllocation(context.Object);

            int repositoryId = 0;

            // Act Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => { List<Allocation> la = da.GetAllocationsByRepositoryID(repositoryId); });
            
            
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
            Logging.Instance.ManipulationLog = logMock.Object;

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
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            int repositoryId = 5;

            // Act
            List<Allocation> la = da.GetAllocationsByRepositoryID(repositoryId);

            // Assert
            Assert.NotNull(la);
            Assert.Empty(la);
            
            
        }

        [Fact]
        public void GetAllocationsByRepositoryID_NoAllocationsDB_ReturnsEmptyList()
        {
            // Arrange
            List<Allocation_SQLDTO> allocationDTO = new List<Allocation_SQLDTO>();

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            int repositoryId = 5;

            // Act
            List<Allocation> la = da.GetAllocationsByRepositoryID(repositoryId);

            // Assert
            Assert.NotNull(la);
            Assert.Empty(la);
            
            
        }
        #endregion

        #region GetAllocationByStudentAndRepositoryIDs
        [Fact]
        public void GetAllocationByStudentAndRepositoryIDs_ParamStudentIdNegativeInt_ExceptionThrown()
        {
            // Arrange

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            DepotAllocation da = new DepotAllocation(context.Object);

            int studentId = -2;
            int repositoryId = 2;

            // Act Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => { Allocation? a = da.GetAllocationByStudentAndRepositoryIDs(studentId, repositoryId); });
            
            
        }

        [Fact]
        public void GetAllocationByStudentAndRepositoryIDs_ParamRepositoryIdNegativeInt_ExceptionThrown()
        {
            // Arrange

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            DepotAllocation da = new DepotAllocation(context.Object);

            int studentId = 2;
            int repositoryId = -2;

            // Act Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => { Allocation a = da.GetAllocationByStudentAndRepositoryIDs(studentId, repositoryId); });
            
            
        }

        [Fact]
        public void GetAllocationByStudentAndRepositoryIDs_ParamStudentId0_ExceptionThrown()
        {
            // Arrange

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            DepotAllocation da = new DepotAllocation(context.Object);

            int studentId = 0;
            int repositoryId = 2;

            // Act Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => { Allocation a = da.GetAllocationByStudentAndRepositoryIDs(studentId, repositoryId); });
            
            
        }

        [Fact]
        public void GetAllocationByStudentAndRepositoryIDs_ParamRepositoryId0_ExceptionThrown()
        {
            // Arrange

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            DepotAllocation da = new DepotAllocation(context.Object);

            int studentId = 2;
            int repositoryId = 0;

            // Act Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => { Allocation a = da.GetAllocationByStudentAndRepositoryIDs(studentId, repositoryId); });
            
            
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
            Logging.Instance.ManipulationLog = logMock.Object;

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
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            int studentId = 4;
            int repositoryId = 2;

            // Act
            Allocation? a = da.GetAllocationByStudentAndRepositoryIDs(studentId, repositoryId);

            // Assert
            Assert.Null(a);
            
            
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
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            int studentId = 1;
            int repositoryId = 1;

            // Act
            Allocation? a = da.GetAllocationByStudentAndRepositoryIDs(studentId, repositoryId);

            // Assert
            Assert.Null(a);
            
            
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
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            int studentId = 2;
            int repositoryId = 1;

            // Act
            Allocation? a = da.GetAllocationByStudentAndRepositoryIDs(studentId, repositoryId);

            // Assert
            Assert.Null(a);
            
            
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
            Logging.Instance.ManipulationLog = logMock.Object;

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
            
            
        }
        #endregion

        #region GetAllocationsByAssignmentID
        [Fact]
        public void GetAllocationsByAssignmentID_ParamAssignmentIdNegativeInt_ExceptionThrown()
        {
            // Arrange
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            DepotAllocation da = new DepotAllocation(context.Object);

            int p_assignmentId = -2;

            // Act Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => { List<Allocation> a = da.GetAllocationsByAssignmentID(p_assignmentId); });
            
            
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
                    ClassroomId = 2,
                    DistributionDate = System.DateTime.Now,
                    Description = "Review a partner\'s code",
                    DeliveryDeadline = System.DateTime.Now.AddDays(1),
                    Active = true
                },
                new Assignment_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2",
                    ClassroomId = 2,
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
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },

                new Classroom_SQLDTO()
                {
                    Id = 2,
                    Name = "Classroom2",
                    OrganisationId = 1,
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
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2-ikeameatbol",
                    FullName = "Organisation1/Assigmnent2-BACenComm",
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 3,
                    Name = "Assigmnent2-BACenComm",
                    FullName = "Organisation1/Assigmnent2-BACenComm",
                    OrganisationId = 1,
                    Active = false
                },
                new Repository_SQLDTO()
                {
                    Id = 4,
                    Name = "Assigmnent1-2151215",
                    FullName = "Organisation1/Assigmnent1-2151215",
                    OrganisationId = 1,
                    Active = true
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

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
                    ClassroomId = 2,
                    DistributionDate = System.DateTime.Now,
                    Description = "Review a partner\'s code",
                    DeliveryDeadline = System.DateTime.Now.AddDays(1),
                    Active = true
                },
                new Assignment_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2",
                    ClassroomId = 2,
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
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },

                new Classroom_SQLDTO()
                {
                    Id = 2,
                    Name = "Classroom2",
                    OrganisationId = 1,
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
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2-ikeameatbol",
                    FullName = "Organisation1/Assigmnent2-BACenComm",
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 3,
                    Name = "Assigmnent2-BACenComm",
                    FullName = "Organisation1/Assigmnent2-BACenComm",
                    OrganisationId = 1,
                    Active = false
                },
                new Repository_SQLDTO()
                {
                    Id = 4,
                    Name = "Assigmnent1-2151215",
                    FullName = "Organisation1/Assigmnent1-2151215",
                    OrganisationId = 1,
                    Active = true
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

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
                    ClassroomId = 2,
                    DistributionDate = System.DateTime.Now,
                    Description = "Review a partner\'s code",
                    DeliveryDeadline = System.DateTime.Now.AddDays(1),
                    Active = true
                },
                new Assignment_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2",
                    ClassroomId = 2,
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
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },

                new Classroom_SQLDTO()
                {
                    Id = 2,
                    Name = "Classroom2",
                    OrganisationId = 1,
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
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2-ikeameatbol",
                    FullName = "Organisation1/Assigmnent2-BACenComm",
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 3,
                    Name = "Assigmnent2-BACenComm",
                    FullName = "Organisation1/Assigmnent2-BACenComm",
                    OrganisationId = 1,
                    Active = false
                },
                new Repository_SQLDTO()
                {
                    Id = 4,
                    Name = "Assigmnent2-2151215",
                    FullName = "Organisation1/Assigmnent2-2151215",
                    OrganisationId = 1,
                    Active = true
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

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
                    ClassroomId = 3,
                    DistributionDate = System.DateTime.Now,
                    Description = "Review a partner\'s code",
                    DeliveryDeadline = System.DateTime.Now.AddDays(1),
                    Active = true
                },
                new Assignment_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2",
                    ClassroomId = 2,
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
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },

                new Classroom_SQLDTO()
                {
                    Id = 2,
                    Name = "Classroom2",
                    OrganisationId = 1,
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
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2-ikeameatbol",
                    FullName = "Organisation1/Assigmnent2-BACenComm",
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 3,
                    Name = "Assigmnent2-BACenComm",
                    FullName = "Organisation1/Assigmnent2-BACenComm",
                    OrganisationId = 1,
                    Active = false
                },
                new Repository_SQLDTO()
                {
                    Id = 4,
                    Name = "Assigmnent2-2151215",
                    FullName = "Organisation1/Assigmnent2-2151215",
                    OrganisationId = 1,
                    Active = true
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

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
                    ClassroomId = 1,
                    DistributionDate = System.DateTime.Now,
                    Description = "Review a partner\'s code",
                    DeliveryDeadline = System.DateTime.Now.AddDays(1),
                    Active = true
                },
                new Assignment_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2",
                    ClassroomId = 2,
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
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },

                new Classroom_SQLDTO()
                {
                    Id = 2,
                    Name = "Classroom2",
                    OrganisationId = 1,
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
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2-ikeameatbol",
                    FullName = "Organisation1/Assigmnent2-BACenComm",
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 3,
                    Name = "Assigmnent2-BACenComm",
                    FullName = "Organisation1/Assigmnent2-BACenComm",
                    OrganisationId = 1,
                    Active = false
                },
                new Repository_SQLDTO()
                {
                    Id = 4,
                    Name = "Assigmnent2-2151215",
                    FullName = "Organisation1/Assigmnent2-2151215",
                    OrganisationId = 1,
                    Active = true
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

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
            
            
        }

        [Fact]
        public void GetAllocationsByAssignmentID_AssignmentClassroomInactive_ReturnsEmptyList()
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
                    ClassroomId = 1,
                    DistributionDate = System.DateTime.Now,
                    Description = "Review a partner\'s code",
                    DeliveryDeadline = System.DateTime.Now.AddDays(1),
                    Active = true
                },
                new Assignment_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2",
                    ClassroomId = 2,
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
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = false
                },

                new Classroom_SQLDTO()
                {
                    Id = 2,
                    Name = "Classroom2",
                    OrganisationId = 1,
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
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2-ikeameatbol",
                    FullName = "Organisation1/Assigmnent2-BACenComm",
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 3,
                    Name = "Assigmnent2-BACenComm",
                    FullName = "Organisation1/Assigmnent2-BACenComm",
                    OrganisationId = 1,
                    Active = false
                },
                new Repository_SQLDTO()
                {
                    Id = 4,
                    Name = "Assigmnent2-2151215",
                    FullName = "Organisation1/Assigmnent2-2151215",
                    OrganisationId = 1,
                    Active = true
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

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
            
            
        }
        #endregion

        #region GetAllocationsByStudentUsername
        [Fact]
        public void GetAllocationsByStudentUsername_ParamNull_ExceptionThrown()
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
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            string studentUsername = null;

            // Act Assert
            Assert.Throws<ArgumentNullException>(() => { List<Allocation> la = da.GetAllocationsByStudentUsername(studentUsername); });
            
            
        }

        [Fact]
        public void GetAllocationsByStudentUsername_ParamWhitespace_ExceptionThrown()
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
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            string studentUsername = "";

            // Act Assert
            Assert.Throws<ArgumentNullException>(() => { List<Allocation> la = da.GetAllocationsByStudentUsername(studentUsername); });
            
            
        }

        [Fact]
        public void GetAllocationsByStudentUsername_List5With3SearchedStudent1Inactive_ReturnsListOf2()
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

            List<Student_SQLDTO> studentsDTO = new List<Student_SQLDTO>()
            {
                new Student_SQLDTO()
                {
                    Id = 1,
                    Username = "Student1",
                    FirstName = "Stud",
                    LastName = "Ent1",
                    Email = "Student1@hotmail.com",
                    Matricule = "1141200",
                    Classes = new List<Classroom_SQLDTO>(),
                    Active = true
                },
                new Student_SQLDTO()
                {
                    Id = 2,
                    Username = "Student2",
                    FirstName = "Stud",
                    LastName = "Ent2",
                    Email = "Student2@hotmail.com",
                    Matricule = "1122334",
                    Active = true
                },
                new Student_SQLDTO()
                {
                    Id = 3,
                    Username = "Student3",
                    FirstName = "Stud",
                    LastName = "Ent3",
                    Email = "Student3@hotmail.com",
                    Matricule = "1324354",
                    Active = false
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            context.Setup(x => x.Students).ReturnsDbSet(studentsDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            string studentUsername = "Student2";

            // Act
            List<Allocation> la = da.GetAllocationsByStudentUsername(studentUsername);

            // Assert
            Assert.NotNull(la);
            Assert.Equal(2, la.Count);
            Assert.Contains(la, a => a.Id == "r1s2t0");
            Assert.Contains(la, a => a.Id == "r2s2t0");
            Assert.DoesNotContain(la, a => a.Id == "r3s2t0");
            
            
        }

        [Fact]
        public void GetAllocationsByStudentUsername_List5With0SearchedStudent_ReturnsEmptyList()
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

            List<Student_SQLDTO> studentsDTO = new List<Student_SQLDTO>()
            {
                new Student_SQLDTO()
                {
                    Id = 1,
                    Username = "Student1",
                    FirstName = "Stud",
                    LastName = "Ent1",
                    Email = "Student1@hotmail.com",
                    Matricule = "1141200",
                    Classes = new List<Classroom_SQLDTO>(),
                    Active = true
                },
                new Student_SQLDTO()
                {
                    Id = 2,
                    Username = "Student2",
                    FirstName = "Stud",
                    LastName = "Ent2",
                    Email = "Student2@hotmail.com",
                    Matricule = "1122334",
                    Active = true
                },
                new Student_SQLDTO()
                {
                    Id = 3,
                    Username = "Student3",
                    FirstName = "Stud",
                    LastName = "Ent3",
                    Email = "Student3@hotmail.com",
                    Matricule = "1324354",
                    Active = true
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            context.Setup(x => x.Students).ReturnsDbSet(studentsDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            string studentUsername = "Student3";

            // Act
            List<Allocation> la = da.GetAllocationsByStudentUsername(studentUsername);

            // Assert
            Assert.NotNull(la);
            Assert.Empty(la);
            
            
        }

        [Fact]
        public void GetAllocationsByStudentUsername_List5WithSearchedStudentNotInBd_ReturnsEmptyList()
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

            List<Student_SQLDTO> studentsDTO = new List<Student_SQLDTO>()
            {
                new Student_SQLDTO()
                {
                    Id = 1,
                    Username = "Student1",
                    FirstName = "Stud",
                    LastName = "Ent1",
                    Email = "Student1@hotmail.com",
                    Matricule = "1141200",
                    Classes = new List<Classroom_SQLDTO>(),
                    Active = true
                },
                new Student_SQLDTO()
                {
                    Id = 2,
                    Username = "Student2",
                    FirstName = "Stud",
                    LastName = "Ent2",
                    Email = "Student2@hotmail.com",
                    Matricule = "1122334",
                    Active = true
                },
                new Student_SQLDTO()
                {
                    Id = 3,
                    Username = "Student3",
                    FirstName = "Stud",
                    LastName = "Ent3",
                    Email = "Student3@hotmail.com",
                    Matricule = "1324354",
                    Active = false
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            context.Setup(x => x.Students).ReturnsDbSet(studentsDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            string studentUsername = "Student5";

            // Act
            List<Allocation> la = da.GetAllocationsByStudentUsername(studentUsername);

            // Assert
            Assert.NotNull(la);
            Assert.Empty(la);
        }

        [Fact]
        public void GetAllocationsByStudentUsername_NoAllocationsDB_ReturnsEmptyList()
        {
            // Arrange
            List<Allocation_SQLDTO> allocationDTO = new List<Allocation_SQLDTO>();

            List<Student_SQLDTO> studentsDTO = new List<Student_SQLDTO>()
            {
                new Student_SQLDTO()
                {
                    Id = 1,
                    Username = "Student1",
                    FirstName = "Stud",
                    LastName = "Ent1",
                    Email = "Student1@hotmail.com",
                    Matricule = "1141200",
                    Classes = new List<Classroom_SQLDTO>(),
                    Active = true
                },
                new Student_SQLDTO()
                {
                    Id = 2,
                    Username = "Student2",
                    FirstName = "Stud",
                    LastName = "Ent2",
                    Email = "Student2@hotmail.com",
                    Matricule = "1122334",
                    Active = true
                },
                new Student_SQLDTO()
                {
                    Id = 3,
                    Username = "Student3",
                    FirstName = "Stud",
                    LastName = "Ent3",
                    Email = "Student3@hotmail.com",
                    Matricule = "1324354",
                    Active = false
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            context.Setup(x => x.Students).ReturnsDbSet(studentsDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            string studentUsername = "Student2";

            // Act
            List<Allocation> la = da.GetAllocationsByStudentUsername(studentUsername);

            // Assert
            Assert.NotNull(la);
            Assert.Empty(la);
            
            
        }
        #endregion

        #region GetAllocationsByRepositoryName
        [Fact]
        public void GetAllocationsByRepositoryName_ParamNull_ExceptionThrown()
        {
            // Arrange

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            DepotAllocation da = new DepotAllocation(context.Object);

            string repositoryName = null;

            // Act Assert
            Assert.Throws<ArgumentNullException>(() => { List<Allocation> la = da.GetAllocationsByRepositoryName(repositoryName); });
            
            
        }

        [Fact]
        public void GetAllocationsByRepositoryName_ParamWhitespace_ExceptionThrown()
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
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            string repositoryName = "";

            // Act Assert
            Assert.Throws<ArgumentNullException>(() => { List<Allocation> la = da.GetAllocationsByRepositoryName(repositoryName); });
            
            
        }

        [Fact]
        public void GetAllocationsByRepositoryName_List5With3SearchedRepository1Inactive_ReturnsListOf2()
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

            List<Repository_SQLDTO> repositoriesDTO = new List<Repository_SQLDTO>()
            {
                new Repository_SQLDTO()
                {
                    Id = 1,
                    Name = "Assigmnent2-Repo1",
                    FullName = "Organisation1/Assigmnent2-Repo1",
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2-Repo2",
                    FullName = "Organisation1/Assigmnent2-Repo2",
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 3,
                    Name = "Assigmnent2-Repo3",
                    FullName = "Organisation1/Assigmnent2-Repo3",
                    OrganisationId = 1,
                    Active = false
                },
                new Repository_SQLDTO()
                {
                    Id = 4,
                    Name = "Assigmnent1-Repo4",
                    FullName = "Organisation1/Assigmnent1-Repo4",
                    OrganisationId = 1,
                    Active = true
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            string repositoryName = "Assigmnent2-Repo1";

            // Act
            List<Allocation> la = da.GetAllocationsByRepositoryName(repositoryName);

            // Assert
            Assert.NotNull(la);
            Assert.Equal(2, la.Count);
            Assert.Contains(la, a => a.Id == "r1s2t0");
            Assert.Contains(la, a => a.Id == "r1s4t0");
            Assert.DoesNotContain(la, a => a.Id == "r1s1t0");
            
            
        }

        [Fact]
        public void GetAllocationsByRepositoryName_List4With0SearchedRepository_ReturnsEmptyList()
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

            List<Repository_SQLDTO> repositoriesDTO = new List<Repository_SQLDTO>()
            {
                new Repository_SQLDTO()
                {
                    Id = 1,
                    Name = "Assigmnent2-Repo1",
                    FullName = "Organisation1/Assigmnent2-Repo1",
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2-Repo2",
                    FullName = "Organisation1/Assigmnent2-Repo2",
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 3,
                    Name = "Assigmnent2-Repo3",
                    FullName = "Organisation1/Assigmnent2-Repo3",
                    OrganisationId = 1,
                    Active = false
                },
                new Repository_SQLDTO()
                {
                    Id = 4,
                    Name = "Assigmnent1-Repo4",
                    FullName = "Organisation1/Assigmnent1-Repo4",
                    OrganisationId = 1,
                    Active = true
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            string repositoryName = "Assigmnent1-Repo4";

            // Act
            List<Allocation> la = da.GetAllocationsByRepositoryName(repositoryName);

            // Assert
            Assert.NotNull(la);
            Assert.Empty(la);
            
            
        }

        [Fact]
        public void GetAllocationsByRepositoryName_NoAllocationsDB_ReturnsEmptyList()
        {
            // Arrange
            List<Allocation_SQLDTO> allocationDTO = new List<Allocation_SQLDTO>();
            List<Repository_SQLDTO> repositoriesDTO = new List<Repository_SQLDTO>()
            {
                new Repository_SQLDTO()
                {
                    Id = 1,
                    Name = "Assigmnent2-Repo1",
                    FullName = "Organisation1/Assigmnent2-Repo1",
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2-Repo2",
                    FullName = "Organisation1/Assigmnent2-Repo2",
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 3,
                    Name = "Assigmnent2-Repo3",
                    FullName = "Organisation1/Assigmnent2-Repo3",
                    OrganisationId = 1,
                    Active = false
                },
                new Repository_SQLDTO()
                {
                    Id = 4,
                    Name = "Assigmnent1-Repo4",
                    FullName = "Organisation1/Assigmnent1-Repo4",
                    OrganisationId = 1,
                    Active = true
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            string repositoryName = "Assigmnent2-Repo2";

            // Act
            List<Allocation> la = da.GetAllocationsByRepositoryName(repositoryName);

            // Assert
            Assert.NotNull(la);
            Assert.Empty(la);
            
            
        }

        [Fact]
        public void GetAllocationsByRepositoryName_SearchedRepositoryNotInDB_ReturnsEmptyList()
        {
            // Arrange
            List<Allocation_SQLDTO> allocationDTO = new List<Allocation_SQLDTO>();
            List<Repository_SQLDTO> repositoriesDTO = new List<Repository_SQLDTO>()
            {
                new Repository_SQLDTO()
                {
                    Id = 1,
                    Name = "Assigmnent2-Repo1",
                    FullName = "Organisation1/Assigmnent2-Repo1",
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2-Repo2",
                    FullName = "Organisation1/Assigmnent2-Repo2",
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 3,
                    Name = "Assigmnent2-Repo3",
                    FullName = "Organisation1/Assigmnent2-Repo3",
                    OrganisationId = 1,
                    Active = false
                },
                new Repository_SQLDTO()
                {
                    Id = 4,
                    Name = "Assigmnent1-Repo4",
                    FullName = "Organisation1/Assigmnent1-Repo4",
                    OrganisationId = 1,
                    Active = true
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            string repositoryName = "Assigmnent2-Repo5";

            // Act
            List<Allocation> la = da.GetAllocationsByRepositoryName(repositoryName);

            // Assert
            Assert.NotNull(la);
            Assert.Empty(la);
        }
        #endregion

        #region GetAllocationByStudentAndRepositoryNames
        [Fact]
        public void GetAllocationByStudentAndRepositoryNames_ParamStudentUsernameNull_ExceptionThrown()
        {
            // Arrange
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            DepotAllocation da = new DepotAllocation(context.Object);

            string studentUsername = null;
            string repositoryName = "Assigmnent2-Repo2";

            // Act Assert
            Assert.Throws<ArgumentNullException>(() => { Allocation? a = da.GetAllocationByStudentAndRepositoryNames(studentUsername, repositoryName); });
            
            
        }

        [Fact]
        public void GetAllocationByStudentAndRepositoryNames_ParamRepositoryNameNull_ExceptionThrown()
        {
            // Arrange

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            DepotAllocation da = new DepotAllocation(context.Object);

            string studentUsername = "Student2";
            string repositoryName = null;

            // Act Assert
            Assert.Throws<ArgumentNullException>(() => { Allocation? a = da.GetAllocationByStudentAndRepositoryNames(studentUsername, repositoryName); });
            
            
        }

        [Fact]
        public void GetAllocationByStudentAndRepositoryNames_ParamStudentUsernameWhiteSpace_ExceptionThrown()
        {
            // Arrange

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            DepotAllocation da = new DepotAllocation(context.Object);

            string studentUsername = " ";
            string repositoryName = "Assigmnent2-Repo2";

            // Act Assert
            Assert.Throws<ArgumentNullException>(() => { Allocation? a = da.GetAllocationByStudentAndRepositoryNames(studentUsername, repositoryName); });
            
            
        }

        [Fact]
        public void GetAllocationByStudentAndRepositoryNamess_ParamRepositoryNameWhiteSpace_ExceptionThrown()
        {
            // Arrange

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            DepotAllocation da = new DepotAllocation(context.Object);

            string studentUsername = "Repo2";
            string repositoryName = "       ";

            // Act Assert
            Assert.Throws<ArgumentNullException>(() => { Allocation? a = da.GetAllocationByStudentAndRepositoryNames(studentUsername, repositoryName); });
            
            
        }

        [Fact]
        public void GetAllocationByStudentAndRepositoryNames_List5WithSearchedAllocationPresent_ReturnsAllocation()
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
            List<Student_SQLDTO> studentsDTO = new List<Student_SQLDTO>()
            {
                new Student_SQLDTO()
                {
                    Id = 1,
                    Username = "Student1",
                    FirstName = "Stud",
                    LastName = "Ent1",
                    Email = "Student1@hotmail.com",
                    Matricule = "1141200",
                    Classes = new List<Classroom_SQLDTO>(),
                    Active = true
                },
                new Student_SQLDTO()
                {
                    Id = 2,
                    Username = "Student2",
                    FirstName = "Stud",
                    LastName = "Ent2",
                    Email = "Student2@hotmail.com",
                    Matricule = "1122334",
                    Active = true
                },
                new Student_SQLDTO()
                {
                    Id = 3,
                    Username = "Student3",
                    FirstName = "Stud",
                    LastName = "Ent3",
                    Email = "Student3@hotmail.com",
                    Matricule = "1324354",
                    Active = false
                },
                new Student_SQLDTO()
                {
                    Id = 4,
                    Username = "Student4",
                    FirstName = "Stud",
                    LastName = "Ent4",
                    Email = "Student4@hotmail.com",
                    Matricule = "2152153",
                    Active = true
                }
            };
            List<Repository_SQLDTO> repositoriesDTO = new List<Repository_SQLDTO>()
            {
                new Repository_SQLDTO()
                {
                    Id = 1,
                    Name = "Assigmnent2-Repo1",
                    FullName = "Organisation1/Assigmnent2-Repo1",
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2-Repo2",
                    FullName = "Organisation1/Assigmnent2-Repo2",
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 3,
                    Name = "Assigmnent2-Repo3",
                    FullName = "Organisation1/Assigmnent2-Repo3",
                    OrganisationId = 1,
                    Active = false
                },
                new Repository_SQLDTO()
                {
                    Id = 4,
                    Name = "Assigmnent1-Repo4",
                    FullName = "Organisation1/Assigmnent1-Repo4",
                    OrganisationId = 1,
                    Active = true
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDTO);
            context.Setup(x => x.Students).ReturnsDbSet(studentsDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            string studentUsername = "Student4";
            string repositoryName = "Assigmnent2-Repo1";

            string expectedAllocationId = "r1s4t0";

            // Act
            Allocation a = da.GetAllocationByStudentAndRepositoryNames(studentUsername, repositoryName);

            // Assert
            Assert.NotNull(a);
            Assert.Equal(expectedAllocationId, a.Id);
            
            
        }

        [Fact]
        public void GetAllocationByStudentAndRepositoryNames_List5WithSearchedAllocationNotPresent_ReturnsNull()
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
            List<Student_SQLDTO> studentsDTO = new List<Student_SQLDTO>()
            {
                new Student_SQLDTO()
                {
                    Id = 1,
                    Username = "Student1",
                    FirstName = "Stud",
                    LastName = "Ent1",
                    Email = "Student1@hotmail.com",
                    Matricule = "1141200",
                    Classes = new List<Classroom_SQLDTO>(),
                    Active = true
                },
                new Student_SQLDTO()
                {
                    Id = 2,
                    Username = "Student2",
                    FirstName = "Stud",
                    LastName = "Ent2",
                    Email = "Student2@hotmail.com",
                    Matricule = "1122334",
                    Active = true
                },
                new Student_SQLDTO()
                {
                    Id = 3,
                    Username = "Student3",
                    FirstName = "Stud",
                    LastName = "Ent3",
                    Email = "Student3@hotmail.com",
                    Matricule = "1324354",
                    Active = false
                },
                new Student_SQLDTO()
                {
                    Id = 4,
                    Username = "Student4",
                    FirstName = "Stud",
                    LastName = "Ent4",
                    Email = "Student4@hotmail.com",
                    Matricule = "2152153",
                    Active = true
                }
            };
            List<Repository_SQLDTO> repositoriesDTO = new List<Repository_SQLDTO>()
            {
                new Repository_SQLDTO()
                {
                    Id = 1,
                    Name = "Assigmnent2-Repo1",
                    FullName = "Organisation1/Assigmnent2-Repo1",
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2-Repo2",
                    FullName = "Organisation1/Assigmnent2-Repo2",
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 3,
                    Name = "Assigmnent2-Repo3",
                    FullName = "Organisation1/Assigmnent2-Repo3",
                    OrganisationId = 1,
                    Active = false
                },
                new Repository_SQLDTO()
                {
                    Id = 4,
                    Name = "Assigmnent1-Repo4",
                    FullName = "Organisation1/Assigmnent1-Repo4",
                    OrganisationId = 1,
                    Active = true
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDTO);
            context.Setup(x => x.Students).ReturnsDbSet(studentsDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            string studentUsername = "Student4";
            string repositoryName = "Assigmnent2-Repo2";

            // Act
            Allocation? a = da.GetAllocationByStudentAndRepositoryNames(studentUsername, repositoryName);

            // Assert
            Assert.Null(a);
            
            
        }

        [Fact]
        public void GetAllocationByStudentAndRepositoryNames_List5WithSearchedAllocationPresentButInactive_ReturnsNull()
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
            List<Student_SQLDTO> studentsDTO = new List<Student_SQLDTO>()
            {
                new Student_SQLDTO()
                {
                    Id = 1,
                    Username = "Student1",
                    FirstName = "Stud",
                    LastName = "Ent1",
                    Email = "Student1@hotmail.com",
                    Matricule = "1141200",
                    Classes = new List<Classroom_SQLDTO>(),
                    Active = true
                },
                new Student_SQLDTO()
                {
                    Id = 2,
                    Username = "Student2",
                    FirstName = "Stud",
                    LastName = "Ent2",
                    Email = "Student2@hotmail.com",
                    Matricule = "1122334",
                    Active = true
                },
                new Student_SQLDTO()
                {
                    Id = 3,
                    Username = "Student3",
                    FirstName = "Stud",
                    LastName = "Ent3",
                    Email = "Student3@hotmail.com",
                    Matricule = "1324354",
                    Active = false
                },
                new Student_SQLDTO()
                {
                    Id = 4,
                    Username = "Student4",
                    FirstName = "Stud",
                    LastName = "Ent4",
                    Email = "Student4@hotmail.com",
                    Matricule = "2152153",
                    Active = true
                }
            };
            List<Repository_SQLDTO> repositoriesDTO = new List<Repository_SQLDTO>()
            {
                new Repository_SQLDTO()
                {
                    Id = 1,
                    Name = "Assigmnent2-Repo1",
                    FullName = "Organisation1/Assigmnent2-Repo1",
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2-Repo2",
                    FullName = "Organisation1/Assigmnent2-Repo2",
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 3,
                    Name = "Assigmnent2-Repo3",
                    FullName = "Organisation1/Assigmnent2-Repo3",
                    OrganisationId = 1,
                    Active = false
                },
                new Repository_SQLDTO()
                {
                    Id = 4,
                    Name = "Assigmnent1-Repo4",
                    FullName = "Organisation1/Assigmnent1-Repo4",
                    OrganisationId = 1,
                    Active = true
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDTO);
            context.Setup(x => x.Students).ReturnsDbSet(studentsDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            string studentUsername = "Student1";
            string repositoryName = "Assigmnent2-Repo1";

            // Act
            Allocation? a = da.GetAllocationByStudentAndRepositoryNames(studentUsername, repositoryName);

            // Assert
            Assert.Null(a);
            
            
        }

        [Fact]
        public void GetAllocationByStudentAndRepositoryNames_List5WithSearchedAllocationPresentAndActiveTwice_ReturnsNull()
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
            List<Student_SQLDTO> studentsDTO = new List<Student_SQLDTO>()
            {
                new Student_SQLDTO()
                {
                    Id = 1,
                    Username = "Student1",
                    FirstName = "Stud",
                    LastName = "Ent1",
                    Email = "Student1@hotmail.com",
                    Matricule = "1141200",
                    Classes = new List<Classroom_SQLDTO>(),
                    Active = true
                },
                new Student_SQLDTO()
                {
                    Id = 2,
                    Username = "Student2",
                    FirstName = "Stud",
                    LastName = "Ent2",
                    Email = "Student2@hotmail.com",
                    Matricule = "1122334",
                    Active = true
                },
                new Student_SQLDTO()
                {
                    Id = 3,
                    Username = "Student3",
                    FirstName = "Stud",
                    LastName = "Ent3",
                    Email = "Student3@hotmail.com",
                    Matricule = "1324354",
                    Active = false
                },
                new Student_SQLDTO()
                {
                    Id = 4,
                    Username = "Student4",
                    FirstName = "Stud",
                    LastName = "Ent4",
                    Email = "Student4@hotmail.com",
                    Matricule = "2152153",
                    Active = true
                }
            };
            List<Repository_SQLDTO> repositoriesDTO = new List<Repository_SQLDTO>()
            {
                new Repository_SQLDTO()
                {
                    Id = 1,
                    Name = "Assigmnent2-Repo1",
                    FullName = "Organisation1/Assigmnent2-Repo1",
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2-Repo2",
                    FullName = "Organisation1/Assigmnent2-Repo2",
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 3,
                    Name = "Assigmnent2-Repo3",
                    FullName = "Organisation1/Assigmnent2-Repo3",
                    OrganisationId = 1,
                    Active = false
                },
                new Repository_SQLDTO()
                {
                    Id = 4,
                    Name = "Assigmnent1-Repo4",
                    FullName = "Organisation1/Assigmnent1-Repo4",
                    OrganisationId = 1,
                    Active = true
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDTO);
            context.Setup(x => x.Students).ReturnsDbSet(studentsDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            string studentUsername = "Student2";
            string repositoryName = "Assigmnent2-Repo1";

            // Act
            Allocation? a = da.GetAllocationByStudentAndRepositoryNames(studentUsername, repositoryName);

            // Assert
            Assert.Null(a);
            
            
        }

        [Fact]
        public void GetAllocationByStudentAndRepositoryNames_List5WithSearchedAllocationPresentTwiceOneInactive_ReturnsActiveAllocation()
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
            List<Student_SQLDTO> studentsDTO = new List<Student_SQLDTO>()
            {
                new Student_SQLDTO()
                {
                    Id = 1,
                    Username = "Student1",
                    FirstName = "Stud",
                    LastName = "Ent1",
                    Email = "Student1@hotmail.com",
                    Matricule = "1141200",
                    Classes = new List<Classroom_SQLDTO>(),
                    Active = true
                },
                new Student_SQLDTO()
                {
                    Id = 2,
                    Username = "Student2",
                    FirstName = "Stud",
                    LastName = "Ent2",
                    Email = "Student2@hotmail.com",
                    Matricule = "1122334",
                    Active = true
                },
                new Student_SQLDTO()
                {
                    Id = 3,
                    Username = "Student3",
                    FirstName = "Stud",
                    LastName = "Ent3",
                    Email = "Student3@hotmail.com",
                    Matricule = "1324354",
                    Active = false
                },
                new Student_SQLDTO()
                {
                    Id = 4,
                    Username = "Student4",
                    FirstName = "Stud",
                    LastName = "Ent4",
                    Email = "Student4@hotmail.com",
                    Matricule = "2152153",
                    Active = true
                }
            };
            List<Repository_SQLDTO> repositoriesDTO = new List<Repository_SQLDTO>()
            {
                new Repository_SQLDTO()
                {
                    Id = 1,
                    Name = "Assigmnent2-Repo1",
                    FullName = "Organisation1/Assigmnent2-Repo1",
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2-Repo2",
                    FullName = "Organisation1/Assigmnent2-Repo2",
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 3,
                    Name = "Assigmnent2-Repo3",
                    FullName = "Organisation1/Assigmnent2-Repo3",
                    OrganisationId = 1,
                    Active = false
                },
                new Repository_SQLDTO()
                {
                    Id = 4,
                    Name = "Assigmnent1-Repo4",
                    FullName = "Organisation1/Assigmnent1-Repo4",
                    OrganisationId = 1,
                    Active = true
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDTO);
            context.Setup(x => x.Students).ReturnsDbSet(studentsDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            string studentUsername = "Student2";
            string repositoryName = "Assigmnent2-Repo1";

            string expectedAllocationId = "r1s2t0";

            // Act
            Allocation? a = da.GetAllocationByStudentAndRepositoryNames(studentUsername, repositoryName);

            // Assert
            Assert.NotNull(a);
            Assert.Equal(expectedAllocationId, a.Id);
            
            
        }
        #endregion

        #region GetAllocationsByAssignmentName
        [Fact]
        public void GetAllocationsByAssignmentName_ParamAssignmentNameNull_ExceptionThrown()
        {
            // Arrange
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            DepotAllocation da = new DepotAllocation(context.Object);

            string p_assignmentName = null;

            // Act Assert
            Assert.Throws<ArgumentNullException>(() => { List<Allocation> a = da.GetAllocationsByAssignmentName(p_assignmentName); });
            
            
        }

        [Fact]
        public void GetAllocationsByAssignmentName_ParamAssignmentNameWhiteSpace_ExceptionThrown()
        {
            // Arrange
            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            DepotAllocation da = new DepotAllocation(context.Object);

            string p_assignmentName = " ";

            // Act Assert
            Assert.Throws<ArgumentNullException>(() => { List<Allocation> a = da.GetAllocationsByAssignmentName(p_assignmentName); });
            
            
        }

        [Fact]
        public void GetAllocationsByAssignmentName_List5AllocationsAllInAssignment_ReturnsListOf5()
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
                    ClassroomId = 2,
                    DistributionDate = System.DateTime.Now,
                    Description = "Review a partner\'s code",
                    DeliveryDeadline = System.DateTime.Now.AddDays(1),
                    Active = true
                },
                new Assignment_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2",
                    ClassroomId = 2,
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
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },

                new Classroom_SQLDTO()
                {
                    Id = 2,
                    Name = "Classroom2",
                    OrganisationId = 1,
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
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2-ikeameatbol",
                    FullName = "Organisation1/Assigmnent2-BACenComm",
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 3,
                    Name = "Assigmnent2-BACenComm",
                    FullName = "Organisation1/Assigmnent2-BACenComm",
                    OrganisationId = 1,
                    Active = false
                },
                new Repository_SQLDTO()
                {
                    Id = 4,
                    Name = "Assigmnent1-2151215",
                    FullName = "Organisation1/Assigmnent1-2151215",
                    OrganisationId = 1,
                    Active = true
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationsDTO);
            context.Setup(x => x.Assignments).ReturnsDbSet(assignmentsDTO);
            context.Setup(x => x.Classrooms).ReturnsDbSet(classroomsDTO);
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            string assignmentName = "Assigmnent2";

            // Act
            List<Allocation> la = da.GetAllocationsByAssignmentName(assignmentName);

            // Assert
            Assert.NotNull(la);
            Assert.Equal(5, la.Count);
            Assert.Contains(la, a => a.Id == "r1s2t0");
            Assert.Contains(la, a => a.Id == "r1s4t0");
            Assert.Contains(la, a => a.Id == "r2s0t1");
            Assert.Contains(la, a => a.Id == "r2s2t0");
            Assert.Contains(la, a => a.Id == "r1s1t0");            
        }

        [Fact]
        public void GetAllocationsByAssignmentName_List5AllocationsNoneInAssignment_ReturnsEmptyList()
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
                    ClassroomId = 2,
                    DistributionDate = System.DateTime.Now,
                    Description = "Review a partner\'s code",
                    DeliveryDeadline = System.DateTime.Now.AddDays(1),
                    Active = true
                },
                new Assignment_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2",
                    ClassroomId = 2,
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
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },

                new Classroom_SQLDTO()
                {
                    Id = 2,
                    Name = "Classroom2",
                    OrganisationId = 1,
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
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2-ikeameatbol",
                    FullName = "Organisation1/Assigmnent2-BACenComm",
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 3,
                    Name = "Assigmnent2-BACenComm",
                    FullName = "Organisation1/Assigmnent2-BACenComm",
                    OrganisationId = 1,
                    Active = false
                },
                new Repository_SQLDTO()
                {
                    Id = 4,
                    Name = "Assigmnent1-2151215",
                    FullName = "Organisation1/Assigmnent1-2151215",
                    OrganisationId = 1,
                    Active = true
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationsDTO);
            context.Setup(x => x.Assignments).ReturnsDbSet(assignmentsDTO);
            context.Setup(x => x.Classrooms).ReturnsDbSet(classroomsDTO);
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            string assignmentName = "Assigmnent2";

            // Act
            List<Allocation> la = da.GetAllocationsByAssignmentName(assignmentName);

            // Assert
            Assert.NotNull(la);
            Assert.Empty(la);
        }

        [Fact]
        public void GetAllocationsByAssignmentName_List5AllocationsNoRepositoryInAssignment_ReturnsEmptyList()
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
                    ClassroomId = 2,
                    DistributionDate = System.DateTime.Now,
                    Description = "Review a partner\'s code",
                    DeliveryDeadline = System.DateTime.Now.AddDays(1),
                    Active = true
                },
                new Assignment_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2",
                    ClassroomId = 2,
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
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },

                new Classroom_SQLDTO()
                {
                    Id = 2,
                    Name = "Classroom2",
                    OrganisationId = 1,
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
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2-ikeameatbol",
                    FullName = "Organisation1/Assigmnent2-BACenComm",
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 3,
                    Name = "Assigmnent2-BACenComm",
                    FullName = "Organisation1/Assigmnent2-BACenComm",
                    OrganisationId = 1,
                    Active = false
                },
                new Repository_SQLDTO()
                {
                    Id = 4,
                    Name = "Assigmnent2-2151215",
                    FullName = "Organisation1/Assigmnent2-2151215",
                    OrganisationId = 1,
                    Active = true
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationsDTO);
            context.Setup(x => x.Assignments).ReturnsDbSet(assignmentsDTO);
            context.Setup(x => x.Classrooms).ReturnsDbSet(classroomsDTO);
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            string assignmentName = "Assigmnent1";

            // Act
            List<Allocation> la = da.GetAllocationsByAssignmentName(assignmentName);

            // Assert
            Assert.NotNull(la);
            Assert.Empty(la);
        }

        [Fact]
        public void GetAllocationsByAssignmentName_List5AllocationsClassroomInactiveInAssignment_ReturnsEmptyList()
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
                    ClassroomId = 3,
                    DistributionDate = System.DateTime.Now,
                    Description = "Review a partner\'s code",
                    DeliveryDeadline = System.DateTime.Now.AddDays(1),
                    Active = true
                },
                new Assignment_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2",
                    ClassroomId = 2,
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
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },

                new Classroom_SQLDTO()
                {
                    Id = 2,
                    Name = "Classroom2",
                    OrganisationId = 1,
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
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2-ikeameatbol",
                    FullName = "Organisation1/Assigmnent2-BACenComm",
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 3,
                    Name = "Assigmnent2-BACenComm",
                    FullName = "Organisation1/Assigmnent2-BACenComm",
                    OrganisationId = 1,
                    Active = false
                },
                new Repository_SQLDTO()
                {
                    Id = 4,
                    Name = "Assigmnent2-2151215",
                    FullName = "Organisation1/Assigmnent2-2151215",
                    OrganisationId = 1,
                    Active = true
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationsDTO);
            context.Setup(x => x.Assignments).ReturnsDbSet(assignmentsDTO);
            context.Setup(x => x.Classrooms).ReturnsDbSet(classroomsDTO);
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            string assignmentName = "Assigmnent1";

            // Act
            List<Allocation> la = da.GetAllocationsByAssignmentName(assignmentName);

            // Assert
            Assert.NotNull(la);
            Assert.Empty(la);
            
            
        }

        [Fact]
        public void GetAllocationsByAssignmentName_List5AllocationsAssignmentNonexistant_ReturnsEmptyList()
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
                    ClassroomId = 1,
                    DistributionDate = System.DateTime.Now,
                    Description = "Review a partner\'s code",
                    DeliveryDeadline = System.DateTime.Now.AddDays(1),
                    Active = true
                },
                new Assignment_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2",
                    ClassroomId = 2,
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
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },

                new Classroom_SQLDTO()
                {
                    Id = 2,
                    Name = "Classroom2",
                    OrganisationId = 1,
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
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2-ikeameatbol",
                    FullName = "Organisation1/Assigmnent2-BACenComm",
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 3,
                    Name = "Assigmnent2-BACenComm",
                    FullName = "Organisation1/Assigmnent2-BACenComm",
                    OrganisationId = 1,
                    Active = false
                },
                new Repository_SQLDTO()
                {
                    Id = 4,
                    Name = "Assigmnent2-2151215",
                    FullName = "Organisation1/Assigmnent2-2151215",
                    OrganisationId = 1,
                    Active = true
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationsDTO);
            context.Setup(x => x.Assignments).ReturnsDbSet(assignmentsDTO);
            context.Setup(x => x.Classrooms).ReturnsDbSet(classroomsDTO);
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            string assignmentName = "Assigmnent3";

            // Act
            List<Allocation> la = da.GetAllocationsByAssignmentName(assignmentName);

            // Assert
            Assert.NotNull(la);
            Assert.Empty(la);
            
            
        }

        [Fact]
        public void GetAllocationsByAssignmentName_AssignmentClassroomInactive_ReturnsEmptyList()
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
                    ClassroomId = 1,
                    DistributionDate = System.DateTime.Now,
                    Description = "Review a partner\'s code",
                    DeliveryDeadline = System.DateTime.Now.AddDays(1),
                    Active = true
                },
                new Assignment_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2",
                    ClassroomId = 2,
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
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = false
                },

                new Classroom_SQLDTO()
                {
                    Id = 2,
                    Name = "Classroom2",
                    OrganisationId = 1,
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
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2-ikeameatbol",
                    FullName = "Organisation1/Assigmnent2-BACenComm",
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 3,
                    Name = "Assigmnent2-BACenComm",
                    FullName = "Organisation1/Assigmnent2-BACenComm",
                    OrganisationId = 1,
                    Active = false
                },
                new Repository_SQLDTO()
                {
                    Id = 4,
                    Name = "Assigmnent2-2151215",
                    FullName = "Organisation1/Assigmnent2-2151215",
                    OrganisationId = 1,
                    Active = true
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationsDTO);
            context.Setup(x => x.Assignments).ReturnsDbSet(assignmentsDTO);
            context.Setup(x => x.Classrooms).ReturnsDbSet(classroomsDTO);
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            string assignmentName = "Assigmnent1";

            // Act
            List<Allocation> la = da.GetAllocationsByAssignmentName(assignmentName);

            // Assert
            Assert.NotNull(la);
            Assert.Empty(la);
            
            
        }

        [Fact]
        public void GetAllocationsByAssignmentName_2ActiveAssignmentsWithSameName_ReturnsListOf5()
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
                    ClassroomId = 2,
                    DistributionDate = System.DateTime.Now,
                    Description = "Review a partner\'s code",
                    DeliveryDeadline = System.DateTime.Now.AddDays(1),
                    Active = true
                },
                new Assignment_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2",
                    ClassroomId = 2,
                    DistributionDate = System.DateTime.Now,
                    Description = "Review another partner\'s code",
                    DeliveryDeadline = System.DateTime.Now.AddDays(1),
                    Active = true
                },
                new Assignment_SQLDTO()
                {
                    Id = 3,
                    Name = "Assigmnent1",
                    ClassroomId = 1,
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
                    OrganisationId = 1,
                    Assignments = new List<Assignment_SQLDTO>(),
                    Students = new List<Student_SQLDTO>(),
                    Teachers = new List<Teacher_SQLDTO>(),
                    Active = true
                },

                new Classroom_SQLDTO()
                {
                    Id = 2,
                    Name = "Classroom2",
                    OrganisationId = 1,
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
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 2,
                    Name = "Assigmnent2-ikeameatbol",
                    FullName = "Organisation1/Assigmnent2-BACenComm",
                    OrganisationId = 1,
                    Active = true
                },
                new Repository_SQLDTO()
                {
                    Id = 3,
                    Name = "Assigmnent2-BACenComm",
                    FullName = "Organisation1/Assigmnent2-BACenComm",
                    OrganisationId = 1,
                    Active = false
                },
                new Repository_SQLDTO()
                {
                    Id = 4,
                    Name = "Assigmnent1-2151215",
                    FullName = "Organisation1/Assigmnent1-2151215",
                    OrganisationId = 1,
                    Active = true
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationsDTO);
            context.Setup(x => x.Assignments).ReturnsDbSet(assignmentsDTO);
            context.Setup(x => x.Classrooms).ReturnsDbSet(classroomsDTO);
            context.Setup(x => x.Repositories).ReturnsDbSet(repositoriesDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            string assignmentName = "Assigmnent1";

            // Act
            List<Allocation> la = da.GetAllocationsByAssignmentName(assignmentName);

            // Assert
            Assert.NotNull(la);
            Assert.Empty(la);
            
            
        }
        #endregion

        #region UpsertAllocation
        [Fact]
        public void UpsertAllocation_ParamNull_ExceptionThrown()
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

            Allocation allocation = null;

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            // Act Assert
            Assert.Throws<ArgumentNullException>(() => { da.UpsertAllocation(allocation); });
        }

        [Fact]
        public void UpsertAllocation_AllocationExists_ModifyAllocation()
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
                    Status = 2
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

            Allocation allocation = new Allocation
            {
                Id = "r1s4t0",
                RepositoryId = 1,
                StudentId = 4,
                TeacherId = null,
                Status = 3
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            // Act
            da.UpsertAllocation(allocation);

            // Assert
            //context.Verify(c => c.Update(It.IsAny<Allocation_SQLDTO>()), Times.Once);
            context.Verify(c => c.SaveChanges(), Times.Once);
            
            
        }

        [Fact]
        public void UpsertAllocation_AllocationDoesNotExists_CreateAllocation()
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
                    Status = 2
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

            Allocation allocation = new Allocation
            {
                Id = "r3s4t0",
                RepositoryId = 1,
                StudentId = 4,
                TeacherId = null,
                Status = 2
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            // Act
            da.UpsertAllocation(allocation);

            // Assert
            context.Verify(c => c.Allocations.Add(It.IsAny<Allocation_SQLDTO>()), Times.Once);
            context.Verify(c => c.SaveChanges(), Times.Once);
            
            
        }

        [Fact]
        public void UpsertAllocation_NoAllocationInBd_CreateAllocation()
        {
            // Arrange
            List<Allocation_SQLDTO> allocationDTO = new List<Allocation_SQLDTO>();
            
            Allocation allocation = new Allocation
            {
                Id = "r3s4t0",
                RepositoryId = 1,
                StudentId = 4,
                TeacherId = null,
                Status = 2
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            // Act
            da.UpsertAllocation(allocation);

            // Assert
            context.Verify(c => c.Allocations.Add(It.IsAny<Allocation_SQLDTO>()), Times.Once);
            context.Verify(c => c.SaveChanges(), Times.Once);
            
            
        }

        [Fact]
        public void UpsertAllocation_AllocationInBdTwice1Inactive_ExceptionThrown()
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
                    Status = 2
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
                    Id= "r1s4t0",
                    RepositoryId = 1,
                    StudentId = 4,
                    TeacherId = null,
                    Status = 0
                }
            };

            Allocation allocation = new Allocation
            {
                Id = "r1s4t0",
                RepositoryId = 1,
                StudentId = 4,
                TeacherId = null,
                Status = 3
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            // Act Assert
            Assert.Throws<InvalidOperationException>(() => da.UpsertAllocation(allocation));
            
            
        }
        #endregion

        #region DeleteAllocation
        [Fact]
        public void DeleteAllocation_ParamNull_ExceptionThrown()
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

            Allocation allocation = null;

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            // Act Assert
            Assert.Throws<ArgumentNullException>(() => { da.DeleteAllocation(allocation); });
            
            
        }

        [Fact]
        public void DeleteAllocation_AllocationExists_ModifyStatusOfAllocation()
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
                    Status = 2
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

            Allocation allocation = new Allocation
            {
                Id = "r1s4t0",
                RepositoryId = 1,
                StudentId = 4,
                TeacherId = null,
                Status = 3
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            // Act
            da.DeleteAllocation(allocation);

            // Assert
            //context.Verify(c => c.Update(It.IsAny<Allocation_SQLDTO>()), Times.Once);
            context.Verify(c => c.SaveChanges(), Times.Once);
            
            
        }

        [Fact]
        public void DeleteAllocation_AllocationDoesNotExists_DoesNothing()
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
                    Status = 2
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

            Allocation allocation = new Allocation
            {
                Id = "r3s4t0",
                RepositoryId = 1,
                StudentId = 4,
                TeacherId = null,
                Status = 2
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            // Act
            da.DeleteAllocation(allocation);

            // Assert
            context.Verify(c => c.Update(It.IsAny<Allocation_SQLDTO>()), Times.Never);
            context.Verify(c => c.SaveChanges(), Times.Never);
            
            
        }

        [Fact]
        public void DeleteAllocation_AllocationInBdTwice1Inactive_ExceptionThrown()
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
                    Status = 2
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
                    Id= "r1s4t0",
                    RepositoryId = 1,
                    StudentId = 4,
                    TeacherId = null,
                    Status = 0
                }
            };

            Allocation allocation = new Allocation
            {
                Id = "r1s4t0",
                RepositoryId = 1,
                StudentId = 4,
                TeacherId = null,
                Status = 3
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            // Act Assert
            Assert.Throws<InvalidOperationException>(() => da.DeleteAllocation(allocation));
            
            
        }
        #endregion

        #region UpsertAllocationsBatch
        [Fact]
        public void UpsertAllocationsBatch_ParamNull_ExceptionThrown()
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

            List<Allocation> allocations = null;

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            // Act Assert
            Assert.Throws<ArgumentNullException>(() => { da.UpsertAllocationsBatch(allocations); });
            
            
        }

        [Fact]
        public void UpsertAllocationsBatch_ParamNullInList_ExceptionThrown()
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
                    Status = 1
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
                    Status = 1
                },
                new Allocation_SQLDTO
                {
                    Id= "r2s2t0",
                    RepositoryId = 2,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 1
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

            List<Allocation> allocations = new List<Allocation>
            {
                new Allocation
                {
                    Id= "r1s2t0",
                    RepositoryId = 1,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 3
                },
                new Allocation
                {
                    Id= "r1s4t0",
                    RepositoryId = 1,
                    StudentId = 4,
                    TeacherId = null,
                    Status = 3
                },
                null,
                new Allocation
                {
                    Id= "r2s2t0",
                    RepositoryId = 2,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 3
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            // Act Assert
            Assert.Throws<ArgumentNullException>(() => { da.UpsertAllocationsBatch(allocations); });
            //context.Verify(c => c.Update(It.IsAny<Allocation_SQLDTO>()), Times.Exactly(2));
        }

        [Fact]
        public void UpsertAllocationsBatch_5AllocationList3Exists2Not_Modify3AllocationCreate2()
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
                    Status = 2
                },
                new Allocation_SQLDTO
                {
                    Id= "r1s0t1",
                    RepositoryId = 1,
                    StudentId = null,
                    TeacherId = 1,
                    Status = 3
                }
            };

            List<Allocation> allocations = new List<Allocation>
            {
                new Allocation
                {
                    Id= "r1s2t0",
                    RepositoryId = 1,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 3
                },
                new Allocation
                {
                    Id= "r1s4t0",
                    RepositoryId = 1,
                    StudentId = 4,
                    TeacherId = null,
                    Status = 3
                },
                new Allocation
                {
                    Id= "r1s0t1",
                    RepositoryId = 1,
                    StudentId = null,
                    TeacherId = 1,
                    Status = 3
                },
                new Allocation
                {
                    Id= "r2s2t0",
                    RepositoryId = 2,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 3
                },
                new Allocation
                {
                    Id= "r3s2t0",
                    RepositoryId = 2,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 2
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            // Act
            da.UpsertAllocationsBatch(allocations);

            // Assert
            //context.Verify(c => c.Update(It.IsAny<Allocation_SQLDTO>()), Times.Exactly(3));
            context.Verify(c => c.Allocations.Add(It.IsAny<Allocation_SQLDTO>()), Times.Exactly(2));
            context.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void UpsertAllocationsBatch_AnAllocationInBdTwice1Inactive_ExceptionThrown()
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
                    Status = 2
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
                    Id= "r1s4t0",
                    RepositoryId = 1,
                    StudentId = 4,
                    TeacherId = null,
                    Status = 0
                }
            };

            List<Allocation> allocations = new List<Allocation>
            {
                new Allocation
                {
                    Id= "r1s2t0",
                    RepositoryId = 1,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 3
                },
                new Allocation
                {
                    Id= "r1s4t0",
                    RepositoryId = 1,
                    StudentId = 4,
                    TeacherId = null,
                    Status = 3
                },
                new Allocation
                {
                    Id= "r1s0t1",
                    RepositoryId = 1,
                    StudentId = null,
                    TeacherId = 1,
                    Status = 3
                },
                new Allocation
                {
                    Id= "r2s2t0",
                    RepositoryId = 2,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 3
                },
                new Allocation
                {
                    Id= "r3s2t0",
                    RepositoryId = 2,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 2
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            // Act Assert
            Assert.Throws<InvalidOperationException>(() => da.UpsertAllocationsBatch(allocations));
            //context.Verify(c => c.Update(It.IsAny<Allocation_SQLDTO>()), Times.Once);
            context.Verify(c => c.SaveChanges(), Times.Never);
            
            
        }
        #endregion

        #region DeleteAllocationsBatch
        [Fact]
        public void DeleteAllocationsBatch_ParamNull_ExceptionThrown()
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

            List<Allocation> allocations = null;

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            // Act Assert
            Assert.Throws<ArgumentNullException>(() => { da.DeleteAllocationsBatch(allocations); });
            
            
        }

        [Fact]
        public void DeleteAllocationsBatch_ParamNullInList_ExceptionThrown()
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
                    Status = 1
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
                    Status = 1
                },
                new Allocation_SQLDTO
                {
                    Id= "r2s2t0",
                    RepositoryId = 2,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 1
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

            List<Allocation> allocations = new List<Allocation>
            {
                new Allocation
                {
                    Id= "r1s2t0",
                    RepositoryId = 1,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 3
                },
                new Allocation
                {
                    Id= "r1s4t0",
                    RepositoryId = 1,
                    StudentId = 4,
                    TeacherId = null,
                    Status = 3
                },
                null,
                new Allocation
                {
                    Id= "r2s2t0",
                    RepositoryId = 2,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 3
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            // Act Assert
            Assert.Throws<ArgumentNullException>(() => { da.DeleteAllocationsBatch(allocations); });
            //context.Verify(c => c.Update(It.IsAny<Allocation_SQLDTO>()), Times.Exactly(2));
        }

        [Fact]
        public void DeleteAllocationsBatch_5AllocationList3Exists2Not_Modify3Allocation()
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
                    Status = 2
                },
                new Allocation_SQLDTO
                {
                    Id= "r1s0t1",
                    RepositoryId = 1,
                    StudentId = null,
                    TeacherId = 1,
                    Status = 3
                }
            };

            List<Allocation> allocations = new List<Allocation>
            {
                new Allocation
                {
                    Id= "r1s2t0",
                    RepositoryId = 1,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 3
                },
                new Allocation
                {
                    Id= "r1s4t0",
                    RepositoryId = 1,
                    StudentId = 4,
                    TeacherId = null,
                    Status = 3
                },
                new Allocation
                {
                    Id= "r1s0t1",
                    RepositoryId = 1,
                    StudentId = null,
                    TeacherId = 1,
                    Status = 3
                },
                new Allocation
                {
                    Id= "r2s2t0",
                    RepositoryId = 2,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 3
                },
                new Allocation
                {
                    Id= "r3s2t0",
                    RepositoryId = 2,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 2
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            // Act
            da.DeleteAllocationsBatch(allocations);

            // Assert
            //context.Verify(c => c.Update(It.IsAny<Allocation_SQLDTO>()), Times.Exactly(3));
            context.Verify(c => c.SaveChanges(), Times.Once);
        }

        [Fact]
        public void DeleteAllocationsBatch_AnAllocationInBdTwice1Inactive_ExceptionThrown()
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
                    Status = 2
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
                    Id= "r1s4t0",
                    RepositoryId = 1,
                    StudentId = 4,
                    TeacherId = null,
                    Status = 0
                }
            };

            List<Allocation> allocations = new List<Allocation>
            {
                new Allocation
                {
                    Id= "r1s2t0",
                    RepositoryId = 1,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 3
                },
                new Allocation
                {
                    Id= "r1s4t0",
                    RepositoryId = 1,
                    StudentId = 4,
                    TeacherId = null,
                    Status = 3
                },
                new Allocation
                {
                    Id= "r1s0t1",
                    RepositoryId = 1,
                    StudentId = null,
                    TeacherId = 1,
                    Status = 3
                },
                new Allocation
                {
                    Id= "r2s2t0",
                    RepositoryId = 2,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 3
                },
                new Allocation
                {
                    Id= "r3s2t0",
                    RepositoryId = 2,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 2
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            // Act Assert
            Assert.Throws<InvalidOperationException>(() => da.DeleteAllocationsBatch(allocations));
            //context.Verify(c => c.Update(It.IsAny<Allocation_SQLDTO>()), Times.Once);
            context.Verify(c => c.SaveChanges(), Times.Never);
            
            
        }
        #endregion

        #region SetAllocationAfterCreation
        [Fact]
        public void SetAllocationAfterCreation_ParamNull_ExceptionThrown()
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

            Allocation allocation = null;

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            // Act Assert
            Assert.Throws<ArgumentNullException>(() => { da.SetAllocationAfterCreation(allocation); });
        }
        #endregion

        #region GetSelectedAllocationsByAllocationID
        [Fact]
        public void GetSelectedAllocationsByAllocationID_ParamNull_ExceptionThrown()
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

            List<Allocation> allocations = null;

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            // Act Assert
            Assert.Throws<ArgumentNullException>(() => { da.GetSelectedAllocationsByAllocationID(allocations); });


        }

        [Fact]
        public void GetSelectedAllocationsByAllocationID_ParamNullInList_ExceptionThrown()
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
                    Status = 1
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
                    Status = 1
                },
                new Allocation_SQLDTO
                {
                    Id= "r2s2t0",
                    RepositoryId = 2,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 1
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

            List<Allocation> allocations = new List<Allocation>
            {
                new Allocation
                {
                    Id= "r1s2t0",
                    RepositoryId = 1,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 3
                },
                new Allocation
                {
                    Id= "r1s4t0",
                    RepositoryId = 1,
                    StudentId = 4,
                    TeacherId = null,
                    Status = 3
                },
                null,
                new Allocation
                {
                    Id= "r2s2t0",
                    RepositoryId = 2,
                    StudentId = 2,
                    TeacherId = null,
                    Status = 3
                }
            };

            var logMock = new Mock<IManipulationLogs>();
            Logging.Instance.ManipulationLog = logMock.Object;

            Mock<RPLPDbContext> context = new Mock<RPLPDbContext>();
            context.Setup(x => x.Allocations).ReturnsDbSet(allocationDTO);
            DepotAllocation da = new DepotAllocation(context.Object);

            // Act Assert
            Assert.Throws<ArgumentNullException>(() => { da.GetSelectedAllocationsByAllocationID(allocations); });
        }
        #endregion
    }
}
