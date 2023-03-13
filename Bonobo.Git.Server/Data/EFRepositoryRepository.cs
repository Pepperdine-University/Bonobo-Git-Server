using Bonobo.Git.Server.Models;
using LibGit2Sharp;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Security.Cryptography;
using Unity;

namespace Bonobo.Git.Server.Data
{
    public class EFRepositoryRepository : IRepositoryRepository
    {
        [Dependency]
        public Func<BonoboGitServerContext> CreateContext { get; set; }

        public IList<RepositoryModel> GetAllRepositories()
        {
            using (var db = CreateContext())
            {
                var dbrepos = db.Repositories.Select(repo => new
                {
                    Id = repo.Id,
                    Name = repo.Name,
                    Group = repo.Group,
                    Description = repo.Description,
                    AnonymousAccess = repo.Anonymous,
                    Users = repo.Users,
                    Teams = repo.Teams,
                    Administrators = repo.Administrators,
                    AuditPushUser = repo.AuditPushUser,
                    AllowAnonPush = repo.AllowAnonymousPush,
                    Logo = repo.Logo,
                    ServiceAccounts = repo.ServiceAccounts,
                    Dependencies = repo.Dependencies,
                }).ToList();

                return dbrepos.Select(repo => new RepositoryModel
                {
                    Id = repo.Id,
                    Name = repo.Name,
                    Group = repo.Group,
                    Description = repo.Description,
                    AnonymousAccess = repo.AnonymousAccess,
                    Users = repo.Users.Select(user => user.ToModel()).ToArray(),
                    Teams = repo.Teams.Select(TeamToTeamModel).ToArray(),
                    Administrators = repo.Administrators.Select(user => user.ToModel()).ToArray(),
                    AuditPushUser = repo.AuditPushUser,
                    AllowAnonymousPush = repo.AllowAnonPush,
                    Logo = repo.Logo,
                    ServiceAccounts = repo.ServiceAccounts.ToList(),
                    Dependencies = repo.Dependencies.ToList()
                }).ToList();
            }
        }

        public IList<KnownDependency> GetAllKnownDependencies()
        {
            using (var db = CreateContext())
            {
                var knownDependencies = db.KnownDependencies.Select(knownDependency => new
                {
                    Id = knownDependency.Id,
                    ComponentName = knownDependency.ComponentName
                }).ToList();

                return knownDependencies.Select(knownDependency => new KnownDependency
                {
                    Id = knownDependency.Id,
                    ComponentName = knownDependency.ComponentName
                }).ToList();
            }
        }

        public RepositoryModel GetRepository(string name)
        {
            if (name == null) throw new ArgumentNullException("name");

            /* The straight-forward solution of using FindFirstOrDefault with
             * string.Equal does not work. Even name.Equals with OrdinalIgnoreCase does not
             * as it seems to get translated into some specific SQL syntax and EF does not
             * provide case insensitive matching :( */
            var repos = GetAllRepositories();
            foreach (var repo in repos)
            {
                if (repo.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    return repo;
                }
            }
            return null;
        }

        public RepositoryModel GetRepository(Guid id)
        {
            using (var db = CreateContext())
            {
                return ConvertToModel(db.Repositories.Include(r => r.Dependencies
                                                             .Select(d => d.KnownDependency))
                                                     .First(i => i.Id.Equals(id)));
            }
        }

        public void Delete(Guid id)
        {
            using (var db = CreateContext())
            {
                var repo = db.Repositories.FirstOrDefault(i => i.Id == id);
                if (repo != null)
                {
                    repo.Administrators.Clear();
                    repo.Users.Clear();
                    repo.Teams.Clear();
                    //removes service accounts and dependencies from repo when deleting repo
                    foreach (ServiceAccount serviceAccount in repo.ServiceAccounts.ToList())
                    {
                        db.ServiceAccounts.Remove(serviceAccount);
                    }
                    foreach (Dependency dependency in repo.Dependencies.ToList())
                    {
                        db.Dependencies.Remove(dependency);
                    }
                    db.Repositories.Remove(repo);
                    db.SaveChanges();
                }
            }
        }
        
        public bool NameIsUnique(string newName, Guid ignoreRepoId)
        {
            var repo = GetRepository(newName);
            return repo == null || repo.Id == ignoreRepoId;
        }

