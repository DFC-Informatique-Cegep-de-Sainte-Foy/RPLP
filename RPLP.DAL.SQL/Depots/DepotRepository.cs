﻿using Microsoft.EntityFrameworkCore;
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
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotRepository - DepotRepository(RPLPDbContext p_context) - p_context de type RPLPDbContext passé en paramètre est null",
                    0));
            }

            this._context = p_context;
        }

        public Repository GetRepositoryById(int id)
        {
            if (id < 0)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentOutOfRangeException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotRepository - GetRepositoryById - id passé en paramêtre est hors des limites", 0));
            }

            Repository_SQLDTO repository =
                this._context.Repositories.FirstOrDefault(repository => repository.Id == id && repository.Active);

            if (repository == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Repository",
                    $"DepotRepository - Method - GetRepositoryById(int id) - Return Repository - repository est null",
                    0));
                return null;
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Repository",
                    $"DepotRepository - Method - GetRepositoryById(int id) - Return Repository"));
            }

            return repository.ToEntity();
        }

        public Repository GetRepositoryByName(string p_repositoryName)
        {
            if (string.IsNullOrWhiteSpace(p_repositoryName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotRepository - GetRepositoryByName - p_repositoryName passé en paramètre est vide", 0));
            }

            Repository_SQLDTO? repository = this._context.Repositories.FirstOrDefault(repository =>
                repository.Name == p_repositoryName && repository.Active);

            if (repository == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Repository",
                    $"DepotRepository - Method - GetRepositoryByName(string p_repositoryName) - Return Repository - repository est null",
                    0));
                return null;
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Repository",
                    $"DepotRepository - Method - GetRepositoryByName(string p_repositoryName) - Return Repository"));
            }

            return repository.ToEntity();
        }

        public void UpsertRepository(Repository p_repository)
        {
            if (p_repository == null)
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotRepository - UpsertRepository - p_repository passé en paramètre est null", 0));
            }

            Repository_SQLDTO? repositoryResult =
                this._context.Repositories.SingleOrDefault(repository => repository.Name == p_repository.Name);

            if (repositoryResult != null)
            {
                repositoryResult.Name = p_repository.Name;
                repositoryResult.Organisation = this._context.Organisations.AsNoTrackingWithIdentityResolution()
                    .SingleOrDefault(o => o.Id == repositoryResult.OrganisationId);
                repositoryResult.FullName = p_repository.FullName;
                repositoryResult.Active = true;

                this._context.Entry<Organisation_SQLDTO>(repositoryResult.Organisation).State = EntityState.Unchanged;

                this._context.Update(repositoryResult);
                this._context.SaveChanges();

                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Repository",
                    $"DepotRepository - Method - UpsertRepository(Repository p_repository) - Void - Update Repository"));
            }
            else
            {
                Repository_SQLDTO repository = new Repository_SQLDTO();
                repository.Name = p_repository.Name;
                repository.Organisation = this._context.Organisations.AsNoTrackingWithIdentityResolution()
                    .SingleOrDefault(o => o.Id == p_repository.Organisation.Id);
                repository.FullName = p_repository.FullName;
                repository.Active = true;

                this._context.Entry<Organisation_SQLDTO>(repository.Organisation).State = EntityState.Unchanged;

                this._context.Repositories.Add(repository);
                this._context.SaveChanges();

                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Repository",
                    $"DepotRepository - Method - UpsertRepository(Repository p_repository) - Void - Add Repository"));
            }
        }

        public void DeleteRepository(string p_repositoryName)
        {
            if (this._context.ChangeTracker != null)
            {
                this._context.ChangeTracker.Clear();
            }

            if (string.IsNullOrWhiteSpace(p_repositoryName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotRepository - DeleteRepository - p_repositoryName passé en paramètre est vide", 0));
            }

            Repository_SQLDTO? repositoryResult =
                this._context.Repositories.FirstOrDefault(repository => repository.Name == p_repositoryName);

            if (repositoryResult != null)
            {
                repositoryResult.Organisation =
                    this._context.Organisations.AsNoTracking().SingleOrDefault(o => o.Id == repositoryResult.OrganisationId);
                repositoryResult.Active = false;
                this._context.Entry<Organisation_SQLDTO>(repositoryResult.Organisation).State = EntityState.Unchanged;

                this._context.Update(repositoryResult);
                this._context.SaveChanges();

                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Repository",
                    $"DepotRepository - Method - DeleteRepository(string p_repositoryName : {p_repositoryName}) - Void - delete repository"));
            }
            else
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Repository",
                    $"DepotRepository - Method - DeleteRepository(string p_repositoryName) - Void - repositoryResult est null",
                    0));
            }
        }

        public List<Repository> GetRepositories()
        {
            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Repository",
                $"DepotRepository - Method - GetRepositories() - Return List<Repository>"));

            List<Repository_SQLDTO> reposInDb =
                this._context.Repositories.Where(repository => repository.Active).ToList();
            reposInDb.ForEach(r =>
                r.Organisation = this._context.Organisations.AsNoTracking().SingleOrDefault(o => o.Id == r.OrganisationId));
            return reposInDb.Select(r => r.ToEntity()).ToList();
        }

        public List<Repository> GetRepositoriesFromOrganisationName(string p_organisationName)
        {
            if (string.IsNullOrWhiteSpace(p_organisationName))
            {
                RPLP.JOURNALISATION.Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotRepository - GetRepositoriesFromOrganisationName - p_organisationName passé en paramètre est vide",
                    0));
            }

            int organisationId = this._context.Organisations.AsNoTracking().SingleOrDefault(o => o.Name == p_organisationName).Id;

            List<Repository> repositories = this._context.Repositories
                .Where(repository => repository.Active && repository.Organisation.Id == organisationId)
                .Select(repository => repository.ToEntity())
                .ToList();

            RPLP.JOURNALISATION.Logging.Instance.Journal(new Log("Repository",
                $"DepotRepository - Method - GetRepositoriesFromOrganisationName(string p_organisationName) - Return List<Repository> Count: {repositories.Count}"));

            return repositories;
        }

        public void ReactivateRepository(string p_repositoryName)
        {
            if (string.IsNullOrWhiteSpace(p_repositoryName))
            {
                Logging.Instance.Journal(new Log(new ArgumentNullException().ToString(),
                    new StackTrace().ToString().Replace(System.Environment.NewLine, "."),
                    "DepotRepository - ReactivateRepository - p_repositoryName passé en paramètre est vide", 0));
                throw new ArgumentNullException(nameof(p_repositoryName));
            }

            Repository_SQLDTO? repositoryResult =
                this._context.Repositories.FirstOrDefault(repository => repository.Name == p_repositoryName);

            if (repositoryResult != null)
            {
                if (this._context.ChangeTracker != null)
                {
                    this._context.ChangeTracker.Clear();
                    repositoryResult.Organisation =
                        this._context.Organisations.AsNoTracking().SingleOrDefault(o => o.Id == repositoryResult.OrganisationId);
                    repositoryResult.Active = true;
                    this._context.Entry<Organisation_SQLDTO>(repositoryResult.Organisation).State =
                        EntityState.Unchanged;
                }

                this._context.Update(repositoryResult);
                this._context.SaveChanges();

                Logging.Instance.Journal(new Log("Repository",
                    $"DepotRepository - Method - ReactivateRepository(string p_repositoryName) - Void - reactivate repository"));
            }
            else
            {
                Logging.Instance.Journal(new Log("Repository",
                    $"DepotRepository - Method - ReactivateRepository(string p_repositoryName) - Void - repositoryResult est null",
                    0));
            }
        }
    }
}