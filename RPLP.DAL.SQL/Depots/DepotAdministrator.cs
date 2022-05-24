using Microsoft.EntityFrameworkCore;
using RPLP.DAL.DTO.Sql;
using RPLP.ENTITES;
using RPLP.SERVICES.InterfacesDepots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            this._context = p_context;
        }

        public List<Administrator> GetAdministrators()
        {
            List<Administrator_SQLDTO> adminResult = this._context.Administrators.Where(admin => admin.Active)
                                                                                 .Include(admin => admin.Organisations.Where(organisation => organisation.Active)).ToList();
            List<Administrator> administrators = adminResult.Select(admin => admin.ToEntityWithoutList()).ToList();

            for (int i = 0; i < adminResult.Count; i++)
            {
                if (adminResult[i].Id == administrators[i].Id && adminResult[i].Organisations.Count >= 1)
                    administrators[i].Organisations = adminResult[i].Organisations.Select(organisation => organisation.ToEntityWithoutList()).ToList();
            }

            return administrators;
        }

        public Administrator GetAdministratorById(int p_id)
        {
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

            return administrator;
        }

        public Administrator GetAdministratorByEmail(string p_email)
        {
            Administrator_SQLDTO? adminResult = this._context.Administrators
                .Include(admin => admin.Organisations.Where(organisation => organisation.Active))
                .FirstOrDefault(admin => admin.Email == p_email && admin.Active);
            if (adminResult == null)
                return null;

            Administrator administrator = adminResult.ToEntityWithoutList();

            if (adminResult.Organisations.Count >= 1)
            {
                List<Organisation> organisations = adminResult.Organisations.Select(organisation => organisation.ToEntityWithoutList()).ToList();
                administrator.Organisations = organisations;
            }

            return administrator;
        }


        public Administrator GetAdministratorByUsername(string p_adminUsername)
        {
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

            return administrator;
        }

        public List<Organisation> GetAdminOrganisations(string p_adminUsername)
        {
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

            return new List<Organisation>();
        }

        public void JoinOrganisation(string p_adminUsername, string p_organisationName)
        {
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
        }

        public void LeaveOrganisation(string p_adminUsername, string p_organisationName)
        {
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
        }

        public void UpsertAdministrator(Administrator p_administrator)
        {
            VerificatorForDepot verificator = new VerificatorForDepot(this._context);
            List<Organisation_SQLDTO> organisations = new List<Organisation_SQLDTO>();

            if (p_administrator.Organisations.Count >= 1)
            {
                foreach (Organisation organisation in p_administrator.Organisations)
                {
                    organisations.Add(new Organisation_SQLDTO(organisation));
                }
            }

            Administrator_SQLDTO? adminResult = this._context.Administrators
                .AsNoTracking()
                .FirstOrDefault(admin => admin.Id == p_administrator.Id);

            if ((adminResult != null && adminResult.Username != p_administrator.Username &&
                verificator.CheckUsernameTaken(p_administrator.Username)) ||
                adminResult == null && verificator.CheckUsernameTaken(p_administrator.Username))
            {
                throw new ArgumentException("Username already taken.");
            }

            if ((adminResult != null && adminResult.Email != p_administrator.Email && verificator.CheckEmailTaken(p_administrator.Email)) ||
                adminResult == null && verificator.CheckEmailTaken(p_administrator.Email))
            {
                throw new ArgumentException("Email already in use.");
            }

            if (adminResult != null)
            {
                if (!adminResult.Active)
                {
                    throw new ArgumentException("Deleted accounts cannot be updated.");
                }

                adminResult.Id = p_administrator.Id;
                adminResult.Username = p_administrator.Username;
                adminResult.Token = p_administrator.Token;
                adminResult.FirstName = p_administrator.FirstName;
                adminResult.LastName = p_administrator.LastName;
                adminResult.Email = p_administrator.Email;

                this._context.Update(adminResult);
                this._context.SaveChanges();
            }
            else
            {

                Administrator_SQLDTO adminDTO = new Administrator_SQLDTO();
                adminDTO.Username = p_administrator.Username;
                adminDTO.Token = p_administrator.Token;
                adminDTO.FirstName = p_administrator.FirstName;
                adminDTO.LastName = p_administrator.LastName;
                adminDTO.Email = p_administrator.Email;
                adminDTO.Active = true;

                this._context.Administrators.Add(adminDTO);
                this._context.SaveChanges();
            }
        }

        public void DeleteAdministrator(string p_adminUsername)
        {
            Administrator_SQLDTO adminResult = this._context.Administrators.Where(admin => admin.Active)
                                                                           .FirstOrDefault(admin => admin.Username == p_adminUsername);
            if (adminResult != null)
            {
                adminResult.Active = false;

                this._context.Update(adminResult);
                this._context.SaveChanges();
            }
        }
    }
}
