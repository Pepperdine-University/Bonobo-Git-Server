using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bonobo.Git.Server.Data
{
    public partial class Dependencies
    {
        public string Id { get; set; }
        public string DateUpdated { get; set; }
        public string VersionInUse { get; set; }
        [Required]
        public virtual Bonobo.Git.Server.Data.Repository Repository { get; set; }
        //[Key, ForeignKey("RepositoryId")]
        public Guid RepositoryId { get; set; }
        [Required]
        public virtual Bonobo.Git.Server.Data.KnownDependencies KnownDependencies { get; set; }
        //[Key, ForeignKey("KnownDependenciesId")]
        public string KnownDependencies_Id { get; set; }
    }
}