using System;

namespace VehicleRecognition.Shared.DTOs
{
    public class PredictionResult
    {
        public string Name { get; set; }
        public string InstanceId { get; set; }
        public string RuntimeStatus { get; set; }
        public object Input { get; set; }
        public string CustomStatus { get; set; }
        public PredictionOutput Output { get; set; }
        public DateTime CreatedTime { get; set; }
        public DateTime LastUpdatedTime { get; set; }
    }
}