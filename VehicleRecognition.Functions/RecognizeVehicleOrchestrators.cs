using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using VehicleRecognition.Shared.DTOs;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;

namespace VehicleRecognition.Functions
{
    public class RecognizeVehicleOrchestrators
    {
        [FunctionName("O_RecognizeVehicle")]
        public async Task<object> RecognizeVehicle(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            var input = context.GetInput<PredictionInput>();
            string prediction = null;

            try
            {
                if (!context.IsReplaying)
                {
                    log.LogInformation("About to call save image activity");
                }
                var path = await context.CallActivityAsync<string>("A_SaveImage", input.Url);

                if (!context.IsReplaying)
                {
                    log.LogInformation("About to call train model sub orchestrator");
                }
                await context.CallSubOrchestratorAsync("O_TrainModel", null);

                if (!context.IsReplaying)
                {
                    log.LogInformation("About to call predict image activity");
                }
                prediction = await context.CallActivityAsync<string>("A_PredictImage", path);

                if (!context.IsReplaying)
                {
                    log.LogInformation("About to call delete image activity");
                }
                await context.CallActivityAsync("A_DeleteImage", path);
            }
            catch (Exception e)
            {
                if (!context.IsReplaying)
                {
                    log.LogError($"Caught an error from an activity: {e.Message}");

                    return new
                    {
                        Error = "Failed to recognize uploaded vehicle",
                        e.Message,
                        e.StackTrace,
                        e.Source
                    };
                }
            }

            return new
            {
                Prediction = prediction
            };
        }

        [FunctionName("O_TrainModel")]
        public async Task TrainModel(
            [OrchestrationTrigger] IDurableOrchestrationContext context,
            ILogger log)
        {
            if (!context.IsReplaying)
            {
                log.LogInformation("About to call train model activity");
            }
            await context.CallActivityAsync("A_TrainModel", null);

            if (!context.IsReplaying)
            {
                log.LogInformation("About to call test model activity");
            }
            await context.CallActivityAsync("A_TestModel", null);
        }
    }
}