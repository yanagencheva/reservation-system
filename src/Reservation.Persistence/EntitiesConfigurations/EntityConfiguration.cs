using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reservation.Persistence.Entities;

namespace Reservation.Persistence.EntitiesConfigurations;

public abstract class EntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity>
    where TEntity : Entity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasKey(x => x.Id);

        ConfigureEntity(builder);
    }

    /// <summary>
    /// Override this method to configure entity-specific properties
    /// </summary>
    protected abstract void ConfigureEntity(EntityTypeBuilder<TEntity> builder);
}
