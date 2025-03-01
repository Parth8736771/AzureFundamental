
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;
using Azure.Storage.Sas;
using AzureBlobProject.Models;

namespace AzureBlobProject.Services
{
    public class BlobService : IBlobService
    {
        private readonly BlobServiceClient _blobServiceClient;
        public BlobService(BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
        }
        

        public async Task<bool> DeleteBlob(string name, string containerName)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            BlobClient blobClient = containerClient.GetBlobClient(name);
            return await blobClient.DeleteIfExistsAsync();
        }

        public async Task<List<string>> GetAllBlobs(string containerName)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            var blobsObj = containerClient.GetBlobsAsync();
            List<string> blobs = new();

            await foreach(var item in blobsObj)
            {
                blobs.Add(item.Name);
            }
            return blobs;
        }

        public async Task<List<Blob>> GetAllBlobsWithUri(string containerName)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

   //         string sasContainerSignature = "";

			//if (containerClient.CanGenerateSasUri)
			//{
			//	BlobSasBuilder sasBuilder = new()
			//	{
   //                 BlobContainerName = containerClient.Name,
			//		Resource = "c",
			//		ExpiresOn = DateTime.UtcNow.AddHours(1),
			//	};
			//	sasBuilder.SetPermissions(BlobAccountSasPermissions.Read | BlobAccountSasPermissions.Write);

			//	sasContainerSignature = containerClient.GenerateSasUri(sasBuilder).AbsoluteUri.Split("?")[1].ToString();
			//}

			var blobsObj = containerClient.GetBlobsAsync();
            List<Blob> blobs = new();

            await foreach (var item in blobsObj)
            {
                var blobClient = containerClient.GetBlobClient(item.Name);
                Blob blobIndividual = new()
                {
                    Uri = blobClient.Uri.AbsoluteUri  // SAS Blob
                    //Uri = blobClient.Uri.AbsoluteUri + "?" + sasContainerSignature // SAS Container
				};

                if (blobClient.CanGenerateSasUri)
                {
                    BlobSasBuilder sasBuilder = new()
                    {
                        BlobContainerName = blobClient.GetParentBlobContainerClient().Name,
                        BlobName = blobClient.Name,
                        Resource = "b",
                        ExpiresOn = DateTime.UtcNow.AddHours(1),
                    };
                    sasBuilder.SetPermissions(BlobAccountSasPermissions.Read | BlobAccountSasPermissions.Write);

                    blobIndividual.Uri = blobClient.GenerateSasUri(sasBuilder).AbsoluteUri;
;               }

                BlobProperties blobProperties = await blobClient.GetPropertiesAsync();
                if (blobProperties.Metadata.ContainsKey("title"))
                {
                    blobIndividual.Title = blobProperties.Metadata["title"];
                }
                if (blobProperties.Metadata.ContainsKey("comment"))
                {
                    blobIndividual.Comment = blobProperties.Metadata["comment"];
                }
                blobs.Add(blobIndividual);
            }
            return blobs;
        }

        public async Task<string> GetBlob(string name, string containerName)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            BlobClient blobClient = containerClient.GetBlobClient(name);
            return blobClient.Uri.AbsoluteUri;
        }

        public async Task<bool> UploadBlob(string name, IFormFile file, string containerName, Blob blob)
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(containerName);

            BlobClient blobClient = containerClient.GetBlobClient(name);
            var httpHeader = new BlobHttpHeaders()
            {
                ContentType = file.ContentType,
            };

            IDictionary<string, string> metaData = new Dictionary<string, string>();
            metaData.Add("title", blob.Title);
            metaData["comment"] = blob.Comment;

            var result = blobClient.UploadAsync(file.OpenReadStream(), httpHeader, metaData);
            if (result != null)
            {
                return true;
            }
            return false;
        }
    }
}
