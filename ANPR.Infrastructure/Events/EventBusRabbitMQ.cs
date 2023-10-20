using Microsoft.Lonsum.Services.ANPR.Infrastructure.Common.BuildingBlocks;
using Microsoft.Lonsum.Services.ANPR.Application.Events;
using RabbitMQ.Client;
using System.Text;

namespace Microsoft.Lonsum.Services.ANPR.Infrastructure.Events
{
    public class EventBusRabbitMQ : IEventBus, IDisposable
    {
        private readonly IRabbitMQPersistentConnection _persistentConnection;
        private readonly IModel _channel;
        private string _queueName;
        public EventBusRabbitMQ(IRabbitMQPersistentConnection persistentConnection, string queueName)
        {
            _persistentConnection = persistentConnection ?? throw new ArgumentNullException(nameof(persistentConnection));
            _queueName = queueName;
            _channel = CreateConsumerChannel();
        }

        public void Dispose()
        {
            if (_channel != null)
            {
                _channel.Dispose();
            }
        }

        public void Publish(string message)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var messageBytes = Encoding.UTF8.GetBytes(message);
            try
            {
                _channel.BasicPublish(exchange: string.Empty,
                                     routingKey: _queueName,
                                     basicProperties: null,
                                     body: messageBytes);
            }
            catch (Exception ex)
            {
                _channel?.Dispose();
            }
        }

        public void Exchange(string message)
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            var messageBytes = Encoding.UTF8.GetBytes(message);
            try
            {
                _channel.BasicPublish(exchange: _queueName,
                                     routingKey: string.Empty,
                                     basicProperties: null,
                                     body: messageBytes);
            }
            catch (Exception ex)
            {
                _channel?.Dispose();
            }
        }

        private IModel CreateConsumerChannel()
        {
            if (!_persistentConnection.IsConnected)
            {
                _persistentConnection.TryConnect();
            }

            IModel model = _persistentConnection.CreateModel();
            model.ExchangeDeclare(_queueName, ExchangeType.Fanout);
            model.QueueDeclare(_queueName, durable: true, exclusive: false, autoDelete: false, null);

            return model;
        }
    }
}
