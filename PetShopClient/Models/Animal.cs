using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PetShopClient.Models
{
    public class Animal
    {
        public int AnimalId { get; set; }
        [Required(ErrorMessage ="Please enter name.")]
        public string? Name { get; set; }
        [Range(1,100,ErrorMessage = "Please enter age.")]
        public int Age { get; set; }
        [Required(ErrorMessage = "Please enter picture Url.")]
        public string? PictureUrl { get; set; }
        [Required(ErrorMessage = "Please enter Description.")]
        public string? Description { get; set; }
        [Range(1,100,ErrorMessage = "Please enter Category.")]
        public int CategoryId { get; set; }

        [JsonIgnore]
        public  virtual Category? Category { get; set; }
        public virtual ICollection<Comment>? Comments { get; set; }
    }
}
