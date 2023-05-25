using RPLP.ENTITES;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPLP.DAL.DTO.Sql
{
    public class Repository_SQLDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int OrganisationId { get; set; }
        [ForeignKey("OrganisationId")]
        public Organisation_SQLDTO Organisation { get; set; }
        public string FullName { get; set; }
        public bool Active { get; set; }

        public Repository_SQLDTO()
        {
            this.Organisation = new Organisation_SQLDTO();
        }

        public Repository_SQLDTO(Repository p_repository)
        {
            this.Id = p_repository.Id;
            this.Name = p_repository.Name;
            this.Organisation = new Organisation_SQLDTO(p_repository.Organisation);
            this.FullName = p_repository.FullName;
            this.Active = true;
        }

        public Repository ToEntity()
        {
            return new Repository(this.Id, this.Name, this.Organisation.ToEntity(), this.FullName);
        }
    }
}
