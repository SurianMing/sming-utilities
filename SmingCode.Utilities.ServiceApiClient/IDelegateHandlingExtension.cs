namespace SmingCode.Utilities.ServiceApiClient;

public interface IDelegateHandlingExtension
{
    HttpRequestMessage Handle(
        HttpRequestMessage requestMessage
    );
}