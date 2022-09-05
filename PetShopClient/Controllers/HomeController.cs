using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PetShopClient.Models;
using System.Diagnostics;

namespace PetShopClient.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        HttpClient client; // Get this service from dependency injection.

        public HomeController(ILogger<HomeController> logger, HttpClient client)
        {
            _logger = logger;
            this.client = client;
        }

        public async Task<IActionResult> Index()
        {
            HttpResponseMessage message = await client.GetAsync("http://localhost:5080/api/Home"); // Send request to get the 2 top commented animals.
            if (message.IsSuccessStatusCode)
            {
                var jstring = await message.Content.ReadAsStringAsync();
                var topAnimals = JsonConvert.DeserializeObject<List<Animal>>(jstring);
                return View(topAnimals);
            }
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}