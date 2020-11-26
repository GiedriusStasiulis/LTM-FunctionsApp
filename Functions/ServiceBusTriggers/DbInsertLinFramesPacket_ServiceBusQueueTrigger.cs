using System;
using System.Text;
using System.Threading.Tasks;
using LTM_AzureFunctionsApp.Models.Data;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LTM_AzureFunctionsApp.Functions.ServiceBusQueueTriggers
{
    public static class DbInsertLinFramesPacket_ServiceBusQueueTrigger
    {
        [FunctionName("DbInsertLinFramesPacket_ServiceBusQueueTrigger")]
        public static async Task Run(
        [ServiceBusTrigger("linframes-packet-add-queue", Connection = "ServiceBusConnectionString", IsSessionsEnabled = true)] string serviceBusMsgItem, ILogger log,
        [CosmosDB(databaseName: "ltmdb", collectionName: "linframespackets", ConnectionStringSetting = "CosmosDBConnectionString", CreateIfNotExists = true, PartitionKey = "/DEVID")] IAsyncCollector<LinFramesPacket> linFramesPacketOut,
        [ServiceBus("signalrservice-send-notification", Connection = "ServiceBusConnectionString")] MessageSender serviceBusMsgQueue)
        {           
            try
            {
                var jsonObj = JsonConvert.DeserializeObject<LinFramesPacket>(serviceBusMsgItem);

                var linFramesPacket = new LinFramesPacket
                {
                    PCKNO = jsonObj.PCKNO,
                    DEVID = jsonObj.DEVID,
                    FRAMES = jsonObj.FRAMES
                };

                await linFramesPacketOut.AddAsync(linFramesPacket);                

                try
                {
                    //ServiceBus Queue
                    var serviceBusMsg = new Message(Encoding.UTF8.GetBytes($"{serviceBusMsgItem}")) 
                    {
                            SessionId = jsonObj.DEVID
                    };

                    await serviceBusMsgQueue.SendAsync(serviceBusMsg);
                }
                catch(Exception ex)
                {
                    log.LogError($"{ex.Message}");
                    throw;
                }               
            }
            catch(Exception ex)
            {
                log.LogError($"[ServiceBusTrigger] function [DbInsertLinFramesPacket_ServiceBusQueueTrigger] caught exception: \n{ex.Message}");
                throw;
            }   
        }
    }
}
