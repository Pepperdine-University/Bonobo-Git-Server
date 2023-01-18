using Bonobo.Git.Server.Models;
using Microsoft.VisualBasic.ApplicationServices;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Reflection;
using System.Web.Configuration;
using System.Web.Services.Description;
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

                    foreach (ServiceAccount serviceAccount in repo.ServiceAccounts.ToList())
                    {
                        db.ServiceAccounts.Remove(serviceAccount);
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
                    ServiceAccounts = model.ServiceAccounts,
                    Dependencies = model.Dependencies
                };
                database.Repositories.Add(repository);
                AddMembers(model.Users.Select(x => x.Id), model.Administrators.Select(x => x.Id), model.Teams.Select(x => x.Id),  repository, database);

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




                    //foreach(var oldServiceAccount in repo.ServiceAccounts)
                    //{
                    //    //Check to see if account already exists
                    //    //Model contains the clientside (newer updated service Account) 
                    //    //Repo contains the origonal/older service account. 
                    //    var currentServiceAccount =  model.ServiceAccounts.FirstOrDefault(i => i.Id == oldServiceAccount.Id);


                    //    //If it exists, then get the current account and set values of current account to new properties
                    //    SetValuesAndLastModified(currentServiceAccount, oldServiceAccount, oldServiceAccount.UserId);



                    //}



                    //If it does not exist, add the new Service Account to db.ServiceAccounts
                    if (model.ServiceAccounts != null)
                    {
                        foreach (var serviceAccount in model.ServiceAccounts.ToList())
                        {
                            var existingServiceAccount = repo.ServiceAccounts
                                .Where(c => c.Id == serviceAccount.Id && c.Id != -1)
                                .SingleOrDefault();

                            if (existingServiceAccount != null)
                            {
                                existingServiceAccount.RepositoryId = model.Id;
                                db.Entry(existingServiceAccount).CurrentValues.SetValues(serviceAccount);
                            }
                            else
                            {
                                repo.ServiceAccounts.Add(serviceAccount);
                            }
                        }
                    }

                    if (model.Dependencies != null)
                    {
                        foreach(var dependency in model.Dependencies.ToList())
                        {
                            var existingDependency = repo.Dependencies
                                .Where(c => c.Id == dependency.Id && c.Id != "")
                                .SingleOrDefault();

                           // var existingKnownDependency = repo.Dependencies;

                            if (existingDependency != null)
                            {
                                existingDependency.RepositoryId = model.Id;
                                existingDependency.KnownDependency = dependency.KnownDependency;
                                db.Entry(existingDependency).CurrentValues.SetValues(dependency);
                            }
                            else
                            {
                                repo.Dependencies.Add(dependency);
                            }
                        }
                    }
                    //updateObject((ICollection<object>)repo.ServiceAccounts, model.ServiceAccounts, db);

                    if (model.Logo != null)
                        repo.Logo = model.Logo;

                    if (model.RemoveLogo)
                        repo.Logo = null;
                    
                    repo.Users.Clear();
                    repo.Teams.Clear();
                    repo.Administrators.Clear();

                    AddMembers(model.Users.Select(x => x.Id),  model.Administrators.Select(x => x.Id), model.Teams.Select(x => x.Id), repo, db);
                    db.SaveChanges();
                }
            }
        }

        //private void updateObject(ICollection<object> currentObject, ICollection<object> newObject, BonoboGitServerContext db)
        //{
        //    if (newObject != null)
        //    {
        //        foreach (var objectX in newObject.ToList())
        //        {
        //            var existingObject = currentObject
        //                .Where(c => c.Id == objectX.Id && c.Id != -1)
        //                .SingleOrDefault();

        //            if (existingObject != null)
        //            {
        //                existingObject.RepositoryId = newObject.Id;
        //                db.Entry(existingObject).CurrentValues.SetValues(objectX);
        //            }
        //            else
        //            {
        //                currentObject.Add(objectX);
        //            }
        //        }
        //    }
        //}

       

        //get func
        private ServiceAccount GetCurrentServiceAccount(int id)
        {
            using (var db = CreateContext())
            {
                var sa = db.ServiceAccounts.FirstOrDefault(i => i.Id == id);
                return new ServiceAccount
                {
                    Id = sa.Id,
                    ServiceAccountName = sa.ServiceAccountName,
                    InPassManager = sa.InPassManager,
                    PassLastUpdated = sa.PassLastUpdated
                };
            }
        }


        private DbEntityEntry SetValuesAndLastModified(object currentServiceAccount, object updatedServiceAccount, string userID)
        {
            if (currentServiceAccount == null)
            {
                return null;
            }

            DbEntityEntry entry = (DbEntityEntry)currentServiceAccount;
            
            if (entry.State == EntityState.Modified)
            {
                entry.CurrentValues.SetValues(updatedServiceAccount);
            }

            return entry;
        }

     
        // might use like this
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

/*        private void UpdateKnownDependencies(Repository repo, BonoboGitServerContext database)
        //{
            //Want to pull KnownDependecies Id
            //var KnownDependeciesId = database.Dependencies.Where(i => )
                public IList<RepositoryModel> UpdateKnownDependencies(Guid[] DependenciesId)
            {
                return GetAllRepositories().Where(repo => repo.Dependencies.Any(Dependency => DependenciesId.Contains(Dependency.KnownDependenciesId))).ToList();
            }
            //Set it to the value
        }*/

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
