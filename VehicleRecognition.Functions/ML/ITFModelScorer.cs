using System.Threading.Tasks;

namespace VehicleRecognition.Functions.ML
{
    public interface ITFModelScorer
    {
        Task<bool> Train();
        Task Test();
        Task<string> ClassifySingleImage(string imagePath);
    }
}
