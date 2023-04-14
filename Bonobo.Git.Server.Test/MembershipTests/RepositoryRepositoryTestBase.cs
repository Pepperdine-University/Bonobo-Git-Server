using System;
using System.Linq;
using System.Text;
using Bonobo.Git.Server.Data;
using Bonobo.Git.Server.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Bonobo.Git.Server.Test.MembershipTests
{
    public abstract class RepositoryRepositoryTestBase
    {
        protected IRepositoryRepository _repo;

        [TestMethod]
        public void NewRepoIsEmpty()
        {
            Assert.AreEqual(0, _repo.GetAllRepositories().Count);
        }

        [TestMethod]
        public void RespositoryWithNoUsersCanBeAdded()
        {
            var newRepo = MakeRepo("Repo1");

            _repo.Create(newRepo);

            Assert.AreEqual("Repo1", _repo.GetAllRepositories().Single().Name);
        }

        [TestMethod]
        public void DuplicateRepoNameAddReturnsFalse()
        {
            Assert.IsTrue(_repo.Create(MakeRepo("Repo1")));
            Assert.IsFalse(_repo.Create(MakeRepo("Repo1")));
        }

        [TestMethod]
        public void RepoNameDifferentCaseCannotBeCreated()
        {
            Assert.IsTrue(_repo.Create(MakeRepo("name")));
            Assert.IsFalse(_repo.Create(MakeRepo("NAME")));
        }

        [TestMethod]
        public void NewRepoNameIsUnique()
        {
            _repo.Create(MakeRepo("abc"));
            Assert.IsTrue(_repo.NameIsUnique("x", Guid.Empty));
        }

        [TestMethod]
        public void DuplicateRepoNameIsNotUniqueEvenIfCaseDiffers()
        {
            _repo.Create(MakeRepo("abc"));
            Assert.IsFalse(_repo.NameIsUnique("ABC", Guid.Empty));
        }

        [TestMethod]
        public void DuplicateRepoNameIsAllowedIfCurrentRepo()
        {
            var repo = MakeRepo("abc");
            _repo.Create(repo);
            Assert.IsTrue(_repo.NameIsUnique("ABC", repo.Id));
        }

        [TestMethod]
        public void GetRepoIsCaseInsensitive()
        {
            var model = MakeRepo("aaa");
            Assert.IsTrue(_repo.Create(model));
            Assert.AreEqual(model.Id, _repo.GetRepository("aaa").Id);
            Assert.AreEqual(model.Id, _repo.GetRepository("aAa").Id);
            Assert.AreEqual(model.Id, _repo.GetRepository("AAA").Id);
        }

        [TestMethod]
        public void RespositoryWithUsersCanBeAdded()
        {
            var newRepo = MakeRepo("Repo1");
            newRepo.Users = new[] { AddUserFred() };

            _repo.Create(newRepo);

            Assert.AreEqual("Fred Blogs", _repo.GetAllRepositories().Single().Users.Single().DisplayName);
        }

        [TestMethod]
        public void RespositoryWithAdministratorCanBeAdded()
        {
            var newRepo = MakeRepo("Repo1");
            newRepo.Administrators = new[] { AddUserFred() };

            _repo.Create(newRepo);

            Assert.AreEqual("Fred Blogs", _repo.GetAllRepositories().Single().Administrators.Single().DisplayName);
        }

        [TestMethod]
        public void NewRepoCanBeRetrievedById()
        {
            var newRepo1 = MakeRepo("Repo1");
            _repo.Create(newRepo1);

            Assert.AreEqual("Repo1", _repo.GetRepository(newRepo1.Id).Name);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void NonExistentRepoIdThrowsException()
        {
            var newRepo1 = MakeRepo("Repo1");
            _repo.Create(newRepo1);

            _repo.GetRepository(Guid.NewGuid());
        }

        [TestMethod]
        public void NonExistentRepoNameReturnsNull()
        {
            var newRepo1 = MakeRepo("Repo1");
            _repo.Create(newRepo1);

            Assert.IsNull(_repo.GetRepository("Repo2"));
        }

        [TestMethod]
        public void NewRepoCanBeRetrievedByName()
        {
            var newRepo1 = MakeRepo("Repo1");
            _repo.Create(newRepo1);

            Assert.AreEqual("Repo1", _repo.GetRepository("Repo1").Name);
        }

        [TestMethod]
        public void NewRepoCanBeDeleted()
        {
            _repo.Create(MakeRepo("Repo1"));
            _repo.Create(MakeRepo("Repo2"));

            _repo.Delete(_repo.GetRepository("Repo1").Id);

            Assert.AreEqual("Repo2", _repo.GetAllRepositories().Single().Name);
        }

        [TestMethod]
        public void DeletingMissingRepoIsSilentlyIgnored()
        {
            _repo.Create(MakeRepo("Repo1"));

            _repo.Delete(Guid.NewGuid());

            Assert.AreEqual("Repo1", _repo.GetAllRepositories().Single().Name);
        }

        [TestMethod]
        public void RepoSimplePropertiesAreSavedOnUpdate()
        {
            var repo = MakeRepo("Repo1");
            _repo.Create(repo);

            repo.Name = "SonOfRepo";
            repo.Group = "RepoGroup";
            repo.AnonymousAccess = true;
            repo.AuditPushUser = true;
            repo.Description = "New desc";

            _repo.Update(repo);

            var readBackRepo = _repo.GetRepository("SonOfRepo");
            Assert.AreEqual("SonOfRepo", readBackRepo.Name);
            Assert.AreEqual(repo.Group, readBackRepo.Group);
            Assert.AreEqual(repo.AnonymousAccess, readBackRepo.AnonymousAccess);
            Assert.AreEqual(repo.AuditPushUser, readBackRepo.AuditPushUser);
            Assert.AreEqual(repo.Description, readBackRepo.Description);

        }

        [TestMethod]
        public void ServiceAccountsPropertiesAreSavedOnUpdate()
        {
            var repo = MakeRepo("Repo1");
            _repo.Create(repo);
            repo.Name = "SonOfRepo";
            var serviceAccount = MakeServiceAccount("my new sa");
            repo.ServiceAccounts = new System.Collections.Generic.List<ServiceAccount>
            {
                serviceAccount
            };

            _repo.Update(repo);

            var readBackRepo = _repo.GetRepository("SonOfRepo");
            for (int i = 0; i < repo.ServiceAccounts.Count(); i++)
            {
                Assert.AreEqual(repo.ServiceAccounts[i].ServiceAccountName, readBackRepo.ServiceAccounts[i].ServiceAccountName);
                Assert.AreEqual(repo.ServiceAccounts[i].PassLastUpdated, readBackRepo.ServiceAccounts[i].PassLastUpdated);
                Assert.AreEqual(repo.ServiceAccounts[i].InPassManager, readBackRepo.ServiceAccounts[i].InPassManager);
            }
        }

        [TestMethod]
        public void ServiceAccountsCanBeRemovedLeadingToNonNull()
        {
            var repo = MakeRepo("Repo1");
            _repo.Create(repo);
            var serviceAccount1 = MakeServiceAccount("my new sa");
            var serviceAccount2 = MakeServiceAccount("deleted sa");
            repo.ServiceAccounts = new System.Collections.Generic.List<ServiceAccount>
            {
                serviceAccount1,serviceAccount2
            };
            _repo.DeleteServiceAccount(repo, serviceAccount2);
            Assert.AreEqual("my new sa", repo.ServiceAccounts.Single().ServiceAccountName);

        }

        [TestMethod]
        public void ServiceAccountsCanBeRemovedLeadingToNull()
        {
            var repo = MakeRepo("Repo1");
            _repo.Create(repo);
            repo.Name = "SonOfRepo";
            var serviceAccount = MakeServiceAccount("my new sa");
            repo.ServiceAccounts = new System.Collections.Generic.List<ServiceAccount>
            {
                serviceAccount
            };
            _repo.DeleteServiceAccount(repo, serviceAccount);
            Assert.IsTrue(repo.ServiceAccounts.Count() == 0);
        }

        [TestMethod]
        public void ServiceAccountsCanBeAddedAfterCreate()
        {
            var repo = MakeRepo("Repo1");
            var serviceAccount = MakeServiceAccount("my new sa");
            repo.ServiceAccounts = new System.Collections.Generic.List<ServiceAccount>
            {
                serviceAccount
            };
            _repo.Create(repo);
            repo.ServiceAccounts.Add(serviceAccount);
            Assert.AreEqual("my new sa", repo.ServiceAccounts.Single().ServiceAccountName);
        }

        [TestMethod]
        public void ServiceAccountsCanBeAddedAfterUpdateFromNull()
        {
            var repo = MakeRepo("Repo1");
            var serviceAccount = MakeServiceAccount("my new sa");
            repo.ServiceAccounts = new System.Collections.Generic.List<ServiceAccount>
            {
                serviceAccount
            };
            _repo.Update(repo);
            Assert.AreEqual("my new sa", repo.ServiceAccounts.Single().ServiceAccountName);
        }

        [TestMethod]
        public void ServiceAccountsCanBeAddedAfterUpdateFromNonNull()
        {
            var repo = MakeRepo("Repo1");
            var serviceAccount = MakeServiceAccount("my new sa");
            repo.ServiceAccounts = new System.Collections.Generic.List<ServiceAccount>
            {
                serviceAccount
            };
            _repo.Update(repo);
            var serviceAccount2 = MakeServiceAccount("my other new sa");
            repo.ServiceAccounts.Add(serviceAccount2);
            _repo.Update(repo);
            Assert.AreEqual("my new sa", repo.ServiceAccounts[0].ServiceAccountName);
            Assert.AreEqual("my other new sa", repo.ServiceAccounts[1].ServiceAccountName);
        }

        [TestMethod]
        public void ServiceAccountCantHaveFutureDates()
        {
            var repo = MakeRepo("Repo1");
            _repo.Create(repo);
            var serviceAccount = MakeServiceAccount("my new sa");
            serviceAccount.PassLastUpdated = DateTime.Today.AddDays(2);
            repo.ServiceAccounts = new System.Collections.Generic.List<ServiceAccount>
            {
                serviceAccount
            };
            var newServiceAccounts = repo.ServiceAccounts
                           .Where(sa => sa.PassLastUpdated > DateTime.Today)
                           .ToArray();
            Assert.AreEqual(newServiceAccounts.Length, 1);
        }

        [TestMethod]
        public void DependenciesCantHaveFutureDates()
        {
            var repo = MakeRepo("Repo1");
            _repo.Create(repo);
            var dependency = MakeDependency("jQuery", new DateTime(2000, 01, 01), "1.0.0");
            dependency.DateUpdated = DateTime.Today.AddDays(2);
            repo.Dependencies = new System.Collections.Generic.List<Dependency>
            {
                dependency
            };
            var newDependencyDateUpdated = repo.Dependencies
                           .Where(d => d.DateUpdated > DateTime.Today)
                           .ToArray();
            Assert.AreEqual(newDependencyDateUpdated.Length, 1);
        }

        [TestMethod]
        public void RepoLogoCanBeAddedAtCreation()
        {
            var repo = MakeRepo("Repo1");
            var logoBytes = Encoding.UTF8.GetBytes("Hello");
            repo.Logo = logoBytes;
            _repo.Create(repo);

            var readBackRepo = _repo.GetRepository("Repo1");
            CollectionAssert.AreEqual(logoBytes, readBackRepo.Logo);
        }


        [TestMethod]
        public void RepoLogoCanBeAddedWithUpdate()
        {
            var repo = MakeRepo("Repo1");
            _repo.Create(repo);

            var logoBytes = Encoding.UTF8.GetBytes("Hello");
            repo.Logo = logoBytes;

            _repo.Update(repo);

            var readBackRepo = _repo.GetRepository("Repo1");
            CollectionAssert.AreEqual(logoBytes, readBackRepo.Logo);
        }

        [TestMethod]
        public void RepoLogoCanBeRemovedWithUpdate()
        {
            var repo = MakeRepo("Repo1");
            _repo.Create(repo);

            repo.Logo = Encoding.UTF8.GetBytes("Hello");
            _repo.Update(repo);
            repo.RemoveLogo = true;
            _repo.Update(repo);

            Assert.IsNull(_repo.GetRepository("Repo1").Logo);
        }

        [TestMethod]
        public void RepoLogoIsPreservedWhenNullAtUpdate()
        {
            var logoBytes = Encoding.UTF8.GetBytes("Hello");
            var repo = MakeRepo("Repo1");
            repo.Logo = logoBytes;
            _repo.Create(repo);

            var updateRepo = new RepositoryModel();
            updateRepo.Id = repo.Id;
            updateRepo.Name = repo.Name;
            updateRepo.Logo = null;
            updateRepo.Users = new UserModel[0];
            updateRepo.Administrators = new UserModel[0];
            updateRepo.Teams = new TeamModel[0];
            _repo.Update(updateRepo);

            CollectionAssert.AreEqual(logoBytes, _repo.GetRepository("Repo1").Logo);
        }

        [TestMethod]
        public void NewRepositoryIsPermittedToNobody()
        {
            _repo.Create(MakeRepo("Repo1"));

            Assert.AreEqual(0, _repo.GetRepository("Repo1").Administrators.Length);
            Assert.AreEqual(0, _repo.GetRepository("Repo1").Teams.Length);
            Assert.AreEqual(0, _repo.GetRepository("Repo1").Users.Length);
        }

        [TestMethod]
        public void RepositoryIsReportedAsAccessibleToTeam()
        {
            var team = AddTeam();
            var repoWithTeam = MakeRepo("Repo1");
            repoWithTeam.Teams = new[] { team };
            _repo.Create(repoWithTeam);
            var repoWithoutTeam = MakeRepo("Repo2");
            _repo.Create(repoWithoutTeam);

            Assert.AreEqual("Repo1", _repo.GetTeamRepositories(new[] { team.Id }).Single().Name);
        }

        [TestMethod]
        public void NoReposistoriesListedIfNoneInTeam()
        {
            var team = AddTeam();
            var repoWithoutTeam1 = MakeRepo("Repo1");
            _repo.Create(repoWithoutTeam1);
            var repoWithoutTeam2 = MakeRepo("Repo2");
            _repo.Create(repoWithoutTeam2);

            Assert.AreEqual(0, _repo.GetTeamRepositories(new[] { team.Id }).Count);
        }
        [TestMethod]
        public void CanCreateDependencyOnUpdate()
        {
            var repo = MakeRepo("Repo1");
            var dependency = MakeDependency("jQuery", new DateTime(2000, 01, 01), "1.0.0");
            repo.Dependencies = new System.Collections.Generic.List<Dependency>();
            _repo.Create(repo);
            repo.Dependencies.Add(dependency);
            Assert.AreEqual("1.0.0", repo.Dependencies.Single().VersionInUse);
        }

        [TestMethod]
        public void CanCreateKnownDependencyOnUpdate()
        {
            var repo = MakeRepo("Repo1");
            var dependency = MakeDependency("jQuery", new DateTime(2000, 01, 01), "1.0.0");
            repo.Dependencies = new System.Collections.Generic.List<Dependency>();
            _repo.Create(repo);
            repo.Dependencies.Add(dependency);
            Assert.AreEqual("jQuery", repo.Dependencies.Single().KnownDependency.ComponentName);
        }

        protected abstract UserModel AddUserFred();
        protected abstract TeamModel AddTeam();

        private static RepositoryModel MakeRepo(string name)
        {
            var newRepo = new RepositoryModel();
            newRepo.Name = name;
            return newRepo;
        }
        private static ServiceAccount MakeServiceAccount(string name)
        {
            ServiceAccount serviceAccount = new ServiceAccount();
            serviceAccount.ServiceAccountName = name;
            serviceAccount.PassLastUpdated = new DateTime(2015, 5,5);
            serviceAccount.InPassManager = true;
            return serviceAccount;
        }

        private static Dependency MakeDependency(string componentName, DateTime dateUpdated, string versionInUse)
        {
            Dependency dependency = new Dependency();
            KnownDependency knownDependency = new KnownDependency();
            dependency.VersionInUse = versionInUse;
            dependency.DateUpdated = dateUpdated;
            dependency.KnownDependency = knownDependency;
            dependency.KnownDependency.ComponentName = componentName;
            return dependency;
        }
    }
}