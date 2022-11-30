using System;
using System.ComponentModel.DataAnnotations.Schema;
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
            HasRequired<Repository>(d => d.Repository)
                .WithMany(r => r.Dependencies)
                .HasForeignKey<Guid>(d => d.RepositoryId);

            HasRequired<KnownDependency>(d => d.KnownDependency)
                .WithMany(k => k.Dependencies)
                .HasForeignKey<Guid>(d => d.KnownDependenciesId);
            
        }



        private void SetPrimaryKey()
        {
            HasKey(t => t.Id);
        }
    }
}
