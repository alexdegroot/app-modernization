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
            //CreateIndexes(collection);

            try
            {
                // Syntax is limiting me to get it to work like a pipeline as described here:
                // https://stackoverflow.com/questions/22664972/mongodb-upsert-on-array
                // That would be preferred as it would be a single database call, now it's several

                _logger.LogInformation($"Upserting entity {change.EntityId}");
                var entityResult = await EnsureEntity(change, collection, cancellationToken);

                if (entityResult.UpsertedId != null)
                {
                    _logger.LogInformation("Entity created.");
                }
                else if (entityResult.IsModifiedCountAvailable && entityResult.ModifiedCount > 0)
                {
                    _logger.LogInformation("Entity updated.");
                }
                else
                {
                    _logger.LogInformation("Entity not updated.");
                }


                //AddMutationIfNotExist(change, collection, cancellationToken);

                _logger.LogInformation($"Updating mutation if exist {change.MutationId}");
                var result = UpdateMutationIfExist(change, collection, cancellationToken);
                
                if (result.IsModifiedCountAvailable && result.ModifiedCount == 0)
                {
                    _logger.LogInformation($"Didn't update mutation");    
                    
                    _logger.LogInformation($"Insert new mutation"); 
                    var result2 = AppendMutation(change, collection, cancellationToken);
                    if (result2.IsModifiedCountAvailable && result.ModifiedCount > 0)
                    {
                        _logger.LogInformation("Mutation added");
                    }
                }
                else
                {
                    _logger.LogInformation($"Updated");
                }

            }
            catch (MongoException e)
            {
                _logger.LogError("Something went wrong inserting the change", e);
                return false;
            }

            return true;
        }

        private void CreateIndexes(IMongoCollection<Entity> collection)
        {
            var options = new CreateIndexOptions
            {
                Unique = true
            };
            var indexModel = new CreateIndexModel<Entity>(
                Builders<Entity>.IndexKeys.Ascending("Mutations.Id"), options);
            collection.Indexes.CreateOne(indexModel);
        }

        private static UpdateResult AppendMutation(Change change, IMongoCollection<Entity> collection,
            CancellationToken cancellationToken)
        {
            var entityFilter = Builders<Entity>.Filter.Where(x => x.Id == change.EntityId);
            var mutationInsert = Builders<Entity>.Update.Combine(
                Builders<Entity>.Update.Push(x => x.Mutations, new Mutation(change))
            );

            return collection.UpdateOne(entityFilter, mutationInsert, cancellationToken: cancellationToken);
        }

        private static UpdateResult AddMutationIfNotExist(Change change, IMongoCollection<Entity> collection,
            CancellationToken cancellationToken)
        {
            var mutationFilter = Builders<Entity>.Filter.And(
                Builders<Entity>.Filter.Where(x => x.Id == change.EntityId)//,
                //Builders<Entity>.Filter.ElemMatch(x => x.Mutations, x => x.MutationId == change.MutationId)
            );
            var update = Builders<Entity>.Update.AddToSet(x => x.Mutations, new Mutation(change));
            return collection.UpdateOne(mutationFilter, update, new UpdateOptions());
        }

        private static UpdateResult UpdateMutationIfExist(Change change, IMongoCollection<Entity> collection,
            CancellationToken cancellationToken)
        {
            var mutationFilter = Builders<Entity>.Filter.And(
                Builders<Entity>.Filter.Where(x => x.Id == change.EntityId),
                Builders<Entity>.Filter.ElemMatch(x => x.Mutations, x => x.MutationId == change.MutationId)
            );
            var update = Builders<Entity>.Update.Set("Mutations.$", new Mutation(change));
            return collection.UpdateOne(mutationFilter, update, new UpdateOptions());
        }

        private static async Task<UpdateResult> EnsureEntity(Change change, IMongoCollection<Entity> collection, CancellationToken cancellationToken)
        {
            var upsert = new UpdateOptions { IsUpsert = true };
            var entityFilter = Builders<Entity>.Filter.Where(x => x.Id == change.EntityId);
            var fullInsert = Builders<Entity>.Update.Combine(
                Builders<Entity>.Update.Set(x => x.TemplateId, change.TemplateId),
                Builders<Entity>.Update.Set(x => x.StartDate, change.EntityStartDate),
                Builders<Entity>.Update.Set(x => x.EndDate, change.EntityEndDate),
                Builders<Entity>.Update.Set(x => x.IsDeleted, change.IsDeleted)
            );
            return await collection.UpdateOneAsync(entityFilter, fullInsert, upsert);
        }
    }
}