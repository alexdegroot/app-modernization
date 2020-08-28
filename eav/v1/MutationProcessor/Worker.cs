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
            if(!await _reader.Ensure(stoppingToken))
            {
                _logger.LogError("Couldn't ensure that Queue is there.");
                await Task.CompletedTask;
                return;
            }

            if (!await _writer.Verify(stoppingToken))
            {
                _logger.LogError("Couldn't verify database connection.");
                await Task.CompletedTask;
                return;
            }
            
            while (!stoppingToken.IsCancellationRequested)
            {
                var changes = new[]
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
                };

                foreach (var change in changes)
                {
                    _logger.LogInformation("Process Mutation with Entity ID: {entityId}; MutationId: {mutationId}", change.MutationId, change.MutationId);
                    var result = await _writer.Append(change, stoppingToken)
                        .ContinueWith(async x =>
                    {
                        await x;
                        _logger.LogInformation("done");
                    }, stoppingToken)
                        .ConfigureAwait(false);
                    _logger.LogInformation("En door!" + result.ToString());
                    
                    // if (result)
                    // {
                    //     _logger.LogInformation("Wel gelukt!");
                    // }
                }

                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.CompletedTask;
                return;
            }
        }
    }
}