using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;
using System.Net;

namespace LTM_AzureFunctionsApp.Functions.UserFunctions
{
    public static class AddUserToSignalRGroup
    {
        [FunctionName(nameof(AddUserToSignalRGroup))]
        public static IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "AddUserToSignalRGroup/{deviceId}")] HttpRequest req, string deviceId, ILogger log,
        [SignalR(HubName = Global.SignalRHubName)]IAsyncCollector<SignalRGroupAction> signalRGroupActions)
        {
            signalRGroupActions.AddAsync(new SignalRGroupAction { UserId = req.Headers["X-MS-CLIENT-PRINCIPAL-ID"], GroupName = deviceId, Action = GroupAction.Add });

            return new OkObjectResult(HttpStatusCode.OK);
        }
    }
}
