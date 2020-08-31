using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Azure;
using Azure.Storage.Queues.Models;
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

        public async IAsyncEnumerable<Message> GetChanges([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var messages = await GetMessages(cancellationToken);
            
            foreach (var message in messages)
            {
                Change change;
                try
                {
                    var messageText = message.MessageText;
                    change = JsonSerializer.Deserialize<Change>(messageText);
                    
                }
                catch (JsonException e)
                {
                    Console.WriteLine(e.Message);
                    continue;
                }
                yield return new Message(message.MessageId, message.PopReceipt, change);
            }
        }

        private async Task<IEnumerable<QueueMessage>> GetMessages(CancellationToken cancellationToken)
        {
            try
            {
                var response = await _client.ReceiveMessagesAsync(10, cancellationToken: cancellationToken);
                return response.Value;
            }
            catch (Exception e)
            {
                _logger.LogError("Couldn't receive messages from the queue", e);
            }

            return Enumerable.Empty<QueueMessage>();
        }

        public async Task DeleteMessage(Message message, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Removing message with id: {message.MessageId}");
            await _client.DeleteMessageAsync(message.MessageId, message.PopReceipt, cancellationToken);
        }
    }
}