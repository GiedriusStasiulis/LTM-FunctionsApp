using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.EventHubs;
using System.Text;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using System;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus;
using LTM_FunctionsApp.Models.Data;
using LTM_FunctionsApp.Shared;

namespace LTM_AzureFunctionsApp.Functions.IoTHubTriggers
{
    public static class LinFramesPacketAdd_IoTHubTrigger
    {
        static readonly IObjectParseResultService<LinFramesPacket> 
            _linFramesPacketParser = new ObjectParseResultService<LinFramesPacket>();

        [FunctionName("LinFramesPacketAdd_IoTHubTrigger")]
        public static async Task Run(
        [IoTHubTrigger("messages/events", Connection = "IoTHubConnectionString")] 
            EventData eventData, ILogger log,
        [ServiceBus("signalr-frames-servicebus-queue", Connection = "ServiceBusConnectionString")] 
            MessageSender signalRFramesServiceBusQueue, 
        [Queue("db-insert-frames-storage-queue", Connection = "AzureWebJobsStorage")] 
            IAsyncCollector<LinFramesPacket> dbInsertFramesStorageQueue,
        [Queue("error-frames-storage-queue", Connection = "AzureWebJobsStorage")] 
            IAsyncCollector<string> errorFramesStorageQueue)
        {        
            try
            {
                log.LogInformation($"LinFramesPacketAdd_IoTHubTrigger received message: " +
                    $"{Encoding.UTF8.GetString(eventData.Body.Array)}");

                string eventDataBody = Encoding.UTF8.GetString(eventData.Body.Array);
                var jsonParseResult = _linFramesPacketParser.TryParseObject(eventDataBody);

                if (jsonParseResult.Item2.Equals(true))
                {
                    Message message = new Message(eventData.Body.Array)
                    {
                        //Example DEVID = ESP32SIM1_1608006300
                        SessionId = jsonParseResult.Item1.DEVID
                    };

                    //Send message to ServiceBus queue: signalr-frames-servicebus-queue
                    await signalRFramesServiceBusQueue.SendAsync(message)
                    //Send LinFramesPacket to Storage queue: db-insert-frames-storage-queue
                    await dbInsertFramesStorageQueue.AddAsync(jsonParseResult.Item1);
                }

                else
                {
                    await errorFramesStorageQueue.AddAsync($"Error inside " +
                        $"[LinFramesPacketAdd_IoTHubTrigger]" +
                        $" while parsing JSON: {eventDataBody}\nReason: {jsonParseResult.Item3}");
                }                
            }

            catch (Exception ex)
            {                
                await errorFramesStorageQueue.AddAsync($"[IoTHubTrigger] function " +
                    $"[LinFramesPacketAdd_IoTHubTrigger]" +
                    $" caught exception: {ex.Message} \nStackTrace: {ex.StackTrace}");
               
                log.LogError($"[IoTHubTrigger] function " +
                    $"[LinFramesPacketAdd_IoTHubTrigger] caught exception: {ex.Message} " +
                    $"\nStackTrace: {ex.StackTrace}");
                throw;
            }
        }
    }
}