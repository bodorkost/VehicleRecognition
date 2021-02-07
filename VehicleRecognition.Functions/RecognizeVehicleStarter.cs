using System.IO;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using VehicleRecognition.Shared.DTOs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace VehicleRecognition.Functions
{
    public class RecognizeVehicleStarter
    {
        [FunctionName("RecognizeVehicleStarter")]
        public async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            [DurableClient] IDurableOrchestrationClient starter,
            ILogger log)
        {
            log.LogInformation("RecognizeVehicle function processed a request.");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var vehicleUrl = JsonConvert.DeserializeObject<PredictionInput>(requestBody);

            if (vehicleUrl == null)
            {
                return new BadRequestObjectResult("Please pass the vehicle url in the request body");
            }

            log.LogInformation($"About to start orchestration for {vehicleUrl}");

            var instanceId = await starter.StartNewAsync("O_RecognizeVehicle", vehicleUrl);

            return starter.CreateCheckStatusResponse(req, instanceId);
        }
    }
}
