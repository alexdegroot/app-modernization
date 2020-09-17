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
                _logger.LogInformation("Verifying existence of MongoDB...");
                var dbs = await _client.ListDatabaseNamesAsync(cancellationToken);
                
                // Every db server at least has a few system databases.
                if (await dbs.AnyAsync(cancellationToken))
                {
                    return true;
                }
            }
            catch (MongoException ex)
            {
                _logger.LogError(ex, "Cannot connect to MongoDB.");
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
                var entityResult =  await EnsureEntity(change, collection, cancellationToken).ConfigureAwait(false);

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
            var options = new CreateIndexOptions();
            var indexModel = new CreateIndexModel<Entity>(
                Builders<Entity>.IndexKeys.Ascending(nameof(Entity.Mutations) + "." + nameof(Mutation.MutationId)), options);
            await collection.Indexes.CreateOneAsync(indexModel);
        }

        private static async Task<UpdateResult> EnsureEntity(Change change, IMongoCollection<Entity> collection, CancellationToken cancellationToken)
        {
            var upsert = new UpdateOptions { IsUpsert = true };
            var entityFilter = Builders<Entity>.Filter.Eq(x => x.Id, change.EntityId);
            var fullInsert = Builders<Entity>.Update.Combine(
                Builders<Entity>.Update.SetOnInsert(x => x.Id, change.EntityId),
                Builders<Entity>.Update.Set(x => x.ParentId, change.EntityParentId),
                Builders<Entity>.Update.Set(x => x.TemplateId, change.EntityTemplateId),
                Builders<Entity>.Update.Set(x => x.IsDeleted, change.EntityDeleted)
            );
            return await collection.UpdateOneAsync(entityFilter, fullInsert, upsert, cancellationToken);
        }
        
        private static async Task<UpdateResult> AddMutationIfNotExist(Change change, IMongoCollection<Entity> collection, CancellationToken cancellationToken)
        {
            var mutationFilter = Builders<Entity>.Filter.And(
                Builders<Entity>.Filter.Eq(x => x.Id, change.EntityId),
                Builders<Entity>.Filter.Ne("Mutations.MutationId", change.MutationId));
            var update = Builders<Entity>.Update.AddToSet(x => x.Mutations, new Mutation(change));
            return await collection.UpdateOneAsync(mutationFilter, update, new UpdateOptions(), cancellationToken).ConfigureAwait(true);
        }
        
        private static async Task UpdateMutation(Change change, IMongoCollection<Entity> collection, CancellationToken cancellationToken)
        {
            var mutationFilter = Builders<Entity>.Filter.And( 
                                    Builders<Entity>.Filter.Eq(x => x.Id, change.EntityId),
                                    Builders<Entity>.Filter.Eq("Mutations.MutationId", change.MutationId));
            var update = Builders<Entity>.Update.Set(x => x.Mutations[-1], new Mutation(change));
            
            await collection.UpdateOneAsync(mutationFilter, update, new UpdateOptions(), cancellationToken).ConfigureAwait(true);
        }
    }
}