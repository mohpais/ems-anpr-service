using RabbitMQ.Client;

namespace Microsoft.Lonsum.Services.ANPR.Infrastructure.Common.BuildingBlocks
{
    public interface IRabbitMQPersistentConnection : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();
    }
}
