using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.Unity;
using ShoppingCartSampleCodes.BO;
using ShoppingCartSampleCodes.Factory;
using ShoppingCartSampleCodes.Models.Cart;
using ShoppingCartSampleCodes.Models.Product;

namespace ShoppingCartSampleCodes.BootStrap {
    public class ContainerBootstrapper {
        
        public static void RegisterTypes(IUnityContainer container)
        {
            //DB Factory DI, Singleton life cycle through out the application.  
            container.RegisterType<IDBContextFactory, DBContextFactory>(new ContainerControlledLifetimeManager(), new InjectionConstructor(container));
            container.RegisterType<IShoppingCartDBContext>(new InjectionConstructor(typeof(String)));
            container.RegisterType<IProductDbContext>(new InjectionConstructor(typeof(String)));

            var factory = container.Resolve<IDBContextFactory>();
            container.RegisterType<IProductRepo>(new InjectionConstructor(factory));
            container.RegisterType<IShoppingCartDBContext>(new InjectionConstructor(typeof(IProductRepo), factory));
        }
    }
}