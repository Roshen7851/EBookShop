using System.ComponentModel.DataAnnotations;
namespace BookShopManagementSystem.Models
{
    public class Book
    {
        [Required]
        public int BookId { get; set; }
        [Required]
        public string Title { get; set; } = "";
        [Required]
        public string Author { get; set; } = "";
        [Required]
        public string Description { get; set; } = "";
        [Required]
        public int AvailableQuantity { get; set; }
        [Required]
        public string BookCategory { get; set; } = "";
        public int ISBN { get; set; }
        [Required]
        public double Price { get; set; }
        [Required]
        public string ImagePath { get; set; } = "";
        public DateTime PublishDate { get; set; }
    }
}
