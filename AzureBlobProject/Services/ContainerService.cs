
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace AzureBlobProject.Services
{
    public class ContainerService : IContainerService
    {
        private readonly BlobServiceClient _blobServiceClient;
        public ContainerService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }
        public async Task CreateContainer(string containerName)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);
        }

        public async Task DeleteContainer(string containerName)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
            await containerClient.DeleteIfExistsAsync();
        }

        public async Task<List<string>> GetAllContainer()
        {
            List<string> containerList = new();

            await foreach(BlobContainerItem blobContainerItem in _blobServiceClient.GetBlobContainersAsync())
            {
                containerList.Add(blobContainerItem.Name);
            }
            return containerList;
        }

        public async Task<List<string>> GetAllContainerAndBlobs()
        {
            List<string> containerAndBlobNames = new List<string>();
            containerAndBlobNames.Add(" Account Name : " + _blobServiceClient.AccountName);
            containerAndBlobNames.Add("================================================");
            await foreach (BlobContainerItem blobContainerItem in _blobServiceClient.GetBlobContainersAsync())
            {
                containerAndBlobNames.Add("--->  " + blobContainerItem.Name);
                BlobContainerClient blobContainerClient = _blobServiceClient.GetBlobContainerClient(blobContainerItem.Name);
                await foreach (BlobItem blobItem in blobContainerClient.GetBlobsAsync())
                {
                    BlobClient blobClient = blobContainerClient.GetBlobClient(blobItem.Name);

                    BlobProperties blobProperties = await blobClient.GetPropertiesAsync();

                    string blobToAdd = blobItem.Name;
                    if (blobProperties.Metadata.ContainsKey("title"))
                    {
                        blobToAdd += "  ( " + blobProperties.Metadata["title"] + " ) ";
                    }
                    containerAndBlobNames.Add("----------->  " + blobToAdd);
                }
                containerAndBlobNames.Add("================================================");
            }
            return containerAndBlobNames;
        }
    }
}
