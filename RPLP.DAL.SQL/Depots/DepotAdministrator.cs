using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
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
using static RPLP.DAL.SQL.Depots.VerificatorForDepot;

namespace RPLP.DAL.SQL.Depots
{
    public class DepotAdministrator : IDepotAdministrator
    {
        private readonly RPLPDbContext _context;

        public DepotAdministrator()
        {
            this._context = new RPLPDbContext();
        }

        public DepotAdministrator(RPLPDbContext p_context)
        {
            if (p_context == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                               "DepotAdministrator -  DepotAdministrator(RPLPDbContext p_context) - p_context de type RPLPDbContext passé en paramètre est null", 0));
            }

            this._context = p_context;
        }

        public List<Administrator> GetAdministrators()
        {
            List<Administrator_SQLDTO> adminResult = this._context.Administrators.Where(admin => admin.Active)
                                                                                 .Include(admin => admin.Organisations.Where(organisation => organisation.Active)).ToList();

            if(adminResult == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                               "DepotAdministrator -  GetAdministrators() - la liste adminResult de type Administrator_SQLDTO assignée à partir de this._context.Administrators.Where(admin => admin.Active).Include(admin => admin.Organisations.Where(organisation => organisation.Active)).ToList(); est null", 0));
            }

            List<Administrator> administrators = adminResult.Select(admin => admin.ToEntityWithoutList()).ToList();

            if (administrators == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                               "DepotAdministrator -  GetAdministrators() - la liste administrators de type Administrator assignée à partir de  adminResult.Select(admin => admin.ToEntityWithoutList()).ToList(); est null", 0));
            }

            for (int i = 0; i < adminResult.Count; i++)
            {
                if (adminResult[i].Id == administrators[i].Id && adminResult[i].Organisations.Count >= 1)
                    administrators[i].Organisations = adminResult[i].Organisations.Select(organisation => organisation.ToEntityWithoutList()).ToList();
            }

            RPLP.JOURNALISATION.Journalisation.Journaliser(new Log("Administrators - Organisations", $"DepotAdministrator - Method - GetAdministrators() - Return List<Administrator>"));
    
            return administrators;
        }

        public List<Administrator> GetDeactivatedAdministrators()
        {
            List<Administrator_SQLDTO> adminResult = this._context.Administrators.Where(admin => !admin.Active)
                                                                                 .Include(admin => admin.Organisations.Where(organisation => organisation.Active)).ToList();
            
            
            List<Administrator> administrators = adminResult.Select(admin => admin.ToEntityWithoutList()).ToList();

            for (int i = 0; i < adminResult.Count; i++)
            {
                if (adminResult[i].Id == administrators[i].Id && adminResult[i].Organisations.Count >= 1)
                    administrators[i].Organisations = adminResult[i].Organisations.Select(organisation => organisation.ToEntityWithoutList()).ToList();
            }

            RPLP.JOURNALISATION.Journalisation.Journaliser(new Log("Administrators - Organisations", $"DepotAdministrator - Method - GetDeactivatedAdministrators() - Return List<Administrator>"));

            return administrators;
        }

        public Administrator GetAdministratorById(int p_id)
        {
            if (p_id < 0)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "DepotAdministrator - GetAdministratorById - p_id passé en paramêtre est hors des limites", 0));
            }

            Administrator_SQLDTO adminResult = this._context.Administrators.Where(admin => admin.Active)
                                                                           .Include(admin => admin.Organisations.Where(organisation => organisation.Active))
                                                                           .FirstOrDefault(admin => admin.Id == p_id);
            if (adminResult == null)
                return null;

            Administrator administrator = adminResult.ToEntityWithoutList();

            if (adminResult.Organisations.Count >= 1)
            {
                List<Organisation> organisations = adminResult.Organisations.Select(organisation => organisation.ToEntityWithoutList()).ToList();
                administrator.Organisations = organisations;
            }

            RPLP.JOURNALISATION.Journalisation.Journaliser(new Log("Administrators - Organisations", $"DepotAdministrator - Method - GetAdministratorById(int p_id) - Return Administrator"));

            return administrator;
        }

        public Administrator GetAdministratorByEmail(string p_email)
        {
            if (string.IsNullOrWhiteSpace(p_email))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotAdministrator - GetAdministratorByEmail - p_email passé en paramètre est vide", 0));
            }

            Administrator_SQLDTO? adminResult = this._context.Administrators
                .Include(admin => admin.Organisations.Where(organisation => organisation.Active))
                .FirstOrDefault(admin => admin.Email == p_email && admin.Active);

            if (adminResult == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotAdministrator - GetAdministratorByEmail - la variable adminResult est null", 0));

                return null;
            }

            Administrator administrator = adminResult.ToEntityWithoutList();

            if (adminResult.Organisations.Count >= 1)
            {
                List<Organisation> organisations = adminResult.Organisations.Select(organisation => organisation.ToEntityWithoutList()).ToList();
                administrator.Organisations = organisations;
            }

            RPLP.JOURNALISATION.Journalisation.Journaliser(new Log("Administrators - Organisations", $"DepotAdministrator - Method - GetAdministratorByEmail(string p_email) - Return Administrator"));

            return administrator;
        }


        public Administrator GetAdministratorByUsername(string p_adminUsername)
        {
            if (string.IsNullOrWhiteSpace(p_adminUsername))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotAdministrator - GetAdministratorByUsername - p_adminUsername passé en paramètre est vide", 0));
            }

            Administrator_SQLDTO adminResult = this._context.Administrators.Where(admin => admin.Active)
                                                                           .Include(admin => admin.Organisations.Where(organisation => organisation.Active))
                                                                           .FirstOrDefault(admin => admin.Username == p_adminUsername);
            if (adminResult == null)
                return null;

            Administrator administrator = adminResult.ToEntityWithoutList();

            if (adminResult.Organisations.Count >= 1)
            {
                List<Organisation> organisations = adminResult.Organisations.Select(organisation => organisation.ToEntityWithoutList()).ToList();
                administrator.Organisations = organisations;
            }

            RPLP.JOURNALISATION.Journalisation.Journaliser(new Log("Administrators - Organisations", $"DepotAdministrator - Method - GetAdministratorByUsername(string p_adminUsername) - Return Administrator"));

            return administrator;
        }

        public List<Organisation> GetAdminOrganisations(string p_adminUsername)
        {
            if (string.IsNullOrWhiteSpace(p_adminUsername))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotAdministrator - GetAdminOrganisations - p_adminUsername passé en paramètre est vide", 0));
            }

            List<Organisation> organisations = new List<Organisation>();
            Administrator_SQLDTO admin = this._context.Administrators.Where(admin => admin.Active)
                                                                     .Include(admin => admin.Organisations.Where(organisation => organisation.Active))
                                                                     .FirstOrDefault(admin => admin.Username == p_adminUsername);
            if (admin != null)
            {
                if (admin.Organisations.Count >= 1)
                {
                    foreach (Organisation_SQLDTO organisation in admin.Organisations)
                    {
                        organisations.Add(organisation.ToEntityWithoutList());
                    }

                    return organisations;
                }
            }

            RPLP.JOURNALISATION.Journalisation.Journaliser(new Log("Administrators - Organisations", $"DepotAdministrator - Method - GetAdminOrganisations(string p_adminUsername) - Return List<Organisation>"));

            return new List<Organisation>();
        }

        public void JoinOrganisation(string p_adminUsername, string p_organisationName)
        {
            if (string.IsNullOrWhiteSpace(p_adminUsername))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotAdministrator - JoinOrganisation - p_adminUsername passé en paramètre est vide", 0));
            }

            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotAdministrator - JoinOrganisation - p_organisationName passé en paramètre est vide", 0));
            }

            Administrator_SQLDTO adminResult = this._context.Administrators.Include(admin => admin.Organisations.Where(organisation => organisation.Active))
                                                                           .FirstOrDefault(admin => admin.Username == p_adminUsername && admin.Active);
            if (adminResult != null)
            {
                Organisation_SQLDTO organisationResult = this._context.Organisations.SingleOrDefault(organisation => organisation.Name == p_organisationName && organisation.Active);

                if (organisationResult != null && !adminResult.Organisations.Contains(organisationResult))
                {
                    adminResult.Organisations.Add(organisationResult);

                    this._context.Update(adminResult);
                    this._context.SaveChanges();
                }
            }

            RPLP.JOURNALISATION.Journalisation.Journaliser(new Log("Administrators - Organisations", $"DepotAdministrator - Method - JoinOrganisation(string p_adminUsername, string p_organisationName) - Void"));
        }

        public void LeaveOrganisation(string p_adminUsername, string p_organisationName)
        {
            if (string.IsNullOrWhiteSpace(p_adminUsername))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotAdministrator - LeaveOrganisation - p_adminUsername passé en paramètre est vide", 0));
            }

            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotAdministrator - LeaveOrganisation - p_organisationName passé en paramètre est vide", 0));
            }

            Administrator_SQLDTO adminResult = this._context.Administrators.Where(admin => admin.Active)
                                                                           .Include(admin => admin.Organisations.Where(organisation => organisation.Active))
                                                                           .FirstOrDefault(admin => admin.Username == p_adminUsername);
            if (adminResult != null)
            {
                Organisation_SQLDTO organisationResult = this._context.Organisations.Where(organisation => organisation.Active)
                                                                                    .SingleOrDefault(organisation => organisation.Name == p_organisationName);

                if (organisationResult != null && adminResult.Organisations.Contains(organisationResult))
                {
                    adminResult.Organisations.Remove(organisationResult);

                    this._context.Update(adminResult);
                    this._context.SaveChanges();
                }
            }

            RPLP.JOURNALISATION.Journalisation.Journaliser(new Log("Administrators - Organisations", $"DepotAdministrator - Method - LeaveOrganisation(string p_adminUsername, string p_organisationName) - Void"));
        }

        public void UpsertAdministrator(Administrator p_administrator)
        {
            if (p_administrator == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotAdministrator - UpsertAdministrator - p_administrator passé en paramètre est null", 0));
            }

            VerificatorForDepot verificator = new VerificatorForDepot(this._context);
            List<Organisation_SQLDTO> organisations = new List<Organisation_SQLDTO>();

            if (p_administrator.Organisations == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotAdministrator - UpsertAdministrator - p_administrator.Organisations passé en paramètre est null", 0));
            }

            foreach (Organisation organisation in p_administrator.Organisations)
            {
                organisations.Add(new Organisation_SQLDTO(organisation));
            }

            Administrator_SQLDTO? adminResult = this._context.Administrators
                .AsNoTracking()
                .FirstOrDefault(admin => admin.Id == p_administrator.Id);

            if (CheckIfUsernameTakenForUpsert(adminResult, p_administrator))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotAdministrator - UpsertAdministrator - CheckIfUsernameTakenForUpsert(adminResult, p_administrator) nous retourne : true", 0));

                throw new ArgumentException("Username already taken.");
            }

            if (CheckIfEmailTakenForUpsert(adminResult, p_administrator))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotAdministrator - UpsertAdministrator - CheckIfEmailTakenForUpsert(adminResult, p_administrator) nous retourne : true", 0));

                throw new ArgumentException("Email already in use.");
            }

            if (adminResult != null)
            {
                if (!adminResult.Active)
                {
                    throw new ArgumentException("Deleted accounts cannot be updated.");
                }

                this.UpdateAdmin(adminResult, p_administrator);
            }
            else
            {
                InsertAdminDTO(p_administrator);
            }
        }

        public void DeleteAdministrator(string p_adminUsername)
        {
            if (string.IsNullOrWhiteSpace(p_adminUsername))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotAdministrator - DeleteAdministrator - p_adminUsername passé en paramètre est vide", 0));
            }

            Administrator_SQLDTO adminResult = this._context.Administrators.Where(admin => admin.Active)
                                                                           .FirstOrDefault(admin => admin.Username == p_adminUsername);
            if (adminResult != null)
            {
                adminResult.Active = false;

                this._context.Update(adminResult);
                this._context.SaveChanges();
            }

            RPLP.JOURNALISATION.Journalisation.Journaliser(new Log("Administrators", $"DepotAdministrator - Method - DeleteAdministrator(string p_adminUsername) - Void"));
        }

        public void ReactivateAdministrator(string p_adminUsername)
        {
            if (string.IsNullOrWhiteSpace(p_adminUsername))
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotAdministrator - ReactivateAdministrator - p_adminUsername passé en paramètre est vide", 0));
            }

            Administrator_SQLDTO adminResult = this._context.Administrators.Where(admin => !admin.Active)
                                                                           .FirstOrDefault(admin => admin.Username == p_adminUsername);
            if (adminResult != null)
            {
                adminResult.Active = true;

                this._context.Update(adminResult);
                this._context.SaveChanges();
            }

            RPLP.JOURNALISATION.Journalisation.Journaliser(new Log("Administrators", $"DepotAdministrator - Method - ReactivateAdministrator(string p_adminUsername) - Void"));
        }

        private bool CheckIfUsernameTakenForUpsert(Administrator_SQLDTO p_adminResult, Administrator p_admin)
        {
            if (p_adminResult == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotAdministrator - CheckIfUsernameTakenForUpsert - p_adminResult passé en paramètre est null", 0));
            }

            if (p_admin == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotAdministrator - CheckIfUsernameTakenForUpsert - p_admin passé en paramètre est null", 0));
            }

            VerificatorForDepot verificator = new VerificatorForDepot(this._context);

            return ((p_adminResult != null && p_adminResult.Username != p_admin.Username &&
                verificator.CheckUsernameTaken(p_admin.Username)) ||
                p_adminResult == null && verificator.CheckUsernameTaken(p_admin.Username));
        }

        private bool CheckIfEmailTakenForUpsert(Administrator_SQLDTO p_adminResult, Administrator p_admin)
        {
            if (p_adminResult == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotAdministrator - CheckIfEmailTakenForUpsert - p_adminResult passé en paramètre est null", 0));
            }

            if (p_admin == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotAdministrator - CheckIfEmailTakenForUpsert - p_admin passé en paramètre est null", 0));
            }

            VerificatorForDepot verificator = new VerificatorForDepot(this._context);

            return (p_adminResult != null && p_adminResult.Email != p_admin.Email && verificator.CheckEmailTaken(p_admin.Email)) ||
                p_adminResult == null && verificator.CheckEmailTaken(p_admin.Email);
        }

        private void UpdateAdmin(Administrator_SQLDTO adminToUpdate, Administrator updatedAdmin)
        {
            if (adminToUpdate == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotAdministrator - CheckIfEmailTakenForUpsert - adminToUpdate passé en paramètre est null", 0));
            }

            if (updatedAdmin == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotAdministrator - CheckIfEmailTakenForUpsert - updatedAdmin passé en paramètre est null", 0));
            }

            adminToUpdate.Id = updatedAdmin.Id;
            adminToUpdate.Username = updatedAdmin.Username;
            adminToUpdate.Token = updatedAdmin.Token;
            adminToUpdate.FirstName = updatedAdmin.FirstName;
            adminToUpdate.LastName = updatedAdmin.LastName;
            adminToUpdate.Email = updatedAdmin.Email;

            this._context.Update(adminToUpdate);
            this._context.SaveChanges();

            RPLP.JOURNALISATION.Journalisation.Journaliser(new Log("Administrators", $"DepotAdministrator - Method - UpdateAdmin(Administrator_SQLDTO adminToUpdate, Administrator updatedAdmin) - Void"));
        }

        private void InsertAdminDTO(Administrator adminInfos)
        {
            if (adminInfos == null)
            {
                RPLP.JOURNALISATION.Journalisation.Journaliser(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotAdministrator - InsertAdminDTO - adminInfos passé en paramètre est null", 0));
            }

            Administrator_SQLDTO adminDTO = new Administrator_SQLDTO();
            adminDTO.Username = adminInfos.Username;
            adminDTO.Token = adminInfos.Token;
            adminDTO.FirstName = adminInfos.FirstName;
            adminDTO.LastName = adminInfos.LastName;
            adminDTO.Email = adminInfos.Email;
            adminDTO.Active = true;

            this._context.Administrators.Add(adminDTO);
            this._context.SaveChanges();

            RPLP.JOURNALISATION.Journalisation.Journaliser(new Log("Administrators", $"DepotAdministrator - Method - InsertAdminDTO(Administrator adminInfos) - Void"));
        }
    }
}
