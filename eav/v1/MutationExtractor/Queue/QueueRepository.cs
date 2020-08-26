using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MutationExtractor.Queue
{
    public class QueueRepository : IQueueRepository
    {
        private readonly ILogger<QueueRepository> _logger;
        private ContainerQueueClient _client;

        public QueueRepository(IOptions<Configuration> config, ILogger<QueueRepository> logger)
        {
            _logger = logger;
            var connectionString = config.Value.Queue_Connectionstring ?? throw new ArgumentException(nameof(config.Value.Queue_Connectionstring));
            var queueName = config.Value.Queue_Name ?? throw new ArgumentException(nameof(config.Value.Queue_Name));
            _client = new ContainerQueueClient(connectionString, queueName);
        }

        public async Task<bool> Ensure(CancellationToken cancellationToken)
        {
            try
            {
                var result = await _client.CreateIfNotExistsAsync(cancellationToken: cancellationToken);
                if (result == null || result.Status == 201 || result.Status == 204)
                {
                    return true;
                }
            }
            catch (RequestFailedException e)
            {
                _logger.LogError("Couldn't connect to queue.", e);
            }
            return false;
        }

        public async Task AddMessage<T>(T message, CancellationToken cancellationToken)
        {
            var json = JsonSerializer.Serialize(message);
            await _client.SendMessageAsync(json, cancellationToken);
        }
    }
}