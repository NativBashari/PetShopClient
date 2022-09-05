using System.ComponentModel.DataAnnotations;

namespace PetShopClient.Models
{
    public class Comment
    {
        public int CommentId { get; set; }
        [Required(ErrorMessage = "Please enter your comment.")]
        public string? CommentText { get; set; }
        public int AnimalId { get; set; }
        public Animal? Animal { get; set; }
    }
}
