using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Routing;
using Microsoft.Practices.Unity;

namespace ShoppingCartSampleCodes
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            GlobalConfiguration.Configure(WebApiConfig.Register);

            //create the Unity Container object for Dependency Injection
            var container = new UnityContainer();
            GlobalConfiguration.Configuration.DependencyResolver = new UnityResolver(container);

        }
    }
}
