using System;
using System.Threading.Tasks;
using LTM_FunctionsApp.Models.Data;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace LTM_FunctionsApp.Functions.StorageQueueTriggers
{
    public static class DbInsertFrames_StorageQueueTrigger
    {
        [FunctionName("DbInsertFrames_StorageQueueTrigger")]
        public static async Task Run(
        [QueueTrigger("db-insert-frames-storage-queue", 
            Connection = "AzureWebJobsStorage")] 
            LinFramesPacket linFramesPacketItem, 
            ILogger log,
        [CosmosDB(databaseName: "ltmdb", 
            collectionName: "linFramesPacketsCollection", 
            ConnectionStringSetting = "CosmosDBConnectionString", 
            CreateIfNotExists = true, PartitionKey = "/DEVID")] 
            IAsyncCollector<LinFramesPacket> linFramesPacketOut,
        [Queue("error-frames-storage-queue", Connection = "AzureWebJobsStorage")] 
            IAsyncCollector<string> errorFramesStorageQueue)
        {
            try
            {
                await linFramesPacketOut.AddAsync(linFramesPacketItem);
            }
            catch (Exception ex)
            {
                await errorFramesStorageQueue.AddAsync($"[StorageQueueTrigger] function " +
                    $"[DbInsertFrames_StorageQueueTrigger] caught exception: \n{ex.Message}" +
                    $"\nStackTrace: {ex.StackTrace}");

                log.LogError($"[StorageQueueTrigger] function " +
                    $"[DbInsertFrames_StorageQueueTrigger] caught exception: \n{ex.Message}" +
                    $"\nStackTrace: {ex.StackTrace}");
                throw;
            }
        }
    }
}
