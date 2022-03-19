using Microsoft.EntityFrameworkCore;
using RPLP.DAL.DTO.Sql;
using RPLP.ENTITES;
using RPLP.SERVICES.InterfacesDepots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.DAL.SQL.Depots
{
    public class DepotAdministrator : IDepotAdminitrator
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

        public void DeleteAdministrator(string p_adminUsername)
        {
            Administrator_SQLDTO adminResult = this._context.Administrators.FirstOrDefault(admin => admin.Username == p_adminUsername);

            if (adminResult != null)
            {
                adminResult.Active = false;

                this._context.Update(adminResult);
                this._context.SaveChanges();
            }
        }

        public Administrator GetAdministratorById(int p_id)
        {
            Administrator administrator = this._context.Administrators.Where(admin => admin.Id == p_id)
                                                                      .Select(admin => admin.ToEntity())
                                                                      .FirstOrDefault();

            if (administrator == null)
                return new Administrator();

            return administrator;
        }

        public Administrator GetAdministratorByName(string p_adminUsername)
        {
            Administrator administrator = this._context.Administrators.Where(admin => admin.Username == p_adminUsername)
                                                                      .Include(admin => admin.Organisations)
                                                                      .Select(admin => admin.ToEntity())
                                                                      .FirstOrDefault();

            if (administrator == null)
                return new Administrator();

            return administrator;
        }

        public List<Administrator> GetAdministrators()
        {
            return this._context.Administrators.Where(admin => admin.Active)
                                               .Select(admin => admin.ToEntity()).ToList();
        }

        public List<Organisation> GetAdminOrganisations(string p_adminUsername)
        {
            List<Organisation> organisations = new List<Organisation>();
            Administrator_SQLDTO admin = this._context.Administrators.Include(admin => admin.Organisations)
                                                                     .FirstOrDefault(admin => admin.Username == p_adminUsername);
            if (admin != null)
            {
                if (admin.Organisations.Count >= 1)
                {
                    foreach (Organisation_SQLDTO organisation in admin.Organisations)
                    {
                        organisations.Add(organisation.ToEntityWithoutAdministrators());
                    }

                    return organisations;
                }
            }

            return new List<Organisation>();
        }

        public void JoinOrganisation(string p_adminUsername, string p_organisationName)
        {
            Administrator_SQLDTO adminResult = this._context.Administrators.Where(admin => admin.Username == p_adminUsername)
                                                                           .Include(admin => admin.Organisations)
                                                                           .FirstOrDefault();
            if (adminResult != null)
            {
                Organisation_SQLDTO organisationResult = this._context.Organisations.Where(organisation => organisation.Active)
                                                                                    .SingleOrDefault(organisation => organisation.Name == p_organisationName);
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
            Administrator_SQLDTO adminResult = this._context.Administrators.Where(admin => admin.Username == p_adminUsername)
                                                                           .Include(admin => admin.Organisations)
                                                                           .FirstOrDefault();
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
            List<Organisation_SQLDTO> organisations = new List<Organisation_SQLDTO>();

            if (p_administrator.Organisations.Count >= 1)
            {
                foreach (Organisation organisation in p_administrator.Organisations)
                {
                    organisations.Add(new Organisation_SQLDTO(organisation));
                }
            }

            Administrator_SQLDTO adminResult = this._context.Administrators.Where(admin => admin.Id == p_administrator.Id)
                                                                           .Include(admin => admin.Organisations)
                                                                           .FirstOrDefault();

            if (adminResult != null)
            {
                adminResult.Id = p_administrator.Id;
                adminResult.Username = p_administrator.Username;
                adminResult.Token = p_administrator.Token;
                adminResult.FirstName = p_administrator.FirstName;
                adminResult.LastName = p_administrator.LastName;
                adminResult.Organisations = organisations;

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
                adminDTO.Organisations = organisations;
                adminDTO.Active = true;

                this._context.Administrators.Add(adminDTO);
                this._context.SaveChanges();
            }
        }
    }
}
