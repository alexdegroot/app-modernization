using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MutationProcessor
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IQueueReader _reader;
        private readonly IDatabaseWriter _writer;

        public Worker(ILogger<Worker> logger, IQueueReader reader, IDatabaseWriter writer)
        {
            _logger = logger;
            _reader = reader;
            _writer = writer;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                if (!await _reader.Ensure(stoppingToken))
                {
                    _logger.LogError("Cannot ensure that Azure storage queue exists.");
                    return;
                }

                if (!await _writer.Verify(stoppingToken))
                {
                    _logger.LogError("Cannot verify MongoDB database connection.");
                    return;
                }

                while (!stoppingToken.IsCancellationRequested)
                {
                    var messages = _reader.GetChanges(stoppingToken);

                    await foreach (var message in messages.WithCancellation(stoppingToken))
                    {
                        var change = message.Change;
                        _logger.LogInformation("a1. Process Mutation with Entity ID: {entityId}; MutationId: {mutationId}", change.EntityId, change.MutationId);
                        if (await _writer.Append(change, stoppingToken))
                        {
                            _logger.LogInformation("a2. Successful processed Entity ID: {entityId}; MutationId: {mutationId}",
                                change.EntityId, change.MutationId);
                            await _reader.DeleteMessage(message, stoppingToken);
                        }
                        else
                        {
                            _logger.LogWarning("a2. Cannot process Entity ID: {entityId}; MutationId: {mutationId}", change.EntityId, change.MutationId);
                        }
                    }

                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    await Task.Delay(100, stoppingToken);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.Message);
                throw;
            }
        }
    }
}