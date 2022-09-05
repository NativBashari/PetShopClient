using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PetShopClient.Models;
using System.Text;

namespace PetShopClient.Controllers
{
    //This is the animal controller. It responsible for all the operation in the Animal View.
    public class AnimalController : Controller
    {
        HttpClient client; // Get the HttpClient service from dependency injection.

        // Json serializer setting in order to ignore serialization of null objects inside a complex object. (like category in animal). 
        JsonSerializerSettings settings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };

        public AnimalController(HttpClient client)
        {
            this.client = client;
        }
        public async Task<IActionResult> Index(int id)
        {
            HttpResponseMessage message = await client.GetAsync("http://localhost:5080/api/animals/" + id); // Send request to get the specific animal.
            if (message.IsSuccessStatusCode)
            {
                var jstring = await message.Content.ReadAsStringAsync();
                var animal = JsonConvert.DeserializeObject<Animal>(jstring);
                //var comments = animal!.Comments;
                return View(animal);
            }
            else
                return View(null);
        }
        [HttpPost]
        public async Task<IActionResult> PostComment(Comment comment) //Post comment for this animal.
        {
            if (ModelState.IsValid)
            {
                var Json = JsonConvert.SerializeObject(comment, settings);
                StringContent content = new StringContent(Json, Encoding.UTF8, "application/json");
                HttpResponseMessage message = await client.PostAsync("http://localhost:5080/api/comments", content);
                if (message.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", new { id = comment.AnimalId });
                }
                else
                    return View(comment);
            }
            else // Any validation errors.
                return View(comment);
        }
    }
}
