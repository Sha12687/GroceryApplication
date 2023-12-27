using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodDeliveryApplicationUI.Models
{
    public class OrderDetailsViewModel
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Subtotal { get; set; }
        public int ProductId { get; set; }
    }
}