using System.ComponentModel.DataAnnotations;

namespace PetShopClient.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        [Required(ErrorMessage = "Please enter name.")]
        public string? Name { get; set; }
        public ICollection<Animal>? Animals { get; set; }
    }
}
