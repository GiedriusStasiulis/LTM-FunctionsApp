using System;
using System.Text;
using System.Threading.Tasks;
using LTM_FunctionsApp.Models.Data;
using LTM_FunctionsApp.Shared;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LTM_AzureFunctionsApp.Functions.ServiceBusQueueTriggers
{
    public class SignalRServiceSendNotification_ServiceBusQueueTrigger
    {
        readonly IObjectParseResultService<LinFramesPacket> _linFramesPacketParser = new ObjectParseResultService<LinFramesPacket>();

        [FunctionName("SignalRServiceSendNotification_ServiceBusQueueTrigger")]
        public async Task Run(
        [ServiceBusTrigger("signalrservice-send-notification", Connection = "ServiceBusConnectionString", IsSessionsEnabled = true)] Message serviceBusMsg, ILogger log,
        [SignalR(HubName = Global.SignalRHubName)] IAsyncCollector<SignalRMessage> signalRMessages,
        [Queue("error-queue", Connection = "AzureWebJobsStorage")] IAsyncCollector<string> errorQueue)
        {
            try
            {
                log.LogInformation($"SignalRServiceSendNotification_ServiceBusQueueTrigger received message: {Encoding.UTF8.GetString(serviceBusMsg.Body)}");

                string serviceBusMessageBody = Encoding.UTF8.GetString(serviceBusMsg.Body);
                //LinFramesPacket linFramesPacket = JsonConvert.DeserializeObject<LinFramesPacket>(serviceBusMessageBody);
                var jsonParseResult = _linFramesPacketParser.TryParseObject(serviceBusMessageBody);

                if(jsonParseResult.Item2.Equals(true))
                {
                    string deviceIdShort = jsonParseResult.Item1.DEVID.Split("_")[0];

                    SignalRMessage signalRMessage = new SignalRMessage()
                    {
                        Target = "notify",
                        GroupName = deviceIdShort,
                        Arguments = new[] { JsonConvert.SerializeObject(jsonParseResult.Item1) }
                    };

                    bool signalRMessageSentResult = Task.Run(async () => await signalRMessages.AddAsync(signalRMessage)).GetAwaiter().IsCompleted;

                    if(!signalRMessageSentResult)
                    {
                        await errorQueue.AddAsync($"Error while sending message to SignalR Service: {serviceBusMessageBody}");
                    }
                }
                else
                {
                    //Send to error-queue
                    await errorQueue.AddAsync($"Error inside [SignalRServiceSendNotification_ServiceBusQueueTrigger] while parsing JSON: {serviceBusMessageBody}\nReason: {jsonParseResult.Item3}");
                }
            }
            catch(Exception ex)
            {
                //Send to error-queue
                await errorQueue.AddAsync($"[ServiceBusTrigger] function [SignalRServiceSendNotification_ServiceBusQueueTrigger] caught exception: \n{ex.Message}\nStackTrace: {ex.StackTrace}");

                log.LogError($"[ServiceBusTrigger] function [SignalRServiceSendNotification_ServiceBusQueueTrigger] caught exception: \n{ex.Message}\nStackTrace: {ex.StackTrace}");
                throw;
            }
        }
    }
}
