using System;
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
            StartDate = change.EntityStartDate;
            EndDate = change.EntityEndDate;
            TemplateId = change.TemplateId;
            IsDeleted = change.EntityDeleted;
        }

        [BsonId]
        [BsonRepresentation(BsonType.Int64)]
        public int Id { get; }

        [BsonElement]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? StartDate { get; }
        
        [BsonElement]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime? EndDate { get; }

        [BsonElement]
        public int TemplateId { get; }
        [BsonElement]
        public bool IsDeleted { get; }
        [BsonElement]
        public Mutation[] Mutations { get; }
    }
}