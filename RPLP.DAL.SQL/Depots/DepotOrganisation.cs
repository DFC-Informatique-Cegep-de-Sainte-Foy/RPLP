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
    public class DepotOrganisation : IDepotOrganisation
    {
        private readonly RPLPDbContext _context;

        public DepotOrganisation()
        {
            this._context = new RPLPDbContext();
        }

        public DepotOrganisation(RPLPDbContext p_context)
        {
            this._context = p_context;
        }

        public List<Organisation> GetOrganisations()
        {
            return this._context.Organisations.Select(organisation => organisation.ToEntity()).ToList();
        }
               
        public Organisation GetOrganisationById(int p_id)
        {
            Organisation organisation = this._context.Organisations.Where(organisation => organisation.Id == p_id).Select(organisation => organisation.ToEntity()).FirstOrDefault();

            if (organisation == null)
                return new Organisation();

            return organisation;
        }

        public Organisation GetOrganisationByName(string p_name)
        {
            Organisation_SQLDTO organisationResult = this._context.Organisations.Where(organisation => organisation.Name == p_name)
                                                                       .Include(organisation => organisation.Administrators)
                                                                       .FirstOrDefault();
            if (organisationResult == null)
                return new Organisation();

            Organisation organisation = organisationResult.ToEntityWithoutList();

            if (organisationResult.Administrators.Count >= 1)
            {
                List<Administrator> administrators = organisationResult.Administrators.Select(administrator => administrator.ToEntityWithoutList()).ToList();
                organisation.Administrators = administrators;
            }
           
            return organisation;
        }

        public List<Administrator> GetAdministratorsByOrganisation(string p_organisationName)
        {
            Organisation_SQLDTO organisationResult = this._context.Organisations.Where(organisation => organisation.Name == p_organisationName)
                                                                       .Include(organisation => organisation.Administrators)
                                                                       .FirstOrDefault();

            if (organisationResult != null && organisationResult.Administrators.Count >= 1)
                return organisationResult.Administrators.Select(administrator => administrator.ToEntity()).ToList();

            return new List<Administrator>();
        }         
        
        public void AddAdministratorToOrganisation(string p_organisationName, string p_adminUsername)
        {
            Administrator_SQLDTO adminResult = this._context.Administrators.Where(admin => admin.Username == p_adminUsername).FirstOrDefault();

            if (adminResult != null)
            {
                Organisation_SQLDTO organisationResult = this._context.Organisations.Where(organisation => organisation.Name == p_organisationName).SingleOrDefault();

                if (organisationResult != null)
                {
                    organisationResult.Administrators.Add(adminResult);

                    this._context.Update(organisationResult);
                    this._context.SaveChanges();
                }
            }
        }       
        
        public void RemoveAdministratorFromOrganisation(string p_organisationName, string p_adminUsername)
        {
            Administrator_SQLDTO adminResult = this._context.Administrators.Where(admin => admin.Username == p_adminUsername).FirstOrDefault();
                        
            if (adminResult != null)
            {
                Organisation_SQLDTO organisationResult = this._context.Organisations.Where(organisation => organisation.Name == p_organisationName).SingleOrDefault();

                if (organisationResult != null)
                {
                    organisationResult.Administrators.Remove(adminResult);

                    this._context.Update(organisationResult);
                    this._context.SaveChanges();
                }
            }
        }
       
        public void UpsertOrganisation(Organisation p_organisation)
        {
            List<Administrator_SQLDTO> administrators = new List<Administrator_SQLDTO>();

            if (p_organisation.Administrators.Count >= 1)
            {
                foreach (Administrator administrator in p_organisation.Administrators)
                {
                    administrators.Add(new Administrator_SQLDTO(administrator));
                }
            }

            Organisation_SQLDTO organisationResult = this._context.Organisations.Where(organisation => organisation.Id == p_organisation.Id).FirstOrDefault();

            if (organisationResult != null)
            {
                organisationResult.Name = p_organisation.Name;
                organisationResult.Administrators = administrators;

                this._context.Update(organisationResult);
                this._context.SaveChanges();
            }
            else
            {
                Organisation_SQLDTO organisation = new Organisation_SQLDTO();
                organisation.Name = p_organisation.Name;
                organisation.Administrators = administrators;
                organisation.Active = true;

                this._context.Organisations.Add(organisation);
                this._context.SaveChanges();
            }
        }

        public void DeleteOrganisation(string p_organisationName)
        {
            Organisation_SQLDTO organisationResult = this._context.Organisations.FirstOrDefault(organisation => organisation.Name == p_organisationName);

            if (organisationResult != null)
            {
                organisationResult.Active = false;

                this._context.Update(organisationResult);
                this._context.SaveChanges();
            }
        }
    }
}
