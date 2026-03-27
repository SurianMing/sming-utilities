namespace SmingCode.Utilities.Data;

internal static class EnvironmentVariableProvider
{
    internal static string GetEnvironmentVariableOrEmpty(string name)
        => Environment.GetEnvironmentVariable(name)
            ?? string.Empty;
}