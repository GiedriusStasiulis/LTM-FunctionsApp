using System;
using System.Text;
using System.Threading.Tasks;
using LTM_FunctionsApp.Models.Data;
using LTM_FunctionsApp.Shared;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LTM_AzureFunctionsApp.Functions.ServiceBusQueueTriggers
{
    public class DbInsertLinFramesPacket_ServiceBusQueueTrigger
    {
        readonly IObjectParseResultService<LinFramesPacket> _linFramesPacketParser = new ObjectParseResultService<LinFramesPacket>();

        [FunctionName("DbInsertLinFramesPacket_ServiceBusQueueTrigger")]
        public async Task Run(
        [ServiceBusTrigger("linframes-packet-add-queue", Connection = "ServiceBusConnectionString", IsSessionsEnabled = true)] Message serviceBusMsg, ILogger log,
        [CosmosDB(databaseName: "ltmdb", collectionName: "linFramesPacketsCollection", ConnectionStringSetting = "CosmosDBConnectionString", CreateIfNotExists = true, PartitionKey = "/DEVID")] IAsyncCollector<LinFramesPacket> linFramesPacketOut,
        [ServiceBus("signalrservice-send-notification", Connection = "ServiceBusConnectionString")] MessageSender signalR_ServiceBusMsgQueue,
        [Queue("error-queue", Connection = "AzureWebJobsStorage")] IAsyncCollector<string> errorQueue)
        {           
            try
            {
                log.LogInformation($"DbInsertLinFramesPacket_ServiceBusQueueTrigger received message: {Encoding.UTF8.GetString(serviceBusMsg.Body)}");

                string serviceBusMessageBody = Encoding.UTF8.GetString(serviceBusMsg.Body);
                var jsonParseResult = _linFramesPacketParser.TryParseObject(serviceBusMessageBody);

                if(jsonParseResult.Item2.Equals(true))
                {
                    bool dbInsertResult = Task.Run(async () => await linFramesPacketOut.AddAsync(jsonParseResult.Item1)).GetAwaiter().IsCompleted;

                    if(dbInsertResult)
                    {
                        //Send to signalR_ServiceBusMsgQueue
                        Message message = new Message(serviceBusMsg.Body)
                        {
                            SessionId = serviceBusMsg.SessionId
                        };

                        await signalR_ServiceBusMsgQueue.SendAsync(message);
                    }
                    else
                    {
                        //Send to error-queue
                        await errorQueue.AddAsync($"Error while inserting JSON to CosmosDB: {serviceBusMessageBody}");
                    }
                }
                else
                {
                    //Send to error-queue
                    await errorQueue.AddAsync($"Error inside [DbInsertLinFramesPacket_ServiceBusQueueTrigger] while parsing JSON: {serviceBusMessageBody}\nReason: {jsonParseResult.Item3}");
                }
            }
            catch(Exception ex)
            {
                //Send to error-queue
                await errorQueue.AddAsync($"[ServiceBusTrigger] function [DbInsertLinFramesPacket_ServiceBusQueueTrigger] caught exception: \n{ex.Message}\nStackTrace: {ex.StackTrace}");

                log.LogError($"[ServiceBusTrigger] function [DbInsertLinFramesPacket_ServiceBusQueueTrigger] caught exception: \n{ex.Message}\nStackTrace: {ex.StackTrace}");
                throw;
            }   
        }
    }
}
