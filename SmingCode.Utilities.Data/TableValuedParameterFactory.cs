namespace SmingCode.Utilities.Data;

internal class TableValuedParameterFactory(List<Type> tvpTypes)
{
    private readonly Dictionary<Type, ITableValuedParameterBuilder> _tvpBuilders = tvpTypes
            .ToDictionary(
                type => type,
                type => (ITableValuedParameterBuilder)Activator.CreateInstance(
                    typeof(TableValuedParameterBuilder<>).MakeGenericType(type)
                )!
            );

    internal KeyValuePair<string, object?> GetTvpParameter<TTvp>(
        string parameterName,
        params TTvp[] entities
    ) where TTvp : ITableValuedParameter
    {
        var tvpType = typeof(TTvp);
        if (!_tvpBuilders.TryGetValue(tvpType, out var tvpBuilder))
        {
            throw new NotSupportedException($"No table valued parameter builder found for type {tvpType.Name}");
        }

        var tvp = (tvpBuilder as TableValuedParameterBuilder<TTvp>)!
            .BuildTvp(
                parameterName,
                entities
            );
            
        return tvp;
    }

    internal Dictionary<string, object?> GetParameterSetWithTvp<TTvp>(
        object? standardParameters,
        string tableValuedParameterName,
        params TTvp[] entities
    ) where TTvp : ITableValuedParameter
    {
        var parameterSet = standardParameters is null
            ? []
            : standardParameters.ToDictionary();

        var tvpParameter = GetTvpParameter(
            tableValuedParameterName,
            entities
        );
        parameterSet.Add(
            tvpParameter.Key, tvpParameter.Value
        );

        return parameterSet;
    }
}