using Azure.Storage.Blobs;
using Azure_Blob_Sorage_Practice.Models;

namespace Azure_Blob_Sorage_Practice.Services
{
    public class ContainerServices
    {
        private BlobServiceClient _client;
        public ContainerServices(BlobServiceClient client)
        {
            _client = client;
        }

        public async Task<List<string>> GetAllContainers()
        {
            List<string> res = new();
            var containers = _client.GetBlobContainersAsync();
            await foreach (var container in containers)
            {
                res.Add(container.Name);
            }
            return res;
        }

        public async Task<bool> containerExist(string containerName)
        {
            bool res = false;
            var containerClient = _client.GetBlobContainerClient(containerName);

            res = await containerClient.ExistsAsync();
            return res;
        }

        public async Task createContainer(string containerName)
        {
            var containerClient = _client.GetBlobContainerClient(containerName);

            containerClient.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
        }

        public async Task deleteContainer(string containerName)
        {
            var containerClient = _client.GetBlobContainerClient(containerName);

            containerClient.DeleteIfExistsAsync();
        }
        
    }

}
