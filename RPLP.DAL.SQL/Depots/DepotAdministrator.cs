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

        public Administrator GetAdministratorById(int p_id)
        {
            Administrator administrator = this._context.Administrators.Where(admin => admin.Id == p_id).Select(admin => admin.ToEntity()).FirstOrDefault();

            if (administrator == null)
                return new Administrator();

            return administrator;
        }

        public List<Administrator> GetAdministrators()
        {
            return this._context.Administrators.Select(admin => admin.ToEntity()).ToList();
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

            Administrator_SQLDTO adminResult = this._context.Administrators.Where(admin => admin.Id == p_administrator.Id).FirstOrDefault();

            if (adminResult != null)
            {
                adminResult.Id = p_administrator.Id;
                adminResult.Username = p_administrator.Username;
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
