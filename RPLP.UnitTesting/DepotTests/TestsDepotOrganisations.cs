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
    [Collection("DatabaseTests")]
    public class TestsDepotOrganisations
    {
        private static readonly DbContextOptions<RPLPDbContext> options = new DbContextOptionsBuilder<RPLPDbContext>()
                .UseSqlServer("Server=localhost,1434; Database=RPLP; User Id=sa; password=Cad3pend86!")
                .Options;

        private void DeleteOrganisationsAndRelatedTablesContent()
        {
            using (var context = new RPLPDbContext(options))
            {
                context.Database.ExecuteSqlRaw("DELETE from Organisations;");
                context.Database.ExecuteSqlRaw("DELETE from Administrators;");
            }
        }

        private void InsertOrgnisations(Classroom_SQLDTO p_classroom)
        {
            using (var context = new RPLPDbContext(options))
            {
                context.Classrooms.Add(p_classroom);
                context.SaveChanges();
            }
        }

        private void InsertPremadeOrganisations()
        {
            List<Administrator_SQLDTO> administrators = new List<Administrator_SQLDTO>()
            {
                new Administrator_SQLDTO()
                {
                    Username = "ThPaquet",
                    FirstName = "Thierry",
                    LastName = "Paquet",
                    Token = "token",
                    Active = true
                },
                new Administrator_SQLDTO()
                {
                    Username = "ikeameatbol",
                    FirstName = "Jonathan",
                    LastName = "Blouin",
                    Token = "token",
                    Active = true
                },
                new Administrator_SQLDTO()
                {
                    Username = "BACenComm",
                    FirstName = "Melissa",
                    LastName = "Lachapelle",
                    Token = "token",
                    Active = false
                }
            };

            List<Organisation_SQLDTO> organisations = new List<Organisation_SQLDTO>()
            {
                new Organisation_SQLDTO()
                {
                    Name = "CEGEP Ste-Foy",
                    Administrators = administrators,
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Name = "College Edouard-Montpetit",
                    Administrators = administrators,
                    Active = true
                },
                new Organisation_SQLDTO()
                {
                    Name = "Universite Laval",
                    Administrators = administrators,
                    Active = false
                },
            };

            using (var context = new RPLPDbContext(options))
            {
                context.Organisations.AddRange(organisations);
                context.SaveChanges();
            }
        }

        [Fact]
        public void Test_GetOrganisations()
        {
            this.DeleteOrganisationsAndRelatedTablesContent();
            this.InsertPremadeOrganisations();

            using (var context = new RPLPDbContext(options))
            {
                DepotOrganisation depot = new DepotOrganisation(context);
                List<Organisation> organisations = depot.GetOrganisations();

                Assert.NotNull(organisations);
                Assert.Equal(2, organisations.Count);
                Assert.NotNull(organisations.FirstOrDefault(o => o.Name == "CEGEP Ste-Foy"));
                Assert.Null(organisations.FirstOrDefault(o => o.Name == "Universite Laval"));
                Assert.Equal(2, organisations.FirstOrDefault(o => o.Name == "CEGEP Ste-Foy").Administrators.Count);
            }

            this.DeleteOrganisationsAndRelatedTablesContent();
        }

        [Fact]
        public void Test_GetOrganisationById()
        {
            this.DeleteOrganisationsAndRelatedTablesContent();
            this.InsertPremadeOrganisations();
            
            using (var context = new RPLPDbContext(options))
            {
                int organisationId = context.Organisations.FirstOrDefault(o => o.Name == "CEGEP Ste-Foy").Id;

                DepotOrganisation depot = new DepotOrganisation(context);
                Organisation organisation = depot.GetOrganisationById(organisationId);

                Assert.NotNull(organisation);
                Assert.Equal(2, organisation.Administrators.Count);
            }

            this.DeleteOrganisationsAndRelatedTablesContent();
        }

        [Fact]
        public void Test_GetOrganisationByName()
        {
            this.DeleteOrganisationsAndRelatedTablesContent();
            this.InsertPremadeOrganisations();

            using (var context = new RPLPDbContext(options))
            {
                DepotOrganisation depot = new DepotOrganisation(context);
                Organisation organisation = depot.GetOrganisationByName("CEGEP Ste-Foy");

                Assert.NotNull(organisation);
                Assert.Equal(2, organisation.Administrators.Count);
            }

            this.DeleteOrganisationsAndRelatedTablesContent();
        }

        [Fact]
        public void Test_GetAdministratorsByOrganisation()
        {
            this.DeleteOrganisationsAndRelatedTablesContent();
            this.InsertPremadeOrganisations();

            using (var context = new RPLPDbContext(options))
            {
                DepotOrganisation depot = new DepotOrganisation(context);
                List<Administrator> administrators = depot.GetAdministratorsByOrganisation("CEGEP Ste-Foy");

                Assert.NotNull(administrators);
                Assert.Equal(2, administrators.Count);
            }

            this.DeleteOrganisationsAndRelatedTablesContent();
        }

        [Fact]
        public void Test_AddAdministratorToOrganisation()
        {
            this.DeleteOrganisationsAndRelatedTablesContent();
            this.InsertPremadeOrganisations();

            Administrator_SQLDTO newAdministrator = new Administrator_SQLDTO()
            {
                Username = "PiFou86",
                FirstName = "Pierre-Francois",
                LastName = "Leon",
                Token = "token",
                Active = true
            };

            using (var context = new RPLPDbContext(options))
            {
                context.Administrators.Add(newAdministrator);
                context.SaveChanges();
            }

            using (var context = new RPLPDbContext(options))
            {
                Administrator_SQLDTO? administratorInContext = context.Administrators.FirstOrDefault(a => a.Username == "PiFou86");
                Assert.NotNull(administratorInContext);

                Organisation_SQLDTO? organisation = context.Organisations
                    .Include(o => o.Administrators)
                    .FirstOrDefault(o => o.Name == "CEGEP Ste-Foy");

                Administrator_SQLDTO? administratorInOrganisation = organisation.Administrators.FirstOrDefault(a => a.Username == "PiFou86");

                Assert.Null(administratorInOrganisation);

                DepotOrganisation depot = new DepotOrganisation(context);
                depot.AddAdministratorToOrganisation(organisation.Name, administratorInContext.Username);
            }

            using (var context = new RPLPDbContext(options))
            {
                Organisation_SQLDTO? organisation = context.Organisations
                                                            .Include(o => o.Administrators)
                                                            .FirstOrDefault(o => o.Name == "CEGEP Ste-Foy");
                Administrator_SQLDTO? administrator = organisation.Administrators.FirstOrDefault(a => a.Username == "PiFou86");

                Assert.NotNull(organisation);
                Assert.NotNull(administrator);
            }

            this.DeleteOrganisationsAndRelatedTablesContent();
        }

        [Fact]
        public void Test_RemoveAdministratorFromOrganisation()
        {
            this.DeleteOrganisationsAndRelatedTablesContent();
            this.InsertPremadeOrganisations();

            string administratorUserName = "ThPaquet";

            using (var context = new RPLPDbContext(options))
            {
                Administrator_SQLDTO? administratorInContext = context.Administrators.FirstOrDefault(a => a.Username == administratorUserName);
                Assert.NotNull(administratorInContext);

                Organisation_SQLDTO? organisation = context.Organisations
                    .Include(o => o.Administrators)
                    .FirstOrDefault(o => o.Name == "CEGEP Ste-Foy");
                Administrator_SQLDTO? administratorInOrganisation = organisation.Administrators.FirstOrDefault(a => a.Username == administratorUserName);

                Assert.NotNull(administratorInOrganisation);

                DepotOrganisation depot = new DepotOrganisation(context);
                depot.RemoveAdministratorFromOrganisation(organisation.Name, administratorInContext.Username);
            }

            using (var context = new RPLPDbContext(options))
            {
                Organisation_SQLDTO? organisation = context.Organisations
                                                            .Include(o => o.Administrators)
                                                            .FirstOrDefault(o => o.Name == "CEGEP Ste-Foy");
                Administrator_SQLDTO? administrator = organisation.Administrators
                                                                  .FirstOrDefault(a => a.Username == administratorUserName);

                Assert.NotNull(organisation);
                Assert.Null(administrator);
            }

            this.DeleteOrganisationsAndRelatedTablesContent();
        }

        [Fact]
        public void Test_UpsertOrganisation_Inserts()
        {
            this.DeleteOrganisationsAndRelatedTablesContent();

            string organisationName = "RPLP";
            string administratorUserName = "ThPaquet";
            string administratorFirstName = "Thierry";
            string administratorLastName = "Paquet";
            string administratorToken = "token";

            using (var context = new RPLPDbContext(options))
            {
                Organisation_SQLDTO? organisation = context.Organisations.FirstOrDefault(o => o.Name == "RPLP");
                Assert.Null(organisation);

                DepotOrganisation depot = new DepotOrganisation(context);

                Organisation newOrganisation = new Organisation()
                {
                    Name = organisationName,
                    Administrators = new List<Administrator>()
                    {
                        new Administrator()
                        {
                            Username = administratorUserName,
                            FirstName = administratorFirstName,
                            LastName = administratorLastName,
                            Token = administratorToken
                        }
                    }
                };

                depot.UpsertOrganisation(newOrganisation);
            }

            using(var context = new RPLPDbContext(options))
            {
                Organisation_SQLDTO? organisation = context.Organisations
                                                            .Include(o => o.Administrators)
                                                            .FirstOrDefault(o => o.Name == "RPLP");
                Assert.NotNull(organisation);
                Assert.Equal(1, organisation.Administrators.Count);
                Assert.Equal(organisationName, organisation.Name);
                Assert.NotNull(organisation.Administrators.FirstOrDefault(a => a.Username == administratorUserName));
                Assert.NotNull(organisation.Administrators.FirstOrDefault(a => a.FirstName == administratorFirstName));
                Assert.NotNull(organisation.Administrators.FirstOrDefault(a => a.LastName == administratorLastName));
                Assert.NotNull(organisation.Administrators.FirstOrDefault(a => a.Token == administratorToken));
            }

            this.DeleteOrganisationsAndRelatedTablesContent();
        }

        [Fact]
        public void Test_UpsertOrganisation_Updates()
        {
            this.DeleteOrganisationsAndRelatedTablesContent();
            this.InsertPremadeOrganisations();

            string organisationName = "RPLP";
            string administratorUserName = "JackJackson";
            string administratorFirstName = "Jack";
            string administratorLastName = "Jackson";
            string administratorToken = "anotherToken";

            using (var context = new RPLPDbContext(options))
            {
                Organisation_SQLDTO? organisation = context.Organisations.FirstOrDefault(o => o.Name == "CEGEP Ste-Foy");
                Assert.NotNull(organisation);

                DepotOrganisation depot = new DepotOrganisation(context);

                Administrator_SQLDTO administrator = new Administrator_SQLDTO()
                {
                    Username = administratorUserName,
                    FirstName = administratorFirstName,
                    LastName = administratorLastName,
                    Token = administratorToken,
                    Active = true
                };

                organisation.Name = organisationName;
                organisation.Administrators.Add(administrator);


                depot.UpsertOrganisation(organisation.ToEntity());
            }

            using (var context = new RPLPDbContext(options))
            {
                Organisation_SQLDTO? organisation = context.Organisations
                                                            .Include(o => o.Administrators.Where(a => a.Active))
                                                            .FirstOrDefault(o => o.Name == "RPLP");
                Assert.NotNull(organisation);
                Assert.Equal(3, organisation.Administrators.Count);
                Assert.Equal(organisationName, organisation.Name);
                Assert.NotNull(organisation.Administrators.FirstOrDefault(a => a.Username == administratorUserName));
                Assert.NotNull(organisation.Administrators.FirstOrDefault(a => a.FirstName == administratorFirstName));
                Assert.NotNull(organisation.Administrators.FirstOrDefault(a => a.LastName == administratorLastName));
                Assert.NotNull(organisation.Administrators.FirstOrDefault(a => a.Token == administratorToken));
            }

            this.DeleteOrganisationsAndRelatedTablesContent();
        }

        [Fact]
        public void Test_DeleteOrganisation()
        {
            this.DeleteOrganisationsAndRelatedTablesContent();
            this.InsertPremadeOrganisations();

            using (var context = new RPLPDbContext(options))
            {
                Organisation_SQLDTO? organisation = context.Organisations.FirstOrDefault(o => o.Name == "CEGEP Ste-Foy");
                Assert.NotNull(organisation);

                DepotOrganisation depot = new DepotOrganisation(context);
                depot.DeleteOrganisation("CEGEP Ste-Foy");
            }

            using (var context = new RPLPDbContext(options))
            {
                Organisation_SQLDTO? organisation = context.Organisations.FirstOrDefault(o => o.Name == "CEGEP Ste-Foy" && o.Active == true);
                Assert.Null(organisation);
            }

            this.DeleteOrganisationsAndRelatedTablesContent();
        }
    }
}
