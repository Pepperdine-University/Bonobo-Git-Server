using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bonobo.Git.Server.Models
{
    public class ThirdPartyModel
    {
        public string ComponentName { get; set; }
        public string VersionInUse { get; set; }
        public string DateUpdated { get; set; }

        //Navigation properties
        //Trying to grab the Repo ID
        public int? Id { get; set; }
        //Creating the Primary key
        public RepositoryDetailModel RepositoryDetail { get; set; }
    }
}