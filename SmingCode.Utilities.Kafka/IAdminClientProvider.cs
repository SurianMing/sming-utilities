namespace SmingCode.Utilities.Kafka;

internal interface IAdminClientProvider
{
    IAdminClient GetAdminClient();
}
