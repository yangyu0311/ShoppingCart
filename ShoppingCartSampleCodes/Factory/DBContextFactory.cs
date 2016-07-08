using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Practices.Unity;

namespace ShoppingCartSampleCodes.Factory {
    public class DBContextFactory : IDBContextFactory {

        private readonly IUnityContainer unityContainer;

        public DBContextFactory(IUnityContainer unityContainer) {
            this.unityContainer = unityContainer;
        }

        public T createDbContext<T>(string connection) {
            return unityContainer.Resolve<T>(new ParameterOverride("connectionString", connection));
        }
    }
}