namespace SmingCode.Utilities.Data;

public interface ICrudHandler<TExtendedEntity, TEntity, TKey>
    where TExtendedEntity : TEntity, IEntity<TKey>
    where TEntity : IEntity
    where TKey : notnull
{
    Task<List<TExtendedEntity>> GetAllEntities();
    Task<TExtendedEntity> GetEntityById(TKey id);
    Task SaveNewEntities(TEntity[] newEntities);
    Task<TKey> SaveNewEntity(TEntity newEntity);
    Task SoftDeleteEntityById(TKey id);
    Task HardDeleteEntityById(TKey id);
    Task UpdateEntityMatchedById(TKey id, TEntity updatedEntity);
}
