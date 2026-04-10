namespace SmingCode.Utilities.ServiceMetadata;

public record Metadata(
    string ServiceName,
    Guid ServiceInstanceId
)
{
    public string FullServiceDescriptor => $"{ServiceName}.{ServiceInstanceId}";
};

public static class MetadataExtensions
{
    public static Dictionary<string, object> GetCustomDimensions(
        this Metadata metadata
    ) => new()
    {
        { "service-name", metadata.ServiceName },
        { "service-instance-id", metadata.ServiceInstanceId }
    };
}