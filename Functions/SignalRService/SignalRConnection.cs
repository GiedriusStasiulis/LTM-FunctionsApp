using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.WebJobs.Extensions.SignalRService;

namespace LTM_AzureFunctionsApp.Functions.UserFunctions
{
    public static class SignalRConnection
    {
        [FunctionName("negotiate")]
        [FixedDelayRetry(5, "00:00:10")]
        public static IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req, IBinder binder, ILogger log)
        {
            var connectionInfo = binder.Bind<SignalRConnectionInfo>(new SignalRConnectionInfoAttribute { HubName = Global.SignalRHubName, UserId = req.Headers["X-MS-CLIENT-PRINCIPAL-ID"], ConnectionStringSetting = "AzureSignalRConnectionString" });
            return new OkObjectResult(connectionInfo);
        }
    }
}