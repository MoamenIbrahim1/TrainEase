namespace TrainEase.Infrastructure.Presistance._Data.Configurations.Base
{
    public class BaseEntityConfiguration<TEntity, TKey> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity<>
    {
    }
}
