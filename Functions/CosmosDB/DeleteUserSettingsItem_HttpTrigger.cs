using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using LTM_FunctionsApp.Models.Data;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents;
using System;

namespace LTM_FunctionsApp.Functions.CosmosDB
{
    public static class DeleteUserSettingsItem_HttpTrigger
    {
        [FunctionName("DeleteUserSettingsItem")]
        [FixedDelayRetry(5, "00:00:10")]
        public static async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = null)] HttpRequest req, ILogger log,
        [CosmosDB(databaseName: "ltmdb", collectionName: "userSettingsCollection", ConnectionStringSetting = "CosmosDBConnectionString", Id = "{Query.id}", PartitionKey = "{Query.UserID}")] UserSettingsItem document,
        [CosmosDB(databaseName: "ltmdb", collectionName: "userSettingsCollection", ConnectionStringSetting = "CosmosDBConnectionString")] DocumentClient client)
        {
            try
            {
                string itemId = req.Query["id"];
                string partitionKey = req.Query["UserID"];

                if (document is null)
                {
                    return new NotFoundResult();
                }

                await client.DeleteDocumentAsync(UriFactory.CreateDocumentUri("ltmdb", "userSettingsCollection", itemId), new RequestOptions() { PartitionKey = new PartitionKey(partitionKey) });                
            }
            catch (Exception ex)
            {
                log.LogError($"[HttpTrigger] function [DeleteUserSettingsItem_HttpTrigger] caught exception: \n{ex.Message}");
                throw;
            }

            return new OkResult();
        }
    }
}
