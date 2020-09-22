
namespace MutationProcessor.Database
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using MongoDB.Driver;

    internal class EntityUpdater
    {
        private readonly Change _change;
        private readonly IMongoCollection<Entity> _collection;
        private readonly CancellationToken _cancellationToken;

        public EntityUpdater(Change change,
            IMongoCollection<Entity> collection, CancellationToken cancellationToken)
        {
            _change = change ?? throw new ArgumentNullException(nameof(change));
            _collection = collection ?? throw new ArgumentNullException(nameof(collection));
            _cancellationToken = cancellationToken;
        }

        public async Task AddOrUpdateChildEntity()
        {
            await EnsureParentEntity();
            await EnsureChildEntity();
            await AddMutation();
        }

        /// <summary>
        /// Tries to find a parent entity for the child entity that must be present to store
        /// the change. If no such parent entity exists, it will be created.
        /// </summary>
        /// <returns></returns>
        private async Task EnsureParentEntity()
        {
            var filter = Builders<Entity>.Filter.Where(x => x.Id == _change.EntityParentId);
            var updateDefinition = Builders<Entity>.Update.SetOnInsert(
                x => x.Id, _change.EntityParentId);
            await _collection.UpdateOneAsync(filter, updateDefinition,
                new UpdateOptions() { IsUpsert = true }, _cancellationToken);
        }

        /// <summary>
        /// Tries to find a child entity that must be present in the parent entity to store
        /// the change. If no such child entity exists, it will be created.
        /// </summary>
        /// <returns></returns>
        private async Task EnsureChildEntity()
        {
            var filter = Builders<Entity>.Filter.And(
                Builders<Entity>.Filter.Where(x => x.Id == _change.EntityParentId),
                Builders<Entity>.Filter.Ne("ChildEntities.Id", _change.EntityId)
            );

            var updateDefinition =
                Builders<Entity>.Update.Push(x => x.ChildEntities, new Entity(_change)
                {
                    Mutations = new Mutation[] { }
                });

            await _collection.UpdateOneAsync(filter, updateDefinition,
                new UpdateOptions(), _cancellationToken);
        }

        /// <summary>
        /// Adds the change data to the child entity.
        /// </summary>
        /// <returns></returns>
        private async Task AddMutation()
        {
            var childEntityFilter = Builders<Entity>.Filter.And(
                Builders<Entity>.Filter.Where(x => x.Id == _change.EntityParentId),
                Builders<Entity>.Filter.Eq("ChildEntities.Id", _change.EntityId)
            );

            // Note: The -1 in the ChildEntities array index represents the $ positional operator.
            // Compare to: ChildEntities.$.Mutations  in MongoDB syntax
            // Cf. https://docs.mongodb.com/manual/reference/operator/update/positional/
            var mutationsUpdateDefinition =
                Builders<Entity>.Update.AddToSet(x => x.ChildEntities[-1].Mutations,
                    new Mutation(_change));

            await _collection.UpdateOneAsync(childEntityFilter, mutationsUpdateDefinition,
                new UpdateOptions(), _cancellationToken);
        }
    }
}
