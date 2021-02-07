using System.Threading.Tasks;
using VehicleRecognition.Shared.DTOs;

namespace VehicleRecognition.Client.Services
{
    public interface IRecognizeService 
    {
        Task<StatusResponse> Predict(string url);
        Task<PredictionResult> GetPredictionResult(string url);
    }
}