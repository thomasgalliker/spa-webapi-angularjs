using System.Data.Entity.ModelConfiguration;

using HomeCinema.Entities;

namespace HomeCinema.Data.Configurations
{
    public abstract class EntityBaseConfiguration<T> : EntityTypeConfiguration<T>
        where T : class, IEntityBase
    {
        protected EntityBaseConfiguration()
        {
            this.HasKey(e => e.ID);
        }
    }
}