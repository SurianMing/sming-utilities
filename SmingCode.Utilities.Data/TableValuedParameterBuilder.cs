using System.Linq.Expressions;

namespace SmingCode.Utilities.Data;

internal interface ITableValuedParameterBuilder { }

internal class TableValuedParameterBuilder<TTvp> : ITableValuedParameterBuilder
    where TTvp : ITableValuedParameter
{
    private readonly string _tvpTypeName = typeof(TTvp).Name;
    private readonly DataTable _dataTable;
    private readonly Func<TTvp, object[]> _dataRowBuilder;

    public TableValuedParameterBuilder()
    {
        var type = typeof(TTvp);
        var properties = type
            .GetProperties(
                BindingFlags.NonPublic |
                BindingFlags.Instance |
                BindingFlags.Public
            )
            .Where(propInfo =>
                propInfo.Name != "Id"
                && propInfo.Name != "TableName"
                && propInfo.Name != "Created"
                && propInfo.Name != "Deleted"
            )
            .ToList();      
        
        _dataTable = new DataTable
        {
            TableName = type.FullName
        };

        foreach (var property in properties)
        {
            _dataTable.Columns.Add(
                new DataColumn(
                    property.Name,
                    Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType
                )
            );
        }

        var input = Expression.Parameter(type, "tvp");
        var output = Expression.Parameter(typeof(object[]));
        var propMappers = properties
            .Select(prop =>
            {
                var map = Expression.Property(input, prop.Name);
                var conv = Expression.Convert(map, typeof(object));

                return conv;
            })
            .ToArray();

        var arrayBuilder = Expression.NewArrayInit(
            typeof(object),
            propMappers
        );

        _dataRowBuilder = Expression.Lambda<Func<TTvp, object[]>>(arrayBuilder, input).Compile();
    }

    public KeyValuePair<string, object?> BuildTvp(
        string parameterName,
        IEnumerable<TTvp> rows
    )
    {
        var dataTable = _dataTable.Clone();

        var dataRows = rows
            .Select(row => _dataRowBuilder(row));

        foreach (var dataRow in dataRows)
        {
            dataTable.Rows.Add(dataRow);
        }

        return new(parameterName, dataTable.AsTableValuedParameter(_tvpTypeName));
    }
}