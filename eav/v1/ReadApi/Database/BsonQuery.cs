
namespace ReadApi.Database
{
    using System;
    using MongoDB.Bson;

    public static class BsonQuery
    {
        /// <summary>
        /// Creates a filter to query on documents that match the specified reference date
        /// and have their 'is deleted' status set to false.
        /// </summary>
        /// <param name="referenceDate"></param>
        /// <param name="filterInput"></param>
        /// <param name="filterOnParentDeletedStatus"></param>
        /// <returns>A <see cref="BsonDocument"/> that can be used to filter documents.</returns>
        public static BsonDocument CreateMutationsFilter(DateTime referenceDate, string filterInput = "$Mutations",
            bool filterOnParentDeletedStatus = false)
        {
            var bsonDocument = new BsonDocument
            {
                {
                    "$filter", new BsonDocument
                    {
                        { "input", filterInput },
                        { "as", "mutation" },
                        {
                            "cond", new BsonDocument
                            {
                                {
                                    "$and", new BsonArray
                                    {
                                        new BsonDocument
                                        {
                                            {
                                                "$eq", new BsonArray
                                                {
                                                    "$$mutation.IsDeleted", false
                                                }
                                            }
                                        },
                                        new BsonDocument
                                        {
                                            {
                                                "$gte", new BsonArray
                                                {
                                                    referenceDate, "$$mutation.StartDate"
                                                }
                                            }
                                        },
                                        new BsonDocument
                                        {
                                            {
                                                "$lte", new BsonArray
                                                {
                                                    referenceDate, "$$mutation.EndDate"
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            if (filterOnParentDeletedStatus)
            {
                AddParentDeletedCheck(bsonDocument);
            }

            return bsonDocument;
        }

        public static BsonDocument CreateChildEntitiesMapper(DateTime referenceDate,
            bool filterOnParentDeletedStatus = false)
        {
            var bsonDocument = new BsonDocument
            {
                {
                    "$map", new BsonDocument
                    {
                        {"input", "$ChildEntities"},
                        {"as", "entity"},
                        {
                            "in", new BsonDocument
                            {
                                {
                                    "Mutations", BsonQuery.CreateMutationsFilter(referenceDate, "$$entity.Mutations",
                                        filterOnParentDeletedStatus)
                                }
                            }
                        }
                    }
                }
            };

            return bsonDocument;
        }

        private static void AddParentDeletedCheck(BsonDocument bsonDocument)
        {
            var filterElement = bsonDocument["$filter"];
            var condElement = filterElement["cond"];
            var andArray = condElement["$and"].AsBsonArray;

            andArray.Add(new BsonDocument
            {
                {
                    "$eq", new BsonArray
                    {
                        "$$entity.IsDeleted", false
                    }
                }
            });
        }
    }
}
