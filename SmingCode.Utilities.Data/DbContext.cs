using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace SmingCode.Utilities.Data;

internal class DbContext(
    TableValuedParameterFactory _tableValuedParameterFactory,
    ILogger<DbContext> _logger
) : IDbContext 
{
    public async Task CheckConnection()
    {
        using var connection = await GetConnection();
        connection.Open();
    }

    public async Task ExecuteNoResult(
        string storedProcedureName,
        object? parameters
    )
    {
        using var connection = await GetConnection();
        connection.Open();

        LogIfTrace(storedProcedureName, parameters, out var callId);
        await connection.ExecuteAsync(
            storedProcedureName,
            parameters,
            commandType: CommandType.StoredProcedure
        );
        LogSuccessIfTrace(callId);
    }

    public async Task<List<TModel>> ExecuteManyOrNoResult<TModel>(
        string storedProcedureName,
        object? parameters
    )
    {
        using var connection = await GetConnection();
        connection.Open();

        LogIfTrace(storedProcedureName, parameters, out var callId);
        var result = await connection.QueryAsync<TModel>(
            storedProcedureName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        var response = result.ToList();
        LogResultIfTrace(callId, response);

        return response;
    }

    public async Task<TResult> Execute<TResult>(
        Func<IDbConnection, Task<TResult>> dapperCommand
    )
    {
        using var connection = await GetConnection();
        connection.Open();

        var result = await dapperCommand(connection);

        return result;
    }

    public async Task<TModel> ExecuteSingleResult<TModel>(
        string storedProcedureName,
        object? parameters
    )
    {
        using var connection = await GetConnection();
        connection.Open();

        LogIfTrace(storedProcedureName, parameters, out var callId);
        var result = await connection.QuerySingleAsync<TModel>(
            storedProcedureName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        var response = result
            ?? throw new InvalidDataException("Expected to find a single result, but found none.");

        LogResultIfTrace(callId, response);
        return response;
    }

    public async Task<TModel?> ExecuteSingleOrNoResult<TModel>(
        string storedProcedureName,
        object? parameters
    )
    {
        using var connection = await GetConnection();
        connection.Open();

        LogIfTrace(storedProcedureName, parameters, out var callId);
        var result = await connection.QuerySingleOrDefaultAsync<TModel>(
            storedProcedureName,
            parameters,
            commandType: CommandType.StoredProcedure
        );

        if (result is null)
        {
            LogSuccessIfTrace(callId);
        }
        else
        {
            LogResultIfTrace(callId, result);
        }
        return result;
    }

    public async Task ExecuteWithMultiRowTvp<TTvp>(
        string storedProcedureName,
        object? standardParameters,
        string tableValuedParameterName,
        params TTvp[] entities
    ) where TTvp : ITableValuedParameter
    {
        var parameterSet = _tableValuedParameterFactory.GetParameterSetWithTvp(
            standardParameters,
            tableValuedParameterName,
            entities
        );

        using var connection = await GetConnection();
        connection.Open();

        LogIfTrace(storedProcedureName, standardParameters, tableValuedParameterName, entities, out var callId);
        await connection.ExecuteAsync(
            storedProcedureName,
            parameterSet,
            commandType: CommandType.StoredProcedure
        );

        LogSuccessIfTrace(callId);
    }

    public async Task<Guid> ExecuteWithSingleRowTvp<TTvp>(
        string storedProcedureName,
        object? standardParameters,
        string tableValuedParameterName,
        TTvp entity
    ) where TTvp : ITableValuedParameter
    {
        var parameterSet = _tableValuedParameterFactory.GetParameterSetWithTvp(
            standardParameters,
            tableValuedParameterName,
            entity
        );

        using var connection = await GetConnection();
        connection.Open();

        LogIfTrace(storedProcedureName, standardParameters, tableValuedParameterName, entity, out var callId);
        var result = await connection.QuerySingleAsync<Guid>(
            storedProcedureName,
            parameterSet,
            commandType: CommandType.StoredProcedure
        );

        LogResultIfTrace(callId, result);
        return result;
    }

    protected static async Task<IDbConnection> GetConnection()
        => await SqlConnectionProvider.CreateConnectionAsync();

    private void LogIfTrace(
        string storedProcedureName,
        object? standardParameters,
        out string? callId
    )
    {
        if (!_logger.IsEnabled(LogLevel.Trace))
        {
            callId = null;
            return;
        }

        callId = Guid.NewGuid().ToString();
        _logger.LogTrace(
            "[{CallId}] Calling stored procedure {StoredProcName} with parameters {StandardParameters} - {AllpayTraceType}.",
            callId,
            storedProcedureName,
            JsonSerializer.Serialize(standardParameters),
            "Sql Client"
        );
    }

    private void LogIfTrace(
        string storedProcedureName,
        object? standardParameters,
        string? tvpName,
        object? tvpParameter,
        out string? callId
    )
    {
        if (!_logger.IsEnabled(LogLevel.Trace))
        {
            callId = null;
            return;
        }

        callId = Guid.NewGuid().ToString();
        _logger.LogTrace(
            "[{CallId}] Calling stored procedure {StoredProcName} with parameters {StandardParameters} and table valued parameter {TvpName} with value {TvpParameter} - {AllpayTraceType}.",
            callId,
            storedProcedureName,
            JsonSerializer.Serialize(standardParameters),
            tvpName,
            JsonSerializer.Serialize(tvpParameter),
            "Sql Client"
        );
    }

    private void LogResultIfTrace(
        string? callId,
        object result
    )
    {
        if (_logger.IsEnabled(LogLevel.Trace))
        {
            _logger.LogTrace(
                "[{CallId}] Successfully retrieved results from database: {Results} - {AllpayTraceType}.",
                callId,
                JsonSerializer.Serialize(result),
                "Sql Client"
            );
        }
    }

    private void LogSuccessIfTrace(string? callId)
    {
        if (_logger.IsEnabled(LogLevel.Trace))
        {
            _logger.LogTrace(
                "[{CallId}] Call to database successful - {AllpayTraceType}.",
                callId,
                "Sql Client"
            );
        }
    }
}
