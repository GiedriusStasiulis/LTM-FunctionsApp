using IoTHubTrigger = Microsoft.Azure.WebJobs.EventHubTriggerAttribute;

using Microsoft.Azure.WebJobs;
using Microsoft.Azure.EventHubs;
using System.Text;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System;
using Microsoft.Azure.ServiceBus.Core;
using Microsoft.Azure.ServiceBus;
using LTM_FunctionsApp.Models.Data;
using LTM_FunctionsApp.Shared;

namespace LTM_AzureFunctionsApp.Functions.IoTHubTriggers
{
    public class LinFramesPacketAdd_IoTHubTrigger
    {
        readonly IObjectParseResultService<LinFramesPacket> _linFramesPacketParser = new ObjectParseResultService<LinFramesPacket>();

        [FunctionName("LinFramesPacketAdd_IoTHubTrigger")]
        public async Task Run(
        [IoTHubTrigger("messages/events", Connection = "IoTHubConnectionString")] EventData eventData, ILogger log,
        [ServiceBus("linframes-packet-add-queue", Connection = "ServiceBusConnectionString")] MessageSender cosmosDb_ServiceBusMsgQueue,
        [Queue("error-queue", Connection = "AzureWebJobsStorage")] IAsyncCollector<string> errorQueue)
        {        
            try
            {
                log.LogInformation($"LinFramesPacketAdd_IoTHubTrigger received message: {Encoding.UTF8.GetString(eventData.Body.Array)}");
                string eventDataBody = Encoding.UTF8.GetString(eventData.Body.Array);
                var jsonParseResult = _linFramesPacketParser.TryParseObject(eventDataBody);

                if(jsonParseResult.Item2.Equals(true))
                {
                    //Send to cosmosDb_ServiceBusMsgQueue
                    Message message = new Message(eventData.Body.Array)
                    {
                        SessionId = jsonParseResult.Item1.DEVID
                    };

                    await cosmosDb_ServiceBusMsgQueue.SendAsync(message);
                }

                else
                {
                    //Send to error-queue
                    await errorQueue.AddAsync($"Error inside [LinFramesPacketAdd_IoTHubTrigger] while parsing JSON: {eventDataBody}\nReason: {jsonParseResult.Item3}");
                }                
            }

            catch (Exception ex)
            {
                //Send to json-parse-error-queue
                await errorQueue.AddAsync($"[IoTHubTrigger] function [LinFramesPacketAdd_IoTHubTrigger] caught exception: {ex.Message} \nStackTrace: {ex.StackTrace}");
               
                log.LogError($"[IoTHubTrigger] function [LinFramesPacketAdd_IoTHubTrigger] caught exception: {ex.Message} \nStackTrace: {ex.StackTrace}");
                throw;
            }
        }
    }
}