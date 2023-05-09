using System;

namespace Bonobo.Git.Server.Data
{
    public class Dependency
    {
        public Guid Id { get; set; }
        public DateTime? DateUpdated { get; set; }
        public string VersionInUse { get; set; }
        
        public Guid RepositoryId { get; set; }
        public virtual Repository Repository { get; set; }

        public Guid KnownDependenciesId { get; set; }
        public virtual KnownDependency KnownDependency { get; set; }
    }
}