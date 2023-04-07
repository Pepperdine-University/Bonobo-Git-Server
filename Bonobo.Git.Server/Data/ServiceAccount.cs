using Bonobo.Git.Server.App_GlobalResources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.AccessControl;
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

    public partial class ServiceAccountWithRepoName
    {
        [Display(ResourceType = typeof(Resources), Name = "ServiceAccount_Repository_Name")]
        public string RepositoryName { get; set; }
        [Display(ResourceType = typeof(Resources), Name = "ServiceAccount_Name")]
        public string ServiceAccountName { get; set; }
        [Display(ResourceType = typeof(Resources), Name = "ServiceAccount_InLastPass")]
        public bool InPassManager { get; set; }
        public DateTime? PassLastUpdated { get; set; }
        public Guid Id { get; set; } = Guid.Parse("94cab580-0018-4a00-9bb4-bed27234ff22");
        public Guid RepositoryId { get; set; }
        public virtual Repository Repository { get; set; }
    }
}