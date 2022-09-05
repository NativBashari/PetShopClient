using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PetShopClient.Models;
using System.Text;

namespace PetShopClient.Controllers
{
    //This is the adminisrator contoller, it responsible for all the operations in the Administrator page.
    public class AdminstratorController : Controller
    {
        HttpClient client; // Get http client service from dependency injection.
        static List<Category> Categories = new List<Category>(); // Global and static list of categories, because I use this in all the actions.
        static Animal? _animal; // global and static Animal object, in order to get his comments. 

        // Json serializer setting in order to ignore serialization of null objects inside a complex object. (like category in animal), beacuse I need only Foreign-Keys.  
        JsonSerializerSettings settings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };
        public AdminstratorController(HttpClient client)
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
                Categories = categories!;
                ViewBag.Items = categories;
            }

            // Check if the user select a specific category. 
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
            return View(null); //TODO : return initialized list
        }
        public async Task<IActionResult> Update(int id)
        {
           
            HttpResponseMessage message = await client.GetAsync("http://localhost:5080/api/animals/" + id); //Send request to get the specific animal bt Id. 
            if (message.IsSuccessStatusCode)
            {
                var jstring = await message.Content.ReadAsStringAsync();
                var animal = JsonConvert.DeserializeObject<Animal>(jstring);
                ViewBag.Items = Categories;
                return View(animal); // Send the animal to Update view.
            }
            else return View(null); //TODO : Redirect to action : Add;
        }
        [HttpPost]
        public async Task<IActionResult> Update(Animal animal) // Get animal from update view by the form methos Post. 
        {    
            ViewBag.Items = Categories;
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(animal, settings);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage message = await client.PutAsync("http://localhost:5080/api/animals", content); // send request to update the animal.
                if (message.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index"); 
                }
                return View(animal);
            }
            else
                return View(animal);
        }

        public IActionResult AddAnimal()
        {
            ViewBag.Items = Categories;
            Animal animal = new Animal(); // Initialize new Animal object.
            return View(animal); // send the empty animal to AddAnimal view.
        }
        [HttpPost]
        public async Task<IActionResult> AddAnimal(Animal animal)// Get animal from AddAnimal view.
        {
            ViewBag.Items = Categories;
            if (ModelState.IsValid)
            {
                var jstring = JsonConvert.SerializeObject(animal,settings);
                StringContent content = new StringContent(jstring, Encoding.UTF8, "application/json");
                HttpResponseMessage message = await client.PostAsync("http://localhost:5080/api/animals", content); // send request to post the animal.
                if (!message.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else //If the request failed at the server level
                {
                    ModelState.AddModelError("Error", "This is an API error.");
                    return View(animal);
                }

            }
            else // If there are validation errors.
                return View(animal);
        }
        public async Task<IActionResult> GetComments(int id) //Get the comments by the animal Id.
        {
            HttpResponseMessage message = await client.GetAsync("http://localhost:5080/api/animals/" + id); // Send request to get animal by Id,
            if (message.IsSuccessStatusCode)
            {
                var jstring = await message.Content.ReadAsStringAsync();
                var animal = JsonConvert.DeserializeObject<Animal>(jstring);
                _animal = animal!; // I save it global because I need to use this animalId in other action.
                ViewBag.Animal = animal;
                ViewBag.Comment = new Comment() { }; 
                var comments = animal!.Comments; // Extricate the comments list that inside the animal entity.
                return View(animal.Comments); // return the comments view for this animal.
            }
            else
                return View(null);
        }
        public async Task<IActionResult> DeleteComment(int id)
        {
            HttpResponseMessage message = await client.DeleteAsync("http://localhost:5080/api/comments/" + id); // send request to delete comment by Id.
            if (message.IsSuccessStatusCode)
            {
                return RedirectToAction("GetComments", new { id = _animal!.AnimalId}); // redirect to comment view for this animal.
            }
            return View();
        }
        public async Task<IActionResult> DeleteAnimal(int id)
        {
            HttpResponseMessage message = await client.DeleteAsync("http://localhost:5080/api/animals/" + id); // send request to delete animal by Id.
            if (message.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
