using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bonobo.Git.Server.Data
{
    public partial class KnownDependency
    {
        private ICollection<Dependency> _dependencies;

        public Guid Id { get; set; }
        public string ComponentName { get; set; }

        public virtual ICollection<Dependency> Dependencies
        {
            get
            {
                return _dependencies ?? (_dependencies = new List<Dependency>());
            }
            set
            {
                _dependencies = value;
            }
        }

    }
}