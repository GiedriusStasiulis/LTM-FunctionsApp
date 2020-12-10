using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using LTM_FunctionsApp.Models.Data;

namespace LTM_FunctionsApp.Functions.CosmosDB
{
    public static class GetSingleUserSettingsItem_HttpTrigger
    {
        [FunctionName("GetSingleUserSettingsItem")]
        [FixedDelayRetry(5, "00:00:10")]
        public static IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req, ILogger log,
        [CosmosDB(databaseName: "ltmdb", collectionName: "userSettingsCollection", ConnectionStringSetting = "CosmosDBConnectionString", Id = "{Query.id}", PartitionKey = "{Query.UserID}")] UserSettingsItem document)
        {
            if(document is null)
            {                
                return new NotFoundResult();
            }

            return new OkObjectResult(document);
        }
    }
}
