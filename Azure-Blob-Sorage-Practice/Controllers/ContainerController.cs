using Azure.Storage.Blobs;
using Azure_Blob_Sorage_Practice.Services;
using Microsoft.AspNetCore.Mvc;

namespace Azure_Blob_Sorage_Practice.Controllers
{
    public class ContainerController : Controller

    {
        private ContainerServices _containerServices;
        private BlobServiceClient _client;
        public ContainerController(BlobServiceClient client)
        {
            _client = client;
            _containerServices = new ContainerServices(_client);
        }
        public async Task<IActionResult> Index()
        {
            var res = await _containerServices.GetAllContainers();
            return View(res);
        }
    }
}