        public bool Create(RepositoryModel model)
        {
            if (model == null) throw new ArgumentException("model");
            if (model.Name == null) throw new ArgumentException("name");

            using (var database = CreateContext())
            {
                model.EnsureCollectionsAreValid();
                model.Id = Guid.NewGuid();
                var repository = new Repository
                {
                    Id = model.Id,
                    Name = model.Name,
                    Logo = model.Logo,
                    Group = model.Group,
                    Description = model.Description,
                    Anonymous = model.AnonymousAccess,
                    AllowAnonymousPush = model.AllowAnonymousPush,
                    AuditPushUser = model.AuditPushUser,
                    LinksUseGlobal = model.LinksUseGlobal,
                    LinksUrl = model.LinksUrl,
                    LinksRegex = model.LinksRegex,
                    ServiceAccounts = model.ServiceAccounts
                };
                database.Repositories.Add(repository);
                AddMembers(model.Users.Select(x => x.Id), model.Administrators.Select(x => x.Id), model.Teams.Select(x => x.Id), repository, database);
                //Adds dependencies to model when creating new repository with a randomly generated Guid id
                if (model.Dependencies != null)
                {
                    foreach (var dependency in model.Dependencies.ToList())
                    {
                        dependency.KnownDependency = database.KnownDependencies.FirstOrDefault(i => i.Id == dependency.KnownDependenciesId);
                        dependency.Id = Guid.NewGuid();
                        dependency.RepositoryId = model.Id;
                        repository.Dependencies.Add(dependency);
                    }
                }
                //Adds service accounts to model when creating new repository  with a randomly generated Guid id
                if (model.ServiceAccounts != null)
                {
                    foreach (var serviceAccount in model.ServiceAccounts.ToList())
                    {
                        serviceAccount.Id = Guid.NewGuid();
                        serviceAccount.RepositoryId = model.Id;
                        repository.ServiceAccounts.Add(serviceAccount);
                    }
                }
                try
                {
                    database.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    Log.Error(ex, "Failed to create repo {RepoName}", model.Name);
                    return false;
                }
                catch (UpdateException)
                {
                    return false;
                }
                return true;
            }
        }

        public bool EFCreateKnownDependency(KnownDependency knownDependency)
        {
            using (var database = CreateContext())
            {
                var knownDep = new KnownDependency
                {
                    Id = knownDependency.Id,
                    ComponentName = knownDependency.ComponentName,
                };
                database.KnownDependencies.Add(knownDep);
                try
                {
                    database.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    Log.Error(ex, "Failed to create known dependency {RepoName}", knownDependency.ComponentName);
                    return false;
                }
                catch (UpdateException ex)
                {
                    Log.Error(ex, "Failed to update {RepoName}", knownDependency.ComponentName);
                    return false;
                }
                return true;
            }
        }

