using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ReadApi.Database
{
    public class Entity
    {
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
    }
}