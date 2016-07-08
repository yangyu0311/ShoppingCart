using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingCartSampleCodes.ViewModels {
    public class SaleItemDataWrapper
    {
        public String productname { get; set; }
        public Decimal basePrice { get; set; }
        public int Quanity { get; set; }
        public Decimal Subtotal
        {
            get { return basePrice*Quanity; }
        }


    }
}