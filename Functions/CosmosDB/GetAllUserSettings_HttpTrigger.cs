using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using LTM_FunctionsApp.Models.Data;

namespace LTM_FunctionsApp.Functions.CosmosDB
{
    public static class GetAllUserSettings_HttpTrigger
    {
        [FunctionName("GetAllUserSettings_HttpTrigger")]
        [FixedDelayRetry(5, "00:00:10")]
        public static IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetAllUserSettings/{UserID}")] HttpRequest req, ILogger log,
        [CosmosDB(databaseName: "ltmdb", collectionName: "userSettingsCollection", ConnectionStringSetting = "CosmosDBConnectionString",
            SqlQuery = "SELECT * FROM userSettingsCollection cbn WHERE cbn.UserID = {UserID}")] IEnumerable<UserSettingsItem> documents)
        {
            if (documents is null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(documents);
        }
    }
}
