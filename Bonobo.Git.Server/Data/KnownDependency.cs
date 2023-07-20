using System;
using System.Collections.Generic;

namespace Bonobo.Git.Server.Data
{
    public class KnownDependency
    {
        private ICollection<Dependency> _dependencies;

        public Guid Id { get; set; }
        public string ComponentName { get; set; }

        public KnownDependency() { }

        public KnownDependency(string componentName)
        {
            Id = Guid.NewGuid();
            ComponentName = componentName;
        }

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