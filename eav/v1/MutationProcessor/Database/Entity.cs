using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MutationProcessor.Database
{
    internal class Entity
    {
        public Entity(Change change)
        {
            Id = change.EntityId;
            ParentId = change.EntityParentId;
            TemplateId = change.EntityTemplateId;
            StartDate = change.EntityStartDate;
            EndDate = change.EntityEndDate;
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
        public DateTime StartDate { get; set; }

        [BsonElement]
        public DateTime EndDate { get; set; }

        [BsonElement]
        public bool IsDeleted { get; set; }

        [BsonElement]
        public Mutation[] Mutations { get; set; }

        [BsonElement]
        public Entity[] ChildEntities { get; set; }
    }
}