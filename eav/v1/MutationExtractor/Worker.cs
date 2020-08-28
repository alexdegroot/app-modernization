using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MutationExtractor
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IQueueRepository _queue;
        private readonly IDatabaseRepository _database;

        public Worker(ILogger<Worker> logger, IQueueRepository queue, IDatabaseRepository database)
        {
            _queue = queue;
            _database = database;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!await _queue.Ensure(stoppingToken))
            {
                _logger.LogError("Couldn't ensure that Queue is there.");
                await Task.CompletedTask;
                return;
            }

            if (!await _database.Verify(stoppingToken))
            {
                _logger.LogError("Couldn't verify database connection.");
                await Task.CompletedTask;
                return;
            }

            while (!stoppingToken.IsCancellationRequested)
            {
                var changes = _database.GetChanges(stoppingToken).ConfigureAwait(true);

                await foreach (var change in changes.WithCancellation(stoppingToken))
                {
                    var messageGuid = Guid.NewGuid();
                    _logger.LogInformation("Create Message: {guid}", messageGuid);
                    string message = JsonSerializer.Serialize(change);
                    await _queue.AddMessage(message, stoppingToken).ConfigureAwait(false);
                }

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(500, stoppingToken).ConfigureAwait(false);
            }
        }
    }
}