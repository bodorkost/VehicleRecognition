using Microsoft.ML;
using VehicleRecognition.Functions.ML;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;

[assembly: FunctionsStartup(typeof(VehicleRecognition.Functions.Startup))]
namespace VehicleRecognition.Functions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddHttpClient();

            builder.Services.AddSingleton<ITFModelScorer, TFModelScorer>();
            builder.Services.AddSingleton(new MLContext());
        }
    }
}
