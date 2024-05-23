// Order.cs

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookShopManagementSystem.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }

        [Required]
        public DateTime OrderDate { get; set; }

        [Required]
        public string Status { get; set; } // "pending", "accepted", "rejected", etc.

        // Navigation property for OrderDetails
        public ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
