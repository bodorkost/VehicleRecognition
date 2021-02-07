namespace VehicleRecognition.Shared.DTOs
{
    public class StatusResponse
    {
        public string Id { get; set; }
        public string StatusQueryGetUri { get; set; }
        public string SendEventPostUri { get; set; }
        public string TerminatePostUri { get; set; }
        public string PurgeHistoryDeleteUri { get; set; }
        public string ErrorMessage { get; set; }
    }
}