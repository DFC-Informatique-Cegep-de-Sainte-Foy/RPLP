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
    public class DepotRepository : IDepotRepository
    {
        private readonly RPLPDbContext _context;

        public DepotRepository()
        {
            this._context = new RPLPDbContext(new DbContextOptionsBuilder<RPLPDbContext>().UseSqlServer("Server=rplp.db; Database=RPLP; User Id=sa; password=Cad3pend86!").Options);
            //this._context = new RPLPDbContext(new DbContextOptionsBuilder<RPLPDbContext>().UseSqlServer("Server=localhost,1433; Database=RPLP; User Id=sa; password=Cad3pend86!").Options);
        }

        public DepotRepository(RPLPDbContext p_context)
        {
            this._context = p_context;
        }

        public Repository GetRepositoryById(int id)
        {
            Repository_SQLDTO repository = this._context.Repositories.FirstOrDefault(repository => repository.Id == id && repository.Active);
            if (repository == null)
                return null;

            return repository.ToEntity();
        }

        public Repository GetRepositoryByName(string p_repositoryName)
        {
            Repository_SQLDTO repository = this._context.Repositories.FirstOrDefault(repository => repository.Name == p_repositoryName && repository.Active);

            if (repository == null)
                return null;

            return repository.ToEntity();
        }

        public void UpsertRepository(Repository p_repository)
        {
            Repository_SQLDTO repositoryResult = this._context.Repositories.SingleOrDefault(repository => repository.Id == p_repository.Id && 
                                                                                            repository.Active);
            if (repositoryResult != null)
            {
                repositoryResult.Name = p_repository.Name;
                repositoryResult.OrganisationName = p_repository.OrganisationName;
                repositoryResult.FullName = p_repository.FullName;

                this._context.Update(repositoryResult);
                this._context.SaveChanges();
            }
            else
            {
                Repository_SQLDTO repository = new Repository_SQLDTO();
                repository.Name = p_repository.Name;
                repository.OrganisationName = p_repository.OrganisationName;
                repository.FullName = p_repository.FullName;
                repository.Active = true;

                this._context.Repositories.Add(repository);
                this._context.SaveChanges();
            }
        }

        public void DeleteRepository(string p_repositoryName)
        {
            Repository_SQLDTO repositoryResult = this._context.Repositories.Where(repository => repository.Active)
                                                                           .FirstOrDefault(repository => repository.Name == p_repositoryName);
            if (repositoryResult != null)
            {
                repositoryResult.Active = false;

                this._context.Update(repositoryResult);
                this._context.SaveChanges();
            }
        }

        public List<Repository> GetRepositories()
        {
            return this._context.Repositories.Where(repository => repository.Active).Select(repository => repository.ToEntity()).ToList();
        }

        public List<Repository> GetRepositoriesFromOrganisationName(string p_organisationName)
        {
            return this._context.Repositories
                .Where(repository => repository.Active && repository.OrganisationName == p_organisationName)
                .Select(repository => repository.ToEntity())
                .ToList();
        }
    }
}
