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

namespace LTM_AzureFunctionsApp.Functions.IoTHubTriggers
{
    public class LinFramesPacketAdd_IoTHubTrigger
    {
        [FunctionName("LinFramesPacketAdd_IoTHubTrigger")]
        public async Task Run(
        [IoTHubTrigger("messages/events", Connection = "IoTHubConnectionString")] EventData eventData,
        [ServiceBus("linframes-packet-add-queue", Connection = "ServiceBusConnectionString")] MessageSender serviceBusMsgQueue,
        [Queue("eventdata-parse-error-queue", Connection = "AzureWebJobsStorage")] IAsyncCollector<string> eventDataParseErrorQueue, ILogger _logger)
        {        
            try
            {
                string eventDataBody = Encoding.UTF8.GetString(eventData.Body.Array);
                dynamic jsonObj = JsonConvert.DeserializeObject(eventDataBody);

                //ServiceBus Queue
                var serviceBusMsg = new Message(Encoding.UTF8.GetBytes($"{eventDataBody}")) 
                {
                        SessionId = jsonObj.DEVID
                };

                await serviceBusMsgQueue.SendAsync(serviceBusMsg);
            }

            catch (Exception ex)
            {
                await eventDataParseErrorQueue.AddAsync($"[IoTHubTrigger] function [LinFramesPacketAdd] caught exception: {ex.Message} \nStackTrace: {ex.StackTrace} \nJSON: {Encoding.UTF8.GetString(eventData.Body.Array)}");
                _logger.LogError($"[IoTHubTrigger] function [LinFramesPacketAdd_IoTHubTrigger] caught exception: {ex.Message} \nStackTrace: {ex.StackTrace} \nJSON: {Encoding.UTF8.GetString(eventData.Body.Array)}");
                throw;
            }
        }
    }
}