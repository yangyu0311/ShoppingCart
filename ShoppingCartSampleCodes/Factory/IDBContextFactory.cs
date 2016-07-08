using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingCartSampleCodes.Factory {
    public interface IDBContextFactory {
        T createDbContext<T>(string connection);
    }
}
