using Microsoft.Extensions.Logging;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using RabbitMQ.Client;
using System.Net.Sockets;
using Polly;
using System.Runtime.CompilerServices;

namespace Microsoft.Lonsum.Services.ANPR.Infrastructure.Common.BuildingBlocks
{
    public class DefaultRabbitMQPersistentConnection : IRabbitMQPersistentConnection, IDisposable
    {
        private readonly IConnectionFactory _connectionFactory;

        private readonly ILogger<DefaultRabbitMQPersistentConnection> _logger;

        private readonly int _retryCount;

        private IConnection _connection;

        private bool _disposed;

        private object sync_root = new object();

        public bool IsConnected
        {
            get
            {
                if (_connection != null && _connection.IsOpen)
                {
                    return !_disposed;
                }

                return false;
            }
        }

        public DefaultRabbitMQPersistentConnection(IConnectionFactory connectionFactory, ILogger<DefaultRabbitMQPersistentConnection> logger, int retryCount = 5)
        {
            _connectionFactory = connectionFactory ?? throw new ArgumentNullException("connectionFactory");
            _logger = logger ?? throw new ArgumentNullException("logger");
            _retryCount = retryCount;
        }

        public IModel CreateModel()
        {
            if (!IsConnected)
            {
                throw new InvalidOperationException("No RabbitMQ connections are available to perform this action");
            }

            return _connection.CreateModel();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                try
                {
                    _connection.ConnectionShutdown -= OnConnectionShutdown;
                    _connection.CallbackException -= OnCallbackException;
                    _connection.ConnectionBlocked -= OnConnectionBlocked;
                    _connection.Dispose();
                }
                catch (IOException ex)
                {
                    _logger.LogCritical(ex.ToString());
                }
            }
        }

        public bool TryConnect()
        {
            _logger.LogInformation("RabbitMQ Client is trying to connect");
            lock (sync_root)
            {
                Policy.Handle<SocketException>().Or<BrokerUnreachableException>().WaitAndRetry(_retryCount, (int retryAttempt) => TimeSpan.FromSeconds(Math.Pow(2.0, retryAttempt)), delegate (Exception ex, TimeSpan time)
                {
                    ILogger<DefaultRabbitMQPersistentConnection> logger = _logger;
                    object[] array = new object[2];
                    DefaultInterpolatedStringHandler defaultInterpolatedStringHandler = new DefaultInterpolatedStringHandler(0, 1);
                    defaultInterpolatedStringHandler.AppendFormatted(time.TotalSeconds, "n1");
                    array[0] = defaultInterpolatedStringHandler.ToStringAndClear();
                    array[1] = ex.Message;
                    logger.LogWarning(ex, "RabbitMQ Client could not connect after {TimeOut}s ({ExceptionMessage})", array);
                })
                    .Execute(delegate
                    {
                        _connection = _connectionFactory.CreateConnection();
                    });
                if (IsConnected)
                {
                    _connection.ConnectionShutdown += OnConnectionShutdown;
                    _connection.CallbackException += OnCallbackException;
                    _connection.ConnectionBlocked += OnConnectionBlocked;
                    _logger.LogInformation("RabbitMQ Client acquired a persistent connection to '{HostName}' and is subscribed to failure events", _connection.Endpoint.HostName);
                    return true;
                }

                _logger.LogCritical("FATAL ERROR: RabbitMQ connections could not be created and opened");
                return false;
            }
        }

        private void OnConnectionBlocked(object sender, ConnectionBlockedEventArgs e)
        {
            if (!_disposed)
            {
                _logger.LogWarning("A RabbitMQ connection is shutdown. Trying to re-connect...");
                TryConnect();
            }
        }

        private void OnCallbackException(object sender, CallbackExceptionEventArgs e)
        {
            if (!_disposed)
            {
                _logger.LogWarning("A RabbitMQ connection throw exception. Trying to re-connect...");
                TryConnect();
            }
        }

        private void OnConnectionShutdown(object sender, ShutdownEventArgs reason)
        {
            if (!_disposed)
            {
                _logger.LogWarning("A RabbitMQ connection is on shutdown. Trying to re-connect...");
                TryConnect();
            }
        }
    }
}
