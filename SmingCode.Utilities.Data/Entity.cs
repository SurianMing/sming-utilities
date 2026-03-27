namespace SmingCode.Utilities.Data;

public interface IEntity : ITableValuedParameter
{
    string TableName { get; }
}

public interface IEntity<TKey> : IEntity
    where TKey : notnull
{
    TKey Id { get; }
    object GetKeyParameters(TKey key);
}