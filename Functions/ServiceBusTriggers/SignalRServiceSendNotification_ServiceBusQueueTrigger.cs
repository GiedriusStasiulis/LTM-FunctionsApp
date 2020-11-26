using System;
using System.Threading.Tasks;
using LTM_AzureFunctionsApp.Models.Data;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LTM_AzureFunctionsApp.Functions.ServiceBusQueueTriggers
{
    public static class SignalRServiceSendNotification_ServiceBusQueueTrigger
    {
        [FunctionName("SignalRServiceSendNotification_ServiceBusQueueTrigger")]
        public static async Task Run(
        [ServiceBusTrigger("signalrservice-send-notification", Connection = "ServiceBusConnectionString", IsSessionsEnabled = true)] string serviceBusMsgItem, ILogger log,
        [SignalR(HubName = Global.SignalRHubName)] IAsyncCollector<SignalRMessage> signalRMessage)
        {
            try
            {
                var jsonObj = JsonConvert.DeserializeObject<LinFramesPacket>(serviceBusMsgItem);

                SignalRMessage signalRMsg = new SignalRMessage()
                {
                    Target = "notify",
                    GroupName = jsonObj.DEVID,
                    Arguments = new[] { JsonConvert.SerializeObject(jsonObj) }
                };

                await signalRMessage.AddAsync(signalRMsg);                
            }
            catch(Exception ex)
            {
                log.LogError($"[ServiceBusTrigger] function [SignalRServiceSendNotification_ServiceBusQueueTrigger] caught exception: \n{ex.Message}");
                throw;
            }
        }
    }
}
