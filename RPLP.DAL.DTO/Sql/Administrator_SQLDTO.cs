using RPLP.ENTITES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.DAL.DTO.Sql
{
    public class Administrator_SQLDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<Organisation_SQLDTO> Organisations { get; set; }
        public bool Active { get; set; }

        public Administrator_SQLDTO()
        {
            this.Organisations = new List<Organisation_SQLDTO>();
        }

        public Administrator_SQLDTO(Administrator p_admin)
        {
            List<Organisation_SQLDTO> organisations = new List<Organisation_SQLDTO>();

            this.Id = p_admin.Id;
            this.Username = p_admin.Username;
            this.Token = p_admin.Token;
            this.FirstName = p_admin.FirstName;
            this.LastName = p_admin.LastName;

            if (p_admin.Organisations.Count >= 1)
            {
                foreach (Organisation organisation in p_admin.Organisations)
                {
                    organisations.Add(new Organisation_SQLDTO(organisation));
                }
            }

            this.Organisations = organisations;
            this.Active = true;
        }

        public Administrator ToEntity()
        {
            return new Administrator(this.Id, this.Username, this.Token, this.FirstName, this.LastName, this.Organisations.Select(organisation => organisation.ToEntity()).ToList());
        }

        public Administrator ToEntityWitouthOrganisations()
        {
            return new Administrator(this.Id, this.Username, this.Token, this.FirstName, this.LastName, new List<Organisation>());
        }
    }
}
