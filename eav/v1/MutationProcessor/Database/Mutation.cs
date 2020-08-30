using System;
using System.Diagnostics.CodeAnalysis;
using MongoDB.Bson.Serialization.Attributes;

namespace MutationProcessor.Database
{
    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
    public class Mutation
    {
        [BsonElement]
        public int MutationId { get; }
        [BsonElement]
        public DateTime? StartDate { get; }
        [BsonElement]
        public DateTime? EndDate { get; }
        [BsonElement]
        public int FieldId { get; }
        [BsonElement]
        public object Value { get; }
        [BsonElement]
        public bool IsDeleted { get; }

        public Mutation(Change change)
        {
            MutationId = change.MutationId;
            StartDate = change.MutationStartDate;
            EndDate = change.MutationEndDate;
            IsDeleted = change.MutationDeleted;
            FieldId = change.FieldId;
            Value = change.FieldValue;
        }
    }
}