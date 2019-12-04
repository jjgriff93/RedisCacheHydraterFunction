using System;
using System.Collections.Generic;
using Microsoft.Azure.Documents;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Team1.Functions
{
    public static class CacheUpdater
    {
        private static IConnectionMultiplexer _redisConnectionMultiplexer =
            ConnectionMultiplexer.Connect("REDIS_CONN_STRNG");

        [FunctionName("CacheUpdater")]
        public static void Run([CosmosDBTrigger(
            databaseName: "contosomovie",
            collectionName: "Category",
            ConnectionStringSetting = "team1cosmos_DOCUMENTDB",
            LeaseCollectionName = "leases",
            CreateLeaseCollectionIfNotExists = true,
            StartFromBeginning = true)]IReadOnlyList<Document> input, ILogger log)
        {
            if (input == null || input.Count <= 0) return;

            var db = _redisConnectionMultiplexer.GetDatabase();

            foreach (var document in input)
            {
                //db.StringSet(document.Id, document.ToString());
                db.ListLeftPush("categories", document.ToString());
                log.LogInformation($"Saved item with id {document.Id} in Azure Redis cache");
            }
        }
    }
}
