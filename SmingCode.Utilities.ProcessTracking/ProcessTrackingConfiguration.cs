namespace SmingCode.Utilities.ProcessTracking;

public class ProcessTrackingConfiguration
{
    public string ServiceInstanceId { get; set; } = Guid.NewGuid().ToString();
    public required string ServiceName { get; set; }

    internal string ActivityName => $"{ServiceName}.{ServiceInstanceId}";
}
