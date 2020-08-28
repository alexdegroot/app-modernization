using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MutationProcessor.Queue
{
    public class QueueReader : IQueueReader
    {
        private ILogger<QueueReader> _logger;
        private ContainerQueueClient _client;

        public QueueReader(IOptions<Configuration> config, ILogger<QueueReader> logger)
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

        public IAsyncEnumerable<Change> GetChanges(CancellationToken stoppingToken)
        {
            return new []
            {
                new Change
                {
                    TenantId = 1000,
                    EntityId = 400,
                    TemplateId = 19,
                    MutationId = 1,
                    FieldId = 12,
                    Value = "yolo"
                },
                new Change
                {
                    TenantId = 1000,
                    EntityId = 400,
                    TemplateId = 19,
                    MutationId = 2,
                    FieldId = 12,
                    Value = "yolo2"
                },                
                new Change
                {
                    TenantId = 1000,
                    EntityId = 400,
                    TemplateId = 19,
                    MutationId = 1,
                    FieldId = 12,
                    Value = "updated value",
                    IsDeleted = true
                },                 
            }.ToAsyncEnumerable();
        }
    }
}