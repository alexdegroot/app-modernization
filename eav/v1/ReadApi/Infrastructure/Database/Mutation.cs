using System;
using MongoDB.Bson.Serialization.Attributes;

namespace ReadApi.Infrastructure.Database
{
    public class Mutation
    {
        [BsonElement]
        public int MutationId { get; set; }

        [BsonElement]
        public DateTime? StartDate { get; set; }

        [BsonElement]
        public DateTime? EndDate { get; set; }

        [BsonElement]
        public int FieldId { get; set; }

        [BsonElement]
        public object Value { get; set; }

        [BsonElement]
        public bool IsDeleted { get; set; }
    }
}