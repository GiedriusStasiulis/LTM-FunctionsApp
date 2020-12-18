using System;
using System.Text;
using System.Threading.Tasks;
using LTM_AzureFunctionsApp;
using LTM_FunctionsApp.Models.Data;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LTM_FunctionsApp.Functions.ServiceBusTriggers
{
    public static class SignalRFrames_ServiceBusQueueTrigger
    {
        [FunctionName("SignalRFrames_ServiceBusQueueTrigger")]
        public static async Task Run(
        [ServiceBusTrigger("signalr-frames-servicebus-queue", 
            Connection = "ServiceBusConnectionString", 
            IsSessionsEnabled = true)] 
            Message serviceBusMsg, 
            ILogger log,
        [SignalR(HubName = Global.SignalRHubName)] 
            IAsyncCollector<SignalRMessage> signalRMessages,
        [Queue("error-frames-storage-queue", Connection = "AzureWebJobsStorage")] 
            IAsyncCollector<string> errorFramesStorageQueue)
        {
            try
            {
                log.LogInformation($"SignalRFrames_ServiceBusQueueTrigger received message: " +
                    $"{Encoding.UTF8.GetString(serviceBusMsg.Body)}");

                string serviceBusMessageBody = Encoding.UTF8.GetString(serviceBusMsg.Body);
                var jsonParseResult = JsonConvert.DeserializeObject<LinFramesPacket>(serviceBusMessageBody);

                string deviceIdShort = jsonParseResult.DEVID.Split("_")[0];

                SignalRMessage signalRMessage = new SignalRMessage()
                {
                    Target = "notifyFrames",
                    GroupName = deviceIdShort,
                    Arguments = new[] { JsonConvert.SerializeObject(jsonParseResult) }
                };

                await signalRMessages.AddAsync(signalRMessage);
            }
            catch(Exception ex)
            {
                await errorFramesStorageQueue.AddAsync($"[ServiceBusTrigger] function " +
                    $"[SignalRFrames_ServiceBusQueueTrigger] caught exception: " +
                    $"\n{ex.Message}\nStackTrace: {ex.StackTrace}");

                log.LogError($"[ServiceBusTrigger] function " +
                    $"[SignalRServiceSendNotification_ServiceBusQueueTrigger] caught exception: " +
                    $"\n{ex.Message}\nStackTrace: {ex.StackTrace}");
                throw;
            }
        }
    }
}
