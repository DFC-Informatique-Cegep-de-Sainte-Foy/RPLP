using Microsoft.EntityFrameworkCore;
using RPLP.DAL.DTO.Sql;
using RPLP.ENTITES;
using RPLP.JOURNALISATION;
using RPLP.SERVICES.InterfacesDepots;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RPLP.DAL.SQL.Depots
{
    public class DepotOrganisation : IDepotOrganisation
    {
        private readonly RPLPDbContext _context;

        public DepotOrganisation()
        {
            this._context = new RPLPDbContext(new DbContextOptionsBuilder<RPLPDbContext>().UseSqlServer("Server=rplp.db; Database=RPLP; User Id=sa; password=Cad3pend86!").Options);
            //this._context = new RPLPDbContext(new DbContextOptionsBuilder<RPLPDbContext>().UseSqlServer("Server=localhost,1433; Database=RPLP; User Id=sa; password=Cad3pend86!").Options);
        }

        public DepotOrganisation(RPLPDbContext p_context)
        {
            if (p_context == null)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                               "DepotOrganisation - DepotOrganisation(RPLPDbContext p_context) - p_context de type RPLPDbContext passé en paramètre est null", 0));
            }

            this._context = p_context;
        }

        public List<Organisation> GetOrganisations()
        {
            List<Organisation_SQLDTO> organisationResult = this._context.Organisations.Where(organisation => organisation.Active)
                                                                                      .Include(organisation => organisation.Administrators.Where(admin => admin.Active)).ToList();

            List<Organisation> organisations = organisationResult.Select(admin => admin.ToEntityWithoutList()).ToList();

            for (int i = 0; i < organisationResult.Count; i++)
            {
                if (organisationResult[i].Id == organisations[i].Id && organisationResult[i].Administrators.Count >= 1)
                    organisations[i].Administrators = organisationResult[i].Administrators.Select(organisation => organisation.ToEntityWithoutList()).ToList();
            }

            RPLP.JOURNALISATION.Logging.Journal(new Log("Organisation", $"DepotOrganisation - Method - GetOrganisations() - Return List<Organisation>"));

            return organisations;
        }

        public Organisation GetOrganisationById(int p_id)
        {
            if (p_id < 0)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "DepotOrganisation - GetOrganisationById - p_id passé en paramêtre est hors des limites", 0));
            }

            Organisation_SQLDTO organisationResult = this._context.Organisations.Where(organisation => organisation.Active)
                                                                           .Include(organisation => organisation.Administrators.Where(admin => admin.Active))
                                                                           .FirstOrDefault(organisation => organisation.Id == p_id);

            if (organisationResult == null)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "DepotOrganisation - GetOrganisationById - organisationResult est null", 0));

                return new Organisation();
            }

            Organisation organisation = organisationResult.ToEntityWithoutList();

            if (organisationResult.Administrators.Count >= 1)
            {
                List<Administrator> administrators = organisationResult.Administrators.Select(admin => admin.ToEntityWithoutList()).ToList();
                organisation.Administrators = administrators;

                RPLP.JOURNALISATION.Logging.Journal(new Log("Organisation", $"DepotOrganisation - Method - GetOrganisationById(int p_id) - Return Organisation"));
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "DepotOrganisation - GetOrganisationById - organisationResult.Administrators.Count >= 1", 0));
            }

            return organisation;
        }

        public Organisation GetOrganisationByName(string p_name)
        {
            if (string.IsNullOrWhiteSpace(p_name))
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotOrganisation - GetOrganisationByName - p_name passé en paramètre est vide", 0));
            }

            Organisation_SQLDTO organisationResult = this._context.Organisations.Where(organisation => organisation.Active)
                                                                                .Include(organisation => organisation.Administrators.Where(admin => admin.Active))
                                                                                .FirstOrDefault(organisation => organisation.Name == p_name);

            if (organisationResult == null)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                      "DepotOrganisation - GetOrganisationByName(string p_name) - organisationResult est null", 0));

                return new Organisation();
            }
               

            Organisation organisation = organisationResult.ToEntityWithoutList();

            if (organisationResult.Administrators.Count >= 1)
            {
                List<Administrator> administrators = organisationResult.Administrators.Select(administrator => administrator.ToEntityWithoutList()).ToList();
                organisation.Administrators = administrators;

                RPLP.JOURNALISATION.Logging.Journal(new Log("Organisation - Administrator", $"DepotOrganisation - Method -  GetOrganisationByName(string p_name) - Return Organisation"));
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "DepotOrganisation - GetOrganisationByName(string p_name) - organisationResult.Administrators.Count >= 1", 0));
            }

            return organisation;
        }

        public List<Administrator> GetAdministratorsByOrganisation(string p_organisationName)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotOrganisation - GetAdministratorsByOrganisation - p_organisationName passé en paramètre est vide", 0));
            }

            Organisation_SQLDTO organisationResult = this._context.Organisations
                .Include(organisation => organisation.Administrators.Where(a => a.Active))
                .FirstOrDefault(organisation => organisation.Name == p_organisationName && organisation.Active);

            

            if (organisationResult != null && organisationResult.Administrators.Count >= 1)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log("Organisation - Administrator", $"DepotOrganisation - Method - GetAdministratorsByOrganisation(string p_organisationName) - Return List<Administrator>"));

                return organisationResult.Administrators.Select(administrator => administrator.ToEntityWithoutList()).ToList();
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log("Organisation - Administrator", $"DepotOrganisation - Method - GetAdministratorsByOrganisation(string p_organisationName) - Return List<Administrator> - liste vide ou organisationResult null"));
            }

            return new List<Administrator>();
        }

        public void AddAdministratorToOrganisation(string p_organisationName, string p_adminUsername)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotOrganisation - AddAdministratorToOrganisation - p_organisationName passé en paramètre est vide", 0));
            }
            if (string.IsNullOrWhiteSpace(p_adminUsername))
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotOrganisation - AddAdministratorToOrganisation - p_adminUsername passé en paramètre est vide", 0));
            }

            Administrator_SQLDTO adminResult = this._context.Administrators.Where(admin => admin.Active)
                                                                           .FirstOrDefault(admin => admin.Username == p_adminUsername);
            if (adminResult != null)
            {
                Organisation_SQLDTO organisationResult = this._context.Organisations.Where(organisation => organisation.Name == p_organisationName).SingleOrDefault();

                if (organisationResult != null)
                {
                    organisationResult.Administrators.Add(adminResult);

                    this._context.Update(organisationResult);
                    this._context.SaveChanges();
                    RPLP.JOURNALISATION.Logging.Journal(new Log("Organisation - Administrator", $"DepotOrganisation - Method - AddAdministratorToOrganisation(string p_organisationName, string p_adminUsername) - Void"));
                }
                else
                {
                    RPLP.JOURNALISATION.Logging.Journal(new Log("Organisation - Administrator", $"DepotOrganisation - Method - AddAdministratorToOrganisation(string p_organisationName, string p_adminUsername) - la variable organisationResult est null", 0));
                }
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log("Organisation - Administrator", $"DepotOrganisation - Method - AddAdministratorToOrganisation(string p_organisationName, string p_adminUsername) - la variable adminResult est null", 0));
            }
        }

        public void RemoveAdministratorFromOrganisation(string p_organisationName, string p_adminUsername)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotOrganisation - RemoveAdministratorFromOrganisation - p_organisationName passé en paramètre est vide", 0));
            }
            if (string.IsNullOrWhiteSpace(p_adminUsername))
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotOrganisation - RemoveAdministratorFromOrganisation - p_adminUsername passé en paramètre est vide", 0));
            }

            Administrator_SQLDTO adminResult = this._context.Administrators.Where(admin => admin.Active)
                                                                           .FirstOrDefault(admin => admin.Username == p_adminUsername);
            if (adminResult != null)
            {
                Organisation_SQLDTO organisationResult = this._context.Organisations.SingleOrDefault(organisation => organisation.Name == p_organisationName);

                if (organisationResult != null)
                {
                    organisationResult.Administrators.Remove(adminResult);

                    this._context.Update(organisationResult);
                    this._context.SaveChanges();

                    RPLP.JOURNALISATION.Logging.Journal(new Log("Organisation - Administrator", $"DepotOrganisation - Method - RemoveAdministratorFromOrganisation(string p_organisationName, string p_adminUsername) - Void - remove admin organisation"));
                }
                else
                {
                    RPLP.JOURNALISATION.Logging.Journal(new Log("Organisation - Administrator", $"DepotOrganisation - Method - RemoveAdministratorFromOrganisation(string p_organisationName, string p_adminUsername) - la variable organisationResult est null", 0));
                }
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log("Organisation - Administrator", $"DepotOrganisation - Method - RemoveAdministratorFromOrganisation(string p_organisationName, string p_adminUsername) - la variable adminResult est null", 0));
            }
        }

        public void UpsertOrganisation(Organisation p_organisation)
        {
            if(p_organisation == null)
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotOrganisation - UpsertOrganisation - p_organisation passé en paramètre est null", 0));
            }

            List<Administrator_SQLDTO> administrators = new List<Administrator_SQLDTO>();

            if (p_organisation.Administrators.Count >= 1)
            {
                foreach (Administrator administrator in p_organisation.Administrators)
                {
                    administrators.Add(new Administrator_SQLDTO(administrator));
                }
            }

            Organisation_SQLDTO organisationResult = this._context.Organisations.Where(organisation => organisation.Active)
                                                                                .FirstOrDefault(organisation => organisation.Id == p_organisation.Id);
            if (organisationResult != null)
            {
                organisationResult.Name = p_organisation.Name;
                organisationResult.Administrators = administrators;

                this._context.Update(organisationResult);
                this._context.SaveChanges();

                RPLP.JOURNALISATION.Logging.Journal(new Log("Organisation - Administrator", $"DepotOrganisation - Method - UpsertOrganisation(Organisation p_organisation) - Void - Update Organisation"));
            }
            else
            {
                Organisation_SQLDTO organisation = new Organisation_SQLDTO();
                organisation.Name = p_organisation.Name;
                organisation.Administrators = administrators;
                organisation.Active = true;

                this._context.Organisations.Add(organisation);
                this._context.SaveChanges();

                RPLP.JOURNALISATION.Logging.Journal(new Log("Organisation - Administrator", $"DepotOrganisation - Method - UpsertOrganisation(Organisation p_organisation) - Void - Add Organisation"));
            }
        }

        public void DeleteOrganisation(string p_organisationName)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotOrganisation - DeleteOrganisation - p_organisationName passé en paramètre est vide", 0));
            }

            Organisation_SQLDTO organisationResult = this._context.Organisations.Where(organisation => organisation.Active)
                                                                                .FirstOrDefault(organisation => organisation.Name == p_organisationName);
            if (organisationResult != null)
            {
                organisationResult.Active = false;

                this._context.Update(organisationResult);
                this._context.SaveChanges();

                RPLP.JOURNALISATION.Logging.Journal(new Log("Organisation", $"DepotOrganisation - Method - DeleteOrganisation(string p_organisationName) - Void - delete organisation"));
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Journal(new Log("Organisation", $"DepotOrganisation - Method - DeleteOrganisation(string p_organisationName) - la variable organisationResult est null", 0));
            }
            
        }
    }
}
