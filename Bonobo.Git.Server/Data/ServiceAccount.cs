using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bonobo.Git.Server.Data
{
    public partial class ServiceAccount
    {

        public string ServiceAccountName { get; set; }
        public bool InPassManager { get; set; }
        public DateTime? PassLastUpdated { get; set; }
        public Guid Id { get; set; } = Guid.Parse("94cab580-0018-4a00-9bb4-bed27234ff22");
        public Guid RepositoryId { get; set; }
        public virtual Repository Repository { get; set; }
    }
}