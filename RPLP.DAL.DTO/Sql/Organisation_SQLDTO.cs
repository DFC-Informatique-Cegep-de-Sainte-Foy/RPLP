using RPLP.ENTITES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.DAL.DTO.Sql
{
    public class Organisation_SQLDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Administrator_SQLDTO> Administrators { get; set; }
        public bool Active { get; set; }

        public Organisation_SQLDTO()
        {
            this.Administrators = new List<Administrator_SQLDTO>();
        }

        public Organisation_SQLDTO(Organisation p_organisation)
        {
            List<Administrator_SQLDTO> administrators = new List<Administrator_SQLDTO>();

            this.Id = p_organisation.Id;
            this.Name = p_organisation.Name;

            if (p_organisation.Administrators.Count >= 1)
            {
                foreach (Administrator administrator in p_organisation.Administrators)
                {
                    administrators.Add(new Administrator_SQLDTO(administrator));
                }
            }

            this.Administrators = administrators;
            this.Active = true;
        }

        public Organisation ToEntity()
        {
            return new Organisation(this.Id, this.Name, this.Administrators.Select(admin => admin.ToEntity()).ToList());
        }

        public Organisation ToEntityWithoutAdministrators()
        {
            return new Organisation(this.Id, this.Name, new List<Administrator>());
        }
    }
}
