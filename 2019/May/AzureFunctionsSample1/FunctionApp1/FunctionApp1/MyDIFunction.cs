using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace FunctionApp1
{
    public interface IGreeter
    {
        string RetrieveGreeting();
    }

    public class AwesomeGreeter : IGreeter
    {
        public string RetrieveGreeting()
        {
            return "Everything is AWESOME";
        }
    }

    public class MyDIFunction
    {
        private static CosmosClient cosmosClient;
        private readonly IGreeter greater;

        public MyDIFunction(CosmosClient myClient, IGreeter myGreeter)
        {
            cosmosClient = myClient;
            greater = myGreeter;
        }

        [FunctionName("MyDIFunction")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            name = name ?? data?.name;

            return name != null
                ? (ActionResult)new OkObjectResult($"{greater.RetrieveGreeting()}, {name}")
                : new BadRequestObjectResult("Please pass a name on the query string or in the request body");
        }
    }
}
