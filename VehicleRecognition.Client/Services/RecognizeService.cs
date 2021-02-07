using System;
using System.Text;
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading.Tasks;
using VehicleRecognition.Shared.DTOs;
using Microsoft.Extensions.Configuration;

namespace VehicleRecognition.Client.Services
{
    public class RecognizeService : IRecognizeService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;

        public RecognizeService(IConfiguration configuration, IHttpClientFactory clientFactory)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
        }

        public async Task<StatusResponse> Predict(string url)
        {
            try
            {
                var content = new StringContent(JsonConvert.SerializeObject(new { url }), Encoding.UTF8, "application/json");

                var response = await _clientFactory.CreateClient().PostAsync(_configuration["FunctionUri"], content);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var statusResponse = JsonConvert.DeserializeObject<StatusResponse>(json);
                    return statusResponse;
                }
            }
            catch (Exception e)
            {
                return new StatusResponse
                {
                    ErrorMessage = e.Message
                };
            }

            return new StatusResponse
            {
                ErrorMessage = "Something went wrong."
            };
        }

        public async Task<PredictionResult> GetPredictionResult(string url)
        {
            try
            {
                var response = await _clientFactory.CreateClient().GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<PredictionResult>(json);
                    switch (result.RuntimeStatus)
                    {
                        case "Completed":
                        case "Failed":
                            {
                                return result;
                            }
                        case "Pending":
                        case "Running":
                            {
                                await Task.Delay(2000);
                                return await GetPredictionResult(url);
                            }
                    }
                }
            }
            catch (Exception e)
            {
                // TODO
            }

            return null;
        }
    }
}