using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using LTM_FunctionsApp.Models.Data;
using System;
using System.Net;

namespace LTM_FunctionsApp.Functions.CosmosDB
{
    public static class UpsertUserSettingsItem_HttpTrigger
    {
        [FunctionName("UpsertUserSettingsItem")]
        [FixedDelayRetry(5, "00:00:10")]
        public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req, ILogger log,
        [CosmosDB(databaseName: "ltmdb", collectionName: "userSettingsCollection", ConnectionStringSetting = "CosmosDBConnectionString", CreateIfNotExists = true, PartitionKey = "/UserID")] IAsyncCollector<UserSettingsItem> upsertDocumentOut)
        {
            try
            {
                string requestBody = await new StreamReader(req.Body).ReadToEndAsync();

                log.LogInformation($"User settings received: {requestBody}");

                UserSettingsItem data = JsonConvert.DeserializeObject<UserSettingsItem>(requestBody);

                await upsertDocumentOut.AddAsync(data);
            }
            catch (Exception ex)
            {
                log.LogError($"[HttpTrigger] function [SaveCustomByteNames_HttpTrigger] caught exception: \n{ex.Message}");
                throw;
            }

            return new OkObjectResult(HttpStatusCode.OK);
        }
    }
}
