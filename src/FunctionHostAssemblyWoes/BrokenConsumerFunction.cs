namespace FunctionHostAssemblyWoes
{
    using Microsoft.ApplicationInsights.Extensibility;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Azure.WebJobs;
    using Microsoft.Azure.WebJobs.Extensions.Http;
    using Microsoft.Extensions.Logging;

    public class BrokenConsumerFunction
    {
        public BrokenConsumerFunction(TelemetryConfiguration config, HttpContext context)
        {
        }

        [FunctionName("BrokenConsumer")]
        public IActionResult Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get")]HttpRequest req,
            ILogger log)
        {
            return new OkObjectResult("If you can see this, TelemetryConfiguration was matched to the runtime assembly!");
        }
    }
}
