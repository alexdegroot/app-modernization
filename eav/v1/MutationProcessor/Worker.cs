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
                if(!await _reader.Ensure(stoppingToken))
                {
                    _logger.LogError("Couldn't ensure that Queue is there.");
                    return;
                }

                if (!await _writer.Verify(stoppingToken))
                {
                    _logger.LogError("Couldn't verify database connection.");
                    return;
                }
                
                while (!stoppingToken.IsCancellationRequested)
                {
                    var messages = _reader.GetChanges(stoppingToken).ConfigureAwait(false);

                    await foreach (var message in messages.WithCancellation(stoppingToken))
                    {
                        var change = message.Change;
                        _logger.LogInformation("a1. Process Mutation with Entity ID: {entityId}; MutationId: {mutationId}", change.EntityId, change.MutationId);
                        if (await _writer.Append(change, stoppingToken).ConfigureAwait(false))
                        {
                            _logger.LogInformation("a2. Successful processed Entity ID: {entityId}; MutationId: {mutationId}",
                                change.EntityId, change.MutationId);
                            await _reader.DeleteMessage(message, stoppingToken).ConfigureAwait(false);
                        }
                        else
                        {
                            _logger.LogWarning("a2. Couldn't process Entity ID: {entityId}; MutationId: {mutationId}", change.EntityId, change.MutationId);
                        }
                    }

                    _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                    await Task.Delay(100, stoppingToken).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}