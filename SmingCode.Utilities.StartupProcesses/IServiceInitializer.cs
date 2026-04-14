namespace SmingCode.Utilities.StartupProcesses;

public interface IServiceInitializer
{
    Delegate ServiceInitializer { get; }
}