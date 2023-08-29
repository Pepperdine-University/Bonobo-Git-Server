using System;

namespace Bonobo.Git.Server.Data.Update.SqlServer
{
    public class InitialCreateScript : IUpdateScript
    {
        public string Command
        {
            get
            {
                // If you modify this scheme make sure to introduce an unit test for the new scheme.
                return string.Format(@"
                   IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'Repository'))
                    BEGIN
                        CREATE TABLE [dbo].[Repository] (
                            [Id] uniqueidentifier NOT NULL,
                            [Name] nvarchar(255) NOT NULL UNIQUE,
                            [Description] nvarchar(255) NULL,
                            [Anonymous] bit NOT NULL,
                            [AllowAnonymousPush] integer DEFAULT 0 NOT NULL,
                            [LinksRegex] nvarchar(255) NOT NULL,
                            [LinksUrl] nvarchar(255) NOT NULL,
                            [LinksUseGlobal] bit DEFAULT 1 NOT NULL,
                            CONSTRAINT [PK_Repository] PRIMARY KEY ([Id])
                        );
                    END

                    IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'Role'))
                    BEGIN
                        CREATE TABLE [dbo].[Role] (
                            [Id] uniqueidentifier,
                            [Name] nvarchar(255) NOT NULL,
                            [Description] nvarchar(255) NULL,
                            CONSTRAINT [PK_Role] PRIMARY KEY ([Id])
                        );
                    END

                    IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'Team'))
                    BEGIN
                        CREATE TABLE [dbo].[Team] (
                            [Id] uniqueidentifier,
                            [Name] nvarchar(255) NOT NULL UNIQUE,
                            [Description] nvarchar(255) NULL,
                            CONSTRAINT [PK_Team] PRIMARY KEY ([Id])
                        );
                    END

                    IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'User'))
                    BEGIN
                        CREATE TABLE [dbo].[User] (
                            [Id] uniqueidentifier,
                            [Name] nvarchar(255) NOT NULL,
                            [Surname] nvarchar(255) NOT NULL,
                            [Username] nvarchar(255) NOT NULL,
                            [Password] nvarchar(255) NOT NULL,
                            [PasswordSalt] nvarchar(255) NOT NULL,
                            [Email] nvarchar(255) NOT NULL,
                            CONSTRAINT [PK_User] PRIMARY KEY ([Id])
                        );
                    END

                    IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'TeamRepository_Permission'))
                    BEGIN
                        CREATE TABLE [dbo].[TeamRepository_Permission] (
                            [Team_Id] uniqueidentifier NOT NULL,
                            [Repository_Id] uniqueidentifier NOT NULL,
                            CONSTRAINT [UNQ_TeamRepository_Permission_1] UNIQUE ([Team_Id], [Repository_Id]),
                            FOREIGN KEY ([Team_Id]) REFERENCES [Team]([Id]),
                            FOREIGN KEY ([Repository_Id]) REFERENCES [Repository]([Id])
                        );
                    END

                    IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'UserRepository_Administrator'))
                    BEGIN
                        CREATE TABLE [dbo].[UserRepository_Administrator] (
                            [User_Id] uniqueidentifier NOT NULL,
                            [Repository_Id] uniqueidentifier NOT NULL,
                            CONSTRAINT [UNQ_UserRepository_Administrator_1] UNIQUE ([User_Id], [Repository_Id]),
                            FOREIGN KEY ([User_Id]) REFERENCES [User]([Id]),
                            FOREIGN KEY ([Repository_Id]) REFERENCES [Repository]([Id])
                        );
                    END

                    IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'UserRepository_Permission'))
                    BEGIN
                        CREATE TABLE [dbo].[UserRepository_Permission] (
                            [User_Id] uniqueidentifier NOT NULL,
                            [Repository_Id] uniqueidentifier NOT NULL,
                            CONSTRAINT [UNQ_UserRepository_Permission_1] UNIQUE ([User_Id], [Repository_Id]),
                            FOREIGN KEY ([User_Id]) REFERENCES [User]([Id]),
                            FOREIGN KEY ([Repository_Id]) REFERENCES [Repository]([Id])
                        );
                    END

                    IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'UserRole_InRole'))
                    BEGIN
                        CREATE TABLE [dbo].[UserRole_InRole] (
                            [User_Id] uniqueidentifier NOT NULL,
                            [Role_Id] uniqueidentifier NOT NULL,
                            CONSTRAINT [UNQ_UserRole_InRole_1] UNIQUE ([User_Id], [Role_Id]),
                            FOREIGN KEY ([User_Id]) REFERENCES [User]([Id]),
                            FOREIGN KEY ([Role_Id]) REFERENCES [Role]([Id])
                        );
                    END

                    IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'UserTeam_Member'))
                    BEGIN
                        CREATE TABLE [dbo].[UserTeam_Member] (
                            [User_Id] uniqueidentifier NOT NULL,
                            [Team_Id] uniqueidentifier NOT NULL,
                            CONSTRAINT [UNQ_UserTeam_Member_1] UNIQUE ([User_Id], [Team_Id]),
                            FOREIGN KEY ([User_Id]) REFERENCES [User]([Id]),
                            FOREIGN KEY ([Team_Id]) REFERENCES [Team]([Id])
                        );
                    END

                    IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'ServiceAccounts'))
                    BEGIN
                        CREATE TABLE [dbo].[ServiceAccounts] (
                            [Id] uniqueidentifier NOT NULL,
                            [ServiceAccountName] nvarchar(36) NOT NULL,
                            [InPassManager] bit DEFAULT 0,
                            [PassLastUpdated] date DEFAULT CURRENT_TIMESTAMP,
                            [RepositoryId] uniqueidentifier,
                            CONSTRAINT [PK_ServiceAccounts] PRIMARY KEY ([Id]),
                            FOREIGN KEY ([RepositoryId]) REFERENCES [Repository]([Id])
                        );
                    END

                    IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'KnownDependencies'))
                    BEGIN
                        CREATE TABLE [dbo].[KnownDependencies] (
                            [Id] uniqueidentifier NOT NULL,
                            [ComponentName] nvarchar(255),
                            CONSTRAINT [PK_KnownDependencies] PRIMARY KEY ([Id])
                        );
                    END

                    IF (NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND  TABLE_NAME = 'Dependencies'))
                    BEGIN
                        CREATE TABLE [dbo].[Dependencies] (
                            [Id] uniqueidentifier NOT NULL,
                            [DateUpdated] date DEFAULT CURRENT_TIMESTAMP,
                            [VersionInUse] nvarchar(255),
                            [RepositoryId] uniqueidentifier,
                            [KnownDependenciesId] uniqueidentifier,
                            CONSTRAINT [PK_Dependencies] PRIMARY KEY ([Id]),
                            FOREIGN KEY ([RepositoryId]) REFERENCES [Repository]([Id]),
                            FOREIGN KEY ([KnownDependenciesId]) REFERENCES [KnownDependencies]([Id])
                        );
                    END

                   
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
