// OrderDetail.cs

using System.ComponentModel.DataAnnotations;

namespace BookShopManagementSystem.Models
{
    public class OrderDetail
    {
        public int OrderDetailId { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int BookId { get; set; }

        [Required]
        public int Quantity { get; set; }

        // Navigation properties
        public Order Order { get; set; }
        public Book Book { get; set; }
    }
}
