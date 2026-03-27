namespace SmingCode.Utilities.Data;

public interface IExtendedEntity
{
    DateTime Created { get; set; }
    DateTime LastUpdated { get; set; }
    bool Deleted { get; set; }
}

public abstract class ExtendedEntity<TKey> : IEntity<TKey>, IExtendedEntity
    where TKey : notnull
{
    public abstract string TableName { get; }
    public abstract TKey Id { get; }
    public DateTime Created { get; set; }
    public DateTime LastUpdated { get; set; } = DateTime.UtcNow;
    public bool Deleted { get; set; }
    public abstract object GetKeyParameters(TKey key);
}