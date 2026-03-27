namespace SmingCode.Utilities.Data;

internal static class SqlConnectionProvider
{
    private static readonly string _connectionString = EnvironmentVariableProvider.GetEnvironmentVariableOrEmpty("SqlConnection");
    private static readonly bool _useAccessToken =
            bool.TryParse(
                EnvironmentVariableProvider.GetEnvironmentVariableOrEmpty("UseSqlAccessToken"),
                out var useAccessToken
            ) && useAccessToken;

    public static async Task<IDbConnection> CreateConnectionAsync()
    {
        var connection = new SqlConnection(_connectionString)
            ?? throw new InvalidOperationException("Unable to create sql connection");

        if (_useAccessToken)
        {
            var managedIdentityClientId = EnvironmentVariableProvider.GetEnvironmentVariableOrEmpty("Management_Identity_ClientId");

            connection.AccessToken = await AzureAccessTokenProvider.GetTokenForManagedIdentityAsync(managedIdentityClientId);
        }

        return connection;
    }
}