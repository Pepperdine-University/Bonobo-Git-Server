using System;
using System.Data.Entity.ModelConfiguration;

namespace Bonobo.Git.Server.Data.Mapping
{
    public class DependencyMap : EntityTypeConfiguration<Dependency>
    {
        public DependencyMap()
        {
            SetPrimaryKey();
            SetRelationships();
        }

        private void SetRelationships()
        {
            HasRequired(d => d.Repository)
                .WithMany(r => r.Dependencies)
                .HasForeignKey(d => d.RepositoryId);

            HasRequired(d => d.KnownDependency)
                .WithMany(k => k.Dependencies)
                .HasForeignKey(d => d.KnownDependenciesId);
            
        }



        private void SetPrimaryKey()
        {
            HasKey(t => t.Id);
        }
    }
}
