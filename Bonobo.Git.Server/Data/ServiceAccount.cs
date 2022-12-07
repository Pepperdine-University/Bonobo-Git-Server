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
        public string PassLastUpdated { get; set; }
        public int Id { get; set; } = -1;
        public Guid RepositoryId { get; set; }
        public string UserId { get; set;}
        public virtual Repository Repository { get; set; }
    }
}