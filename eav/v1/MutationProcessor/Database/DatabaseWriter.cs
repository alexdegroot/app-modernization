using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace MutationProcessor.Database
{
    public class DatabaseWriter : IDatabaseWriter
    {
        private readonly ILogger<DatabaseWriter> _logger;
        private readonly MongoClient _client;

        public DatabaseWriter(IOptions<Configuration> config, ILogger<DatabaseWriter> logger)
        {
            _logger = logger;
            var connectionString = config.Value.Mongo_Connectionstring ?? throw new ArgumentException(nameof(config.Value.Mongo_Connectionstring));
            _client = new MongoClient(connectionString);
        }
        
        public async Task<bool> Verify(CancellationToken cancellationToken)
        {
            try
            {
                var dbs = await _client.ListDatabaseNamesAsync(cancellationToken);
                
                // Every db server at least has a few system databases
                if (await dbs.AnyAsync(cancellationToken))
                {
                    return true;
                }
            }
            catch (MongoException ex)
            {
                _logger.LogError("Couldn't connect to MongoDB", ex);
            }

            return false;
        }

        public async Task<bool> Append(Change change, CancellationToken cancellationToken)
        {
            var db = _client.GetDatabase("entities");
            var collection = db.GetCollection<Entity>(change.TenantId.ToString());
            await CreateIndexes(collection);
            try
            {
                // Syntax is limiting me to get it to work like a pipeline as described here:
                // https://stackoverflow.com/questions/22664972/mongodb-upsert-on-array
                // That would be preferred as it would be a single database call, now it's several

                _logger.LogInformation($"b1: Upserting entity {change.EntityId}");
                var entityResult =  await EnsureEntity(change, collection, cancellationToken).ConfigureAwait(true);

                if (entityResult.UpsertedId != null)
                {
                    _logger.LogInformation($"b2: New entity created {change.EntityId}");
                }
                else if (entityResult.IsModifiedCountAvailable && entityResult.ModifiedCount > 0)
                {
                    _logger.LogInformation($"b2: Entity updated {change.EntityId}");
                }
                else
                {
                    _logger.LogInformation($"b2: Entity not updated {change.EntityId}");
                }
                
                
                _logger.LogInformation($"b3: Insert mutation if not exist {change.MutationId}");

                var result = await AddMutationIfNotExist(change, collection,cancellationToken);

                if (result.IsModifiedCountAvailable && result.ModifiedCount == 0)
                {
                    _logger.LogInformation($"b4: Updating mutation {change.MutationId}");
                    await UpdateMutation(change, collection, cancellationToken);
                }
                else if(result.IsModifiedCountAvailable && result.ModifiedCount == 1)
                {
                    _logger.LogInformation($"b4: Mutation inserted {change.MutationId}");
                }
                
                _logger.LogInformation($"b5: Done mutation processing {change.MutationId}");
                
            }
            catch (MongoException e)
            {
                _logger.LogError("Something went wrong inserting the change", e);
                return false;
            }

            return true;
        }

        private static async Task CreateIndexes(IMongoCollection<Entity> collection)
        {
            var options = new CreateIndexOptions
            {
                Unique = true
            };
            var indexModel = new CreateIndexModel<Entity>(
                Builders<Entity>.IndexKeys.Ascending(nameof(Mutation) + "." + nameof(Mutation.MutationId)), options);
            await collection.Indexes.CreateOneAsync(indexModel);
        }

        private static async Task<UpdateResult> EnsureEntity(Change change, IMongoCollection<Entity> collection, CancellationToken cancellationToken)
        {
            var upsert = new UpdateOptions { IsUpsert = true };
            var entityFilter = Builders<Entity>.Filter.Eq(x => x.Id, change.EntityId);
            var fullInsert = Builders<Entity>.Update.Combine(
                Builders<Entity>.Update.Set(x => x.TemplateId, change.TemplateId),
                Builders<Entity>.Update.Set(x => x.StartDate, change.EntityStartDate),
                Builders<Entity>.Update.Set(x => x.EndDate, change.EntityEndDate),
                Builders<Entity>.Update.Set(x => x.IsDeleted, change.IsDeleted)
            );
            return await collection.UpdateOneAsync(entityFilter, fullInsert, upsert, cancellationToken);
        }
        
        private static async Task<UpdateResult> AddMutationIfNotExist(Change change, IMongoCollection<Entity> collection, CancellationToken cancellationToken)
        {
            var mutationFilter = Builders<Entity>.Filter.Not(
                Builders<Entity>.Filter.Where(x => x.Id == change.EntityId) &
                Builders<Entity>.Filter.ElemMatch(x => x.Mutations, Builders<Mutation>.Filter.Eq(x => x.MutationId, change.MutationId)));
            var update = Builders<Entity>.Update.AddToSet(x => x.Mutations, new Mutation(change));
            return await collection.UpdateOneAsync(mutationFilter, update, new UpdateOptions(), cancellationToken).ConfigureAwait(true);
        }
        
        private async Task UpdateMutation(Change change, IMongoCollection<Entity> collection, CancellationToken cancellationToken)
        {
            var mutationFilter = Builders<Entity>.Filter.Where(x => x.Id == change.EntityId) &
                                 Builders<Entity>.Filter.ElemMatch(x => x.Mutations, Builders<Mutation>.Filter.Eq(x => x.MutationId, change.MutationId));
            var update = Builders<Entity>.Update.Set("Mutations.$", new Mutation(change));
            
            await collection.UpdateOneAsync(mutationFilter, update, new UpdateOptions(), cancellationToken).ConfigureAwait(true);
        }
    }
}