        public void Update(RepositoryModel model)
        {
            if (model == null) throw new ArgumentException("model");
            if (model.Name == null) throw new ArgumentException("name");

            using (var db = CreateContext())
            {
                var repo = db.Repositories
                                .Include(r => r.ServiceAccounts)
                                .Include(r => r.Dependencies.Select(d => d.KnownDependency))
                                .FirstOrDefault(i => i.Id == model.Id);

                if (repo != null)
                {
                    model.EnsureCollectionsAreValid();

                    repo.Name = model.Name;
                    repo.Group = model.Group;
                    repo.Description = model.Description;
                    repo.Anonymous = model.AnonymousAccess;
                    repo.AuditPushUser = model.AuditPushUser;
                    repo.AllowAnonymousPush = model.AllowAnonymousPush;
                    repo.LinksRegex = model.LinksRegex;
                    repo.LinksUrl = model.LinksUrl;
                    repo.LinksUseGlobal = model.LinksUseGlobal;

                    if (model.ServiceAccounts != null)
                    {
                        //Update Service accounts in database when added with javascript
                        foreach (var serviceAccount in model.ServiceAccounts.ToList())
                        {
                            var existingServiceAccount = repo.ServiceAccounts
                                .Where(c => c.Id == serviceAccount.Id && c.Id != Guid.Parse("94cab580-0018-4a00-9bb4-bed27234ff22"))
                                .SingleOrDefault();
                            
                            if (existingServiceAccount != null)
                            {
                                existingServiceAccount.RepositoryId = model.Id;
                                db.Entry(existingServiceAccount).CurrentValues.SetValues(serviceAccount);
                            }
                            else
                            {
                                serviceAccount.Id = Guid.NewGuid();
                                serviceAccount.RepositoryId = model.Id;
                                repo.ServiceAccounts.Add(serviceAccount);
                            }
                        }
                        //Updates Service accounts in database when deleted with javascript
                        foreach (var serviceAccount in repo.ServiceAccounts.Where(repoServiceAccount => model.ServiceAccounts.All(modelServiceAccount => modelServiceAccount.Id != repoServiceAccount.Id)).ToList())
                        {
                            var removedServiceAccount = db.ServiceAccounts.FirstOrDefault(i => i.Id == serviceAccount.Id);
                            db.ServiceAccounts.Remove(removedServiceAccount);
                        }
                    }
                    else
                    {
                        foreach(var serviceAccount in repo.ServiceAccounts.ToList())
                        {
                            var removedServiceAccount = db.ServiceAccounts.FirstOrDefault(i => i.Id == serviceAccount.Id);
                            db.ServiceAccounts.Remove(removedServiceAccount);
                        }
                    }
                    
                
                    if (model.Dependencies != null)
                    {
                        //Updates Dependencies in database when added with javascript
                        // could check if the dependency.knowndependencyid is equal to the add new id, then you know you have to look in the input field, but how do u get access to the contents of that input field from the backend?
                        foreach (var dependency in model.Dependencies.ToList())
                        {
                            dependency.KnownDependency = db.KnownDependencies.FirstOrDefault(i => i.Id == dependency.KnownDependenciesId);
                            var existingDependency = repo.Dependencies
                                .Where(c => c.Id == dependency.Id && c.Id != null)
                                .SingleOrDefault();

                            if (existingDependency != null)
                            {
                                existingDependency.RepositoryId = model.Id;
                                db.Entry(existingDependency).CurrentValues.SetValues(dependency);
                            }
                            else
                            {
                                dependency.Id = Guid.NewGuid();
                                dependency.RepositoryId = model.Id;
                                repo.Dependencies.Add(dependency);
                            }
                        }
                        //Updates Dependencies in database when deleted with javascript
                        foreach (var dependency in repo.Dependencies.Where(repoDep => model.Dependencies.All(modelDep => modelDep.Id != repoDep.Id)).ToList())
                        {
                            var dep = db.Dependencies.FirstOrDefault(i => i.Id == dependency.Id);
                            db.Dependencies.Remove(dep);
                        }
                    }
                    else
                    {
                        foreach (var dependency in repo.Dependencies.ToList())
                        {
                            var dep = db.Dependencies.FirstOrDefault(i => i.Id == dependency.Id);
                            db.Dependencies.Remove(dep);
                        }
                    }

                    if (model.Logo != null)
                        repo.Logo = model.Logo;

                    if (model.RemoveLogo)
                        repo.Logo = null;

                    repo.Users.Clear();
                    repo.Teams.Clear();
                    repo.Administrators.Clear();

                    AddMembers(model.Users.Select(x => x.Id), model.Administrators.Select(x => x.Id), model.Teams.Select(x => x.Id), repo, db);
                    db.SaveChanges();
                }
            }
        }



        private TeamModel TeamToTeamModel(Team t)
        {
            return new TeamModel
            {
                Id = t.Id,
                Name = t.Name,
                Description = t.Description,
                Members = t.Users.Select(user => user.ToModel()).ToArray()
            };
        }

        private RepositoryModel ConvertToModel(Repository item)
        {
            if (item == null)
            {
                return null;
            }

            return new RepositoryModel
            {
                Id = item.Id,
                Name = item.Name,
                Group = item.Group,
                Description = item.Description,
                AnonymousAccess = item.Anonymous,
                Users = item.Users.Select(user => user.ToModel()).ToArray(),
                Teams = item.Teams.Select(TeamToTeamModel).ToArray(),
                Administrators = item.Administrators.Select(user => user.ToModel()).ToArray(),
                AuditPushUser = item.AuditPushUser,
                AllowAnonymousPush = item.AllowAnonymousPush,
                Logo = item.Logo,
                LinksRegex = item.LinksRegex,
                LinksUrl = item.LinksUrl,
                LinksUseGlobal = item.LinksUseGlobal,
                ServiceAccounts = item.ServiceAccounts.ToList(),
                Dependencies = item.Dependencies.ToList()

            };
        }

        private void AddMembers(IEnumerable<Guid> users, IEnumerable<Guid> admins, IEnumerable<Guid> teams, Repository repo, BonoboGitServerContext database)
        {
            if (admins != null)
            {
                var administrators = database.Users.Where(i => admins.Contains(i.Id));
                foreach (var item in administrators)
                {
                    repo.Administrators.Add(item);
                }
            }

            if (users != null)
            {
                var permittedUsers = database.Users.Where(i => users.Contains(i.Id));
                foreach (var item in permittedUsers)
                {
                    repo.Users.Add(item);
                }
            }

            if (teams != null)
            {
                var permittedTeams = database.Teams.Where(i => teams.Contains(i.Id));
                foreach (var item in permittedTeams)
                {
                    repo.Teams.Add(item);
                }
            }


        }

        public IList<RepositoryModel> GetTeamRepositories(Guid[] teamsId)
        {
            return GetAllRepositories().Where(repo => repo.Teams.Any(team => teamsId.Contains(team.Id))).ToList();
        }
    }
}
