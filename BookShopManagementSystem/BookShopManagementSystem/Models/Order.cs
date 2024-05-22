using System.ComponentModel.DataAnnotations;

namespace BookShopManagementSystem.Models
{
    public class Order
    {
        [Required]
        public int OrderId { get; set; }

        [Required]
        public int BookId { get; set; }

        public Book Book { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public string OrderStatus { get; set; } = "Pending";

        [Required]
        public int Quantity { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }
    }
}
