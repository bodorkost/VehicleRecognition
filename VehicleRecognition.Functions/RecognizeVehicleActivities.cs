using System;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using VehicleRecognition.Functions.ML;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage;

namespace VehicleRecognition.Functions
{
    public class RecognizeVehicleActivities
    {
        private readonly ITFModelScorer _tfModelScorer;

        public RecognizeVehicleActivities(ITFModelScorer tfModelScorer)
        {
            _tfModelScorer = tfModelScorer;
        }

        [FunctionName("A_TrainModel")]
        public async Task TrainModel([ActivityTrigger] string input, ILogger log)
        {
            log.LogInformation("Training model");

            await _tfModelScorer.Train();
        }

        [FunctionName("A_TestModel")]
        public async Task TestModel([ActivityTrigger] string input, ILogger log)
        {
            log.LogInformation("Testing model");

            await _tfModelScorer.Test();
        }

        [FunctionName("A_PredictImage")]
        public async Task<string> PredictImage([ActivityTrigger] string input, ILogger log)
        {
            log.LogInformation("Predicting image");

            return await _tfModelScorer.ClassifySingleImage(input);
        }

        [FunctionName("A_UploadImage")]
        public async Task<string> UploadImage([ActivityTrigger] string input, ILogger log)
        {
            log.LogInformation("Uploading image");

            string storageConnection = "DefaultEndpointsProtocol=https;AccountName=tbodorkosrecognition;AccountKey=7Oo2Fx5cApfdONcoeouqEAs5qivciYUNZYlf3sjoOGo30MUdtqgz8MiXw6e2+wtaAlYrpa8pwxxWk1HzN4vADg==;EndpointSuffix=core.windows.net";
            var storageAccount = CloudStorageAccount.Parse(storageConnection);
            var blobClient = storageAccount.CreateCloudBlobClient();
            var blobContainer = blobClient.GetContainerReference("prediction-container");

            if (await blobContainer.CreateIfNotExistsAsync())
            {
                await blobContainer.SetPermissionsAsync(new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Container });
            }

            var imageName = "prediction.jpg";
            var imageResponse = await WebRequest.Create(input).GetResponseAsync();
            var stream = imageResponse.GetResponseStream();

            var blob = blobContainer.GetBlockBlobReference(imageName); 
            blob.Properties.ContentType = "image/jpg";

            await blob.UploadFromStreamAsync(stream);

            return blob.Uri?.AbsoluteUri;
        }

        [FunctionName("A_RemoveImage")]
        public void RemoveImage([ActivityTrigger] string input, ILogger log)
        {
            log.LogInformation("Removing image");

            try
            {
                if (File.Exists(input))
                {
                    File.Delete(input);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        [FunctionName("A_SaveImage")]
        public async Task<string> SaveImage([ActivityTrigger] string input, ILogger log)
        {
            log.LogInformation("Saving image");

            string saveLocation = @Path.Combine(Environment.CurrentDirectory, "assets", "images", "prediction.jpg");

            byte[] imageBytes;
            var imageResponse = await WebRequest.Create(input).GetResponseAsync();
            var responseStream = imageResponse.GetResponseStream();

            using (var binaryReader = new BinaryReader(responseStream))
            {
                imageBytes = binaryReader.ReadBytes(500000);
                binaryReader.Close();
            }
            responseStream.Close();
            imageResponse.Close();

            var fileStream = new FileStream(saveLocation, FileMode.Create);
            var binaryWriter = new BinaryWriter(fileStream);
            try
            {
                binaryWriter.Write(imageBytes);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                fileStream.Close();
                binaryWriter.Close();
            }

            return saveLocation;
        }

        [FunctionName("A_DeleteImage")]
        public void DeleteImage([ActivityTrigger] string input, ILogger log)
        {
            log.LogInformation("Deleting image");

            try
            {
                if (File.Exists(input))
                {
                    File.Delete(input);
                }
            }
            catch (IOException e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
