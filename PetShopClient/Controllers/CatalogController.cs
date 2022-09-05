using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PetShopClient.Models;
using System.Text;

namespace PetShopClient.Controllers
{
    public class CatalogController : Controller
    {
        HttpClient client; //Get this service from dependency injection.


        // Json serializer setting in order to ignore serialization of null objects inside a complex object. (like category in animal).  
        JsonSerializerSettings settings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
        public CatalogController(HttpClient client)
        {
            this.client = client;
        }
        public async Task<IActionResult> Index(int id)
        {
            //Get all categories in order to show them in the select element.
            client = new HttpClient();
            HttpResponseMessage message = await client.GetAsync("http://localhost:5080/api/categories");
            if (message.IsSuccessStatusCode)
            {
                var jstring = await message.Content.ReadAsStringAsync();
                var categories = JsonConvert.DeserializeObject<List<Category>>(jstring);
                ViewBag.Items = categories;
            }
            // Check if the user select specific category. 
            if (id == 0) // It said that the user is not select any category, he want to see all animals from all categories.
            {

                message = await client.GetAsync("http://localhost:5080/api/animals"); 
                if (message.IsSuccessStatusCode)
                {
                    var jstring = await message.Content.ReadAsStringAsync();
                    var animals = JsonConvert.DeserializeObject<List<Animal>>(jstring);
                    return View(animals);
                }
            }
            // if user select a specicific category.
            else
            {
                message = await client.GetAsync("http://localhost:5080/api/categories/" + id); // send request to get this category.
                if (message.IsSuccessStatusCode)
                {
                    var jstring = await message.Content.ReadAsStringAsync();
                    var category = JsonConvert.DeserializeObject<Category>(jstring);
                    var animals = category!.Animals; // Extricate the animals list that inside the category entity. 
                    return View(animals);
                }
            }
            return View(null);
        }

      
    }
}
