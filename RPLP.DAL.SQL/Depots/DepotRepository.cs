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
            this._context = new RPLPDbContext();
        }

        public List<Repository> GetRepositories()
        {
            return this._context.Repositories.Where(repository => repository.Active).Select(repository => repository.ToEntity()).ToList();
        }

        public List<Repository> GetRepositoryByClassroomName(string p_classroomName)
        {
            List<Repository> repositories = new List<Repository>();
            List<Repository_SQLDTO> repositoryResult = this._context.Repositories.Where(repository => repository.Active).ToList();

            if (repositoryResult.Count >= 1)
            {
                foreach (Repository_SQLDTO repository in repositoryResult)
                {
                    string[] words = repository.FullName.Split('/');

                    if (words.Contains(p_classroomName))
                    {
                        repositories.Add(repository.ToEntity());
                    }
                }
            }

            return repositories;
        }

        public Repository GetRepositoryById(int id)
        {
            Repository repository = this._context.Repositories.Where(repository => repository.Id == id).Select(repository => repository.ToEntity()).FirstOrDefault();

            if (repository == null)
                return new Repository();

            return repository;
        }

        public Repository GetRepositoryByName(string p_repositoryName)
        {
            Repository repository = this._context.Repositories.Where(repository => repository.Name == p_repositoryName).Select(repository => repository.ToEntity()).FirstOrDefault();

            if (repository == null)
                return new Repository();

            return repository;
        }

        public void UpsertRepository(Repository p_repository)
        {
            Repository_SQLDTO repositoryResult = this._context.Repositories.Where(repository => repository.Id == p_repository.Id).SingleOrDefault();

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
    }
}
