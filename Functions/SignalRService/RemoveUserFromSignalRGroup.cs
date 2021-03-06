using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using System.Net;

namespace LTM_AzureFunctionsApp.Functions.UserFunctions
{
    public static class RemoveUserFromSignalRGroup
    {
        [FunctionName(nameof(RemoveUserFromSignalRGroup))]
        [FixedDelayRetry(5, "00:00:10")]
        public static IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "RemoveUserFromSignalRGroup/{deviceId}")] HttpRequest req, string deviceId, ILogger log,
        [SignalR(HubName = Global.SignalRHubName)]IAsyncCollector<SignalRGroupAction> signalRGroupActions)
        {
            signalRGroupActions.AddAsync(new SignalRGroupAction { UserId = req.Headers["X-MS-CLIENT-PRINCIPAL-ID"], GroupName = deviceId, Action = GroupAction.Remove });

            return new OkObjectResult(HttpStatusCode.OK);
        }
    }
}
