using System.Diagnostics.CodeAnalysis;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MutationProcessor.Database
{
    [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
    internal class Entity
    {
        public Entity(Change change)
        {
            Id = change.EntityId;
            ParentId = change.EntityParentId;
            TemplateId = change.EntityTemplateId;
            IsDeleted = change.EntityDeleted;
        }

        [BsonId]
        [BsonRepresentation(BsonType.Int64)]
        public int Id { get; set; }

        [BsonElement]
        public int ParentId { get; set; }

        [BsonElement]
        public int TemplateId { get; set; }

        [BsonElement]
        public bool IsDeleted { get; set; }

        [BsonElement]
        public Mutation[] Mutations { get; set; }

        [BsonElement]
        public Entity[] ChildEntities { get; set; }
    }
}