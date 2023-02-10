using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bonobo.Git.Server.Data
{
    public partial class Dependency
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