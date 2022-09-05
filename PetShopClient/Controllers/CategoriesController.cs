using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PetShopClient.Models;
using System.Text;

namespace PetShopClient.Controllers
{
    public class CategoriesController : Controller
    {
        HttpClient client;// Get this service from dependency injection.


        // Json serializer setting in order to ignore serialization of null objects inside a complex object. (like category in animal).  
        JsonSerializerSettings settings = new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore };

        public CategoriesController(HttpClient client)
        {
            this.client = client;
        }
        public async Task<IActionResult> Index()
        {
            HttpResponseMessage message = await client.GetAsync("http://localhost:5080/api/categories"); // Send request to get all categories.
            if (message.IsSuccessStatusCode)
            {
                var jstring = await message.Content.ReadAsStringAsync();
                var categories = JsonConvert.DeserializeObject<List<Category>>(jstring);
                return View(categories);
            }
            return View();
        }
        public IActionResult AddCategory()
        {
            Category category = new Category();// Initialize new and empty catrgory object.
            return View (category);// Send it to AddCategory View.

        }
        [HttpPost]
        public async Task<IActionResult> AddCategory(Category category)// Get category from Add Category View.
        {
            if (ModelState.IsValid)// Check validation.
            {
                var jstring = JsonConvert.SerializeObject(category, settings);
                var content = new StringContent(jstring, Encoding.UTF8, "application/json");
                HttpResponseMessage message = await client.PostAsync("http://localhost:5080/api/categories", content);// send request to post the category.
                if (message.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                else //If the request failed at the server level
                {
                    ModelState.AddModelError("Error", "This is an API error.");
                    return View(category);
                }
            }
            else //Any validation errors.
                return View(category);
        }
        public async Task<IActionResult> Update(int id)
        {
            HttpResponseMessage message = await client.GetAsync("http://localhost:5080/api/categories/" + id); // Send request to get specific category by Id.
            if (message.IsSuccessStatusCode)
            {
                var jstring = await message.Content.ReadAsStringAsync();
                var Category = JsonConvert.DeserializeObject<Category>(jstring);
                return View(Category); // Return the UpdateCategory view with the selected category.
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Update(Category category)// Get the updated category from UpdateCategory view.
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(category);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage message = await client.PutAsync("http://localhost:5080/api/categories", content); // Send request to update the category.
                if (message.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                return View(category);
            }
            else // Any validation errors.
                return View(category);
        }
        public async Task<IActionResult> Delete(int id)
        {
            HttpResponseMessage message = await client.DeleteAsync("http://localhost:5080/api/categories/" + id); // send request to delete category by Id.
            if (message.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
