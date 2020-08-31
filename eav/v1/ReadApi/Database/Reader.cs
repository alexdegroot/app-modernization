using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ReadApi.Database
{
    public class Reader
    {
        public Reader()
        {
            
        }

        public async Task GetEmployees()
        {
            // https://dotnetcodr.com/2016/05/17/introduction-to-mongodb-with-net-part-28-aggregation-in-the-net-driver-using-loose-typing-and-appendstage/
            // https://gist.github.com/peters/66abc9be2f6334e9d68603697b29d74f
            var client = new MongoClient();
            var db = client.GetDatabase("entities");
            var collection = db.GetCollection<Entity>("1000");

            var complexPipeline = new EmptyPipelineDefinition<Entity>()
                .AppendStage<Entity, Entity, Entity>(x => ) // Make an aggregation which searches for the right tenant, type and graphlookup
                // https://docs.mongodb.com/manual/reference/operator/aggregation/graphLookup/
                .Project(new BsonDocument()); // here define which fields to select

            var complexQueryResults = collection
                .Aggregate(complexPipeline)
                .ToList();

        }
    }

    public class Entity
    {
    }
}