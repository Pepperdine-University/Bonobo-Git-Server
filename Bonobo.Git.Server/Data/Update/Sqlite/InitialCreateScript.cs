
using System;

namespace Bonobo.Git.Server.Data.Update.Sqlite
{
    public class InitialCreateScript : IUpdateScript
    {
        public string Command
        {
            get 
            {
                // If you modify this scheme make sure to introduce an unit test for the new scheme.
                return string.Format(@"

                    CREATE TABLE IF NOT EXISTS [Repository] (
                        [Id] Char(36) PRIMARY KEY NOT NULL,
                        [Name] VarChar(255) Not Null UNIQUE,
                        [Description] VarChar(255) Null,
                        [Anonymous] Bit Not Null,
                        [AllowAnonymousPush] Integer NULL Default {0},
                        [LinksRegex] VarChar(255) Not Null,
                        [LinksUrl] VarChar(255) Not Null,
                        [LinksUseGlobal] Bit default 1 Not Null,
                        UNIQUE ([Name] COLLATE NOCASE)
                    );

                    CREATE TABLE IF NOT EXISTS [Role] (
                        [Id] Char(36) PRIMARY KEY,
                        [Name] VarChar(255) Not Null UNIQUE,
                        [Description] VarChar(255) Null
                    );

                    CREATE TABLE IF NOT EXISTS [Team] (
                        [Id] Char(36) PRIMARY KEY,
                        [Name] VarChar(255) Not Null UNIQUE,
                        [Description] VarChar(255) Null
                    );

                    CREATE TABLE IF NOT EXISTS [User] (
                        [Id] Char(36) PRIMARY KEY,
                        [Name] VarChar(255) Not Null,
                        [Surname] VarChar(255) Not Null,
                        [Username] VarChar(255) Not Null UNIQUE,
                        [Password] VarChar(255) Not Null,
                        [PasswordSalt] VarChar(255) Not Null,
                        [Email] VarChar(255) Not Null
                    );

                    CREATE TABLE IF NOT EXISTS [TeamRepository_Permission] (
                        [Team_Id] Char(36) Not Null,
                        [Repository_Id] Char(36) Not Null,
                        Constraint [UNQ_TeamRepository_Permission_1] Unique ([Team_Id], [Repository_Id]),
                        Foreign Key ([Team_Id]) References [Team]([Id]),
                        Foreign Key ([Repository_Id]) References [Repository]([Id])
                    );

                    CREATE TABLE IF NOT EXISTS [UserRepository_Administrator] (
                        [User_Id] Char(36) Not Null,
                        [Repository_Id] Char(36) Not Null,
                        Constraint [UNQ_UserRepository_Administrator_1] Unique ([User_Id], [Repository_Id]),
                        Foreign Key ([User_Id]) References [User]([Id]),
                        Foreign Key ([Repository_Id]) References [Repository]([Id])
                    );

                    CREATE TABLE IF NOT EXISTS [UserRepository_Permission] (
                        [User_Id] Char(36) Not Null,
                        [Repository_Id] Char(36) Not Null,
                        Constraint [UNQ_UserRepository_Permission_1] Unique ([User_Id], [Repository_Id]),
                        Foreign Key ([User_Id]) References [User]([Id]),
                        Foreign Key ([Repository_Id]) References [Repository]([Id])
                    );

                    CREATE TABLE IF NOT EXISTS [UserRole_InRole] (
                        [User_Id] Char(36) Not Null,
                        [Role_Id] Char(36) Not Null,
                        Constraint [UNQ_UserRole_InRole_1] Unique ([User_Id], [Role_Id]),
                        Foreign Key ([User_Id]) References [User]([Id]),
                        Foreign Key ([Role_Id]) References [Role]([Id])
                    );

                    CREATE TABLE IF NOT EXISTS [UserTeam_Member] (
                        [User_Id] Char(36) Not Null,
                        [Team_Id] Char(36) Not Null,
                        Constraint [UNQ_UserTeam_Member_1] Unique ([User_Id], [Team_Id]),
                        Foreign Key ([User_Id]) References [User]([Id]),
                        Foreign Key ([Team_Id]) References [Team]([Id])
                    );
                    
                    CREATE TABLE IF NOT EXISTS [ServiceAccounts] (
                        [Id] nvarchar(36) PRIMARY KEY,
	                    [ServiceAccountName] nvarchar(36),
	                    [InPassManager]	Bit Default 0,
	                    [PassLastUpdated] Date Default 0,
	                    [RepositoryId] nvarchar(36),
                        Primary Key([Id] Autoincrement),
	                    Foreign Key([RepositoryId]) References [Repository]([Id])
                    );

                    CREATE TABLE IF NOT EXISTS [Dependencies] (
                        [Id] nvarchar(36) PRIMARY KEY,
                        [DateUpdated] VarChar(255),
                        [VersionInUse] VarChar(255),
                        [RepositoryId] nvarchar(36),
                        [KnownDependenciesId] nvarchar(36),
                        Foreign Key([RepositoryId]) References [Repository]([Id]),
						Foreign Key([KnownDependenciesId]) References [KnownDependencies]([Id])
                    );
					
                    CREATE TABLE IF NOT EXISTS [KnownDependencies] (
                        [Id] nvarchar(36) PRIMARY KEY,
                        [ComponentName] VarChar(255)
                    );

                    ", (int)RepositoryPushMode.Global);
            }
        }

        public string Precondition
        {
            get { return null; }
        }

        public void CodeAction(BonoboGitServerContext context) { }
    }
}