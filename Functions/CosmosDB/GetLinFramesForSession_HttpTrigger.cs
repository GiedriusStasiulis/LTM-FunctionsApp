using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using LTM_FunctionsApp.Models.Data;

namespace LTM_FunctionsApp.Functions.CosmosDB
{
    public static class GetLinFramesForSession_HttpTrigger
    {
        [FunctionName("GetLinFramesForSession_HttpTrigger")]
        [FixedDelayRetry(5, "00:00:10")]
        public static IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "GetLinFramesForSession/{SessionID}")] HttpRequest req, ILogger log,
        [CosmosDB(databaseName: "ltmdb", collectionName: "linFramesPacketsCollection", ConnectionStringSetting = "CosmosDBConnectionString",
            SqlQuery = "SELECT * FROM linFramesPacketsCollection lfp WHERE lfp.DEVID = {SessionID} ORDER BY lfp.PCKNO ASC")] IEnumerable<LinFramesPacket> documents)
        {
            if (documents is null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(documents);
        }
    }
}
