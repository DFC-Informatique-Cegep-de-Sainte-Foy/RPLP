using Microsoft.EntityFrameworkCore;
using RPLP.DAL.DTO.Sql;
using RPLP.ENTITES;
using RPLP.JOURNALISATION;
using RPLP.ENTITES.InterfacesDepots;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RPLP.DAL.SQL.Depots
{
    public class DepotRepository : IDepotRepository
    {
        private readonly RPLPDbContext _context;

        public DepotRepository(RPLPDbContext p_context)
        {
            if (p_context == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                               "DepotRepository - DepotRepository(RPLPDbContext p_context) - p_context de type RPLPDbContext passé en paramètre est null", 0));
            }

            this._context = p_context;
        }

        public Repository GetRepositoryById(int id)
        {
            if (id < 0)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentOutOfRangeException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                       "DepotRepository - GetRepositoryById - id passé en paramêtre est hors des limites", 0));
            }

            Repository_SQLDTO repository = this._context.Repositories.FirstOrDefault(repository => repository.Id == id && repository.Active);

            if (repository == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Repository", $"DepotRepository - Method - GetRepositoryById(int id) - Return Repository - repository est null",0));
                return null;
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Repository", $"DepotRepository - Method - GetRepositoryById(int id) - Return Repository"));
            }

            return repository.ToEntity();
        }

        public Repository GetRepositoryByName(string p_repositoryName)
        {
            if (string.IsNullOrWhiteSpace(p_repositoryName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotRepository - GetRepositoryByName - p_repositoryName passé en paramètre est vide", 0));
            }

            Repository_SQLDTO? repository = this._context.Repositories.FirstOrDefault(repository => repository.Name == p_repositoryName && repository.Active);

            if (repository == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Repository", $"DepotRepository - Method - GetRepositoryByName(string p_repositoryName) - Return Repository - repository est null",0));
                return null;
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Repository", $"DepotRepository - Method - GetRepositoryByName(string p_repositoryName) - Return Repository"));
            }

            return repository.ToEntity();
        }

        public void UpsertRepository(Repository p_repository)
        {
            if (p_repository == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotRepository - UpsertRepository - p_repository passé en paramètre est null", 0));
            }

            Repository_SQLDTO? repositoryResult = this._context.Repositories.SingleOrDefault(repository => repository.Id == p_repository.Id);
            if (repositoryResult != null)
            {
                repositoryResult.Name = p_repository.Name;
                repositoryResult.OrganisationName = p_repository.OrganisationName;
                repositoryResult.FullName = p_repository.FullName;
                repositoryResult.Active = true;

                this._context.Update(repositoryResult);
                this._context.SaveChanges();

                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Repository", $"DepotRepository - Method - UpsertRepository(Repository p_repository) - Void - Update Repository"));
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

                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Repository", $"DepotRepository - Method - UpsertRepository(Repository p_repository) - Void - Add Repository"));
            }
        }

        public void DeleteRepository(string p_repositoryName)
        {
            if(this._context.ChangeTracker != null)
            {
                Logging.Instance.Journal(new Log($"DeleteRepository - J'ai passé le this._context.ChangeTracker"));
                this._context.ChangeTracker.Clear();
            }
            

            Logging.Instance.Journal(new Log($"DeleteRepository - Debut - p_repositoryName {p_repositoryName}"));
            if (string.IsNullOrWhiteSpace(p_repositoryName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotRepository - DeleteRepository - p_repositoryName passé en paramètre est vide", 0));
            }

            Logging.Instance.Journal(new Log($"DeleteRepository - Debut - p_repositoryName {p_repositoryName}"));

            Repository_SQLDTO? repositoryResult = this._context.Repositories.FirstOrDefault(repository => repository.Name == p_repositoryName);
            
            if (repositoryResult != null)
            {
                Logging.Instance.Journal(new Log($"DeleteRepository - repositoryResult not null - repositoryResult.Name {repositoryResult.Name}"));

                repositoryResult.Active = false;
                this._context.Update(repositoryResult);
                this._context.SaveChanges();

                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Repository", $"DepotRepository - Method - DeleteRepository(string p_repositoryName : {p_repositoryName}) - Void - delete repository"));
            }
            else
            {
                Logging.Instance.Journal(new Log($"DeleteRepository - repositoryResult null"));

                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Repository", $"DepotRepository - Method - DeleteRepository(string p_repositoryName) - Void - repositoryResult est null",0));
            }
            
        }

        public List<Repository> GetRepositories()
        {
            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Repository", $"DepotRepository - Method - GetRepositories() - Return List<Repository>"));

            return this._context.Repositories.Where(repository => repository.Active)
                .Select(repository => repository.ToEntity())
                .ToList();
        }

        public List<Repository> GetRepositoriesFromOrganisationName(string p_organisationName)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotRepository - GetRepositoriesFromOrganisationName - p_organisationName passé en paramètre est vide", 0));
            }
            List<Repository> repositories = this._context.Repositories
                .Where(repository => repository.Active && repository.OrganisationName == p_organisationName)
                .Select(repository => repository.ToEntity())
                .ToList();

            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Repository", $"DepotRepository - Method - GetRepositoriesFromOrganisationName(string p_organisationName) - Return List<Repository> Count: {repositories.Count}"));

            return repositories;
        }

        public void ReactivateRepository(string p_repositoryName)
        {
            if (string.IsNullOrWhiteSpace(p_repositoryName))
            {
                Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(), new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                     "DepotRepository - ReactivateRepository - p_repositoryName passé en paramètre est vide", 0));
                throw new ArgumentNullException(nameof(p_repositoryName));
            }

            Repository_SQLDTO? repositoryResult = this._context.Repositories.FirstOrDefault(repository => repository.Name == p_repositoryName);

            if (repositoryResult != null)
            {
                repositoryResult.Active = true;

                this._context.Update(repositoryResult);
                this._context.SaveChanges();

                Logging.Instance.Journal(new Log("Repository", $"DepotRepository - Method - ReactivateRepository(string p_repositoryName) - Void - reactivate repository"));
            }
            else
            {
                Logging.Instance.Journal(new Log("Repository", $"DepotRepository - Method - ReactivateRepository(string p_repositoryName) - Void - repositoryResult est null", 0));
            }
        }
    }
}
