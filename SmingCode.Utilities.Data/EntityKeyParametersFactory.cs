namespace SmingCode.Utilities.Data;

internal class EntityKeyParametersFactory(List<Type> tvpTypes)
{
    private readonly Dictionary<Type, IEntity> _keyParametersBuilders = tvpTypes
        .Where(type => type.GetInterface(typeof(IEntity<>).Name) is not null)
        .ToDictionary(
            type => type,
            type => (IEntity)Activator.CreateInstance(type)!
        );

    public object GetKeyParameters<TEntity, TKey>(
        TKey key
    ) where TEntity : IEntity<TKey>
      where TKey : notnull
    {
        var keyType = typeof(TEntity);
        if (!_keyParametersBuilders.TryGetValue(keyType, out var keyParametersBuilder))
        {
            throw new NotSupportedException($"No key parameters builder found for type {keyType.Name}");
        }

        var tvp = (keyParametersBuilder as IEntity<TKey>)!
            .GetKeyParameters(
                key
            );

        return tvp;
    }
}