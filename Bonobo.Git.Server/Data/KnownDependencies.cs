using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Bonobo.Git.Server.Data
{ 
    public partial class KnownDependencies
{
        
    public int Id { get; set; }
    public string ComponentName { get; set; }
    public virtual Bonobo.Git.Server.Data.Dependencies Dependencies { get; set; }
    //[Key, ForeignKey("DependenciesId")]
    public Guid DependenciesId { get; set; }
}
}