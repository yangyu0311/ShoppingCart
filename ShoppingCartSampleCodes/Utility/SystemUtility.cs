using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace ShoppingCartSampleCodes.Utility {
    public static class SystemUtility {

        public static String defaultConnectionString()
        {
            return ConfigurationManager.ConnectionStrings["ShoppingCart"].ConnectionString;
        }
    }
}