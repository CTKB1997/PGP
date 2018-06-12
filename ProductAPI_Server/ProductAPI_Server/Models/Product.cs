using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProductAPI_Server.Models
{
    public class Product
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public float UnitPrice { get; set; }
        public int Quantity { get; set; }
    }
}