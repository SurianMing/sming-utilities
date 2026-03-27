namespace SmingCode.Utilities.Data;

public interface IDbContext
{
    Task CheckConnection();
    Task ExecuteNoResult(string storedProcedureName, object? parameters);
    Task<List<TModel>> ExecuteManyOrNoResult<TModel>(
        string storedProcedureName, object? parameters
    );
    Task<TResult> Execute<TResult>(
        Func<IDbConnection, Task<TResult>> dapperCommand
    );
    Task<TModel> ExecuteSingleResult<TModel>(
        string storedProcedureName,
        object? parameters
    );
    Task<TModel?> ExecuteSingleOrNoResult<TModel>(
        string storedProcedureName,
        object? parameters
    );
    Task ExecuteWithMultiRowTvp<TTvp>(
        string storedProcedureName,
        object? standardParameters,
        string tableValuedParameterName,
        params TTvp[] entities
    ) where TTvp : ITableValuedParameter;
    Task<Guid> ExecuteWithSingleRowTvp<TTvp>(
        string storedProcedureName,
        object? standardParameters,
        string tableValuedParameterName,
        TTvp entity
    ) where TTvp : ITableValuedParameter;
}