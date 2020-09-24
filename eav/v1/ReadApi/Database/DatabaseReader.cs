﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ReadApi.Database
{
    public class DatabaseReader : IDatabaseReader
    {
        private const string EntitiesDbName = "entities";
        private readonly ILogger<DatabaseReader> _logger;
        private readonly MongoClient _mongoClient;

        public DatabaseReader(IOptions<Configuration> config, ILogger<DatabaseReader> logger)
        {
            _logger = logger;
            var connectionString = config.Value.Mongo_Connectionstring ?? throw new ArgumentException(nameof(config.Value.Mongo_Connectionstring));
            _mongoClient = new MongoClient(connectionString);
        }

        public async Task<Entity> ReadEntity(string tenantCode, int entityId)
        {
            if (entityId <= 0)
            {
                throw new ArgumentException("Invalid entity ID", nameof(entityId));
            }

            var entityCollection = GetEntityCollection(tenantCode);

            var dtNow = DateTime.Now.Date;
            var entity = await entityCollection.Aggregate()
                .Match(e => e.Id == entityId && !e.IsDeleted)
                .Project<Entity>(new BsonDocument
                    {
                        {
                            "Mutations", new BsonDocument
                            {
                                {
                                    "$filter", new BsonDocument
                                    {
                                        {"input", "$Mutations"},
                                        {"as", "mutation"},
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
                                                                    dtNow, "$$mutation.StartDate"
                                                                }
                                                            }
                                                        },
                                                        new BsonDocument
                                                        {
                                                            {
                                                                "$lte", new BsonArray
                                                                {
                                                                    dtNow, "$$mutation.EndDate"
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                ).SingleOrDefaultAsync();

            return entity;
        }

        public async Task<IList<Entity>> ReadEntities(string tenantCode, int parentEntityId, int templateId = 21)
        {
            if (parentEntityId <= 0)
            {
                throw new ArgumentException("Invalid parent entity ID", nameof(parentEntityId));
            }

            var entityCollection = GetEntityCollection(tenantCode);

            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var dtNow = DateTime.Now.Date;
            var aggregate = entityCollection.Aggregate()
                .Match(e => e.ParentId == parentEntityId)
                .Match(e => e.TemplateId == templateId)
                .Match(e => !e.IsDeleted)
                .Project<Entity>(new BsonDocument
                    {
                        {
                            "Mutations", new BsonDocument
                            {
                                {
                                    "$filter", new BsonDocument
                                    {
                                        {"input", "$Mutations"},
                                        {"as", "mutation"},
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
                                                                    dtNow, "$$mutation.StartDate"
                                                                }
                                                            }
                                                        },
                                                        new BsonDocument
                                                        {
                                                            {
                                                                "$lte", new BsonArray
                                                                {
                                                                    dtNow, "$$mutation.EndDate"
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                );

            var entities = await aggregate.ToListAsync();

            stopwatch.Stop();
            _logger.LogInformation($"Executed MongoDB query in {stopwatch.ElapsedMilliseconds} milliseconds.");

            return entities;
        }

        private IMongoCollection<Entity> GetEntityCollection(string tenantCode)
        {
            if (string.IsNullOrWhiteSpace(tenantCode))
            {
                throw new ArgumentException("Invalid tenant code", nameof(tenantCode));
            }
            // TODO: Maybe introduce other validations on the tenantCode value.

            // Get a reference to the 'entities' Mongo db.
            var db = _mongoClient.GetDatabase(EntitiesDbName);

            // Get a collection of entities for the tenant with the specified ID.
            var tenantCollectionName = tenantCode;
            var collection = db.GetCollection<Entity>(tenantCollectionName);
            if (collection == null)
            {
                throw new MongoClientException($"A collection with name {tenantCollectionName} does not exist.");
            }

            return collection;
        }
    }
}
