using System;
using System.Threading.Tasks;
using ShoppingCartSampleCodes.Models.Product;

namespace ShoppingCartSampleCodes.BO {
    public interface IProductRepo
    {
       Task<Product> GetProduct(int id);
        Task<int> InsertProduct(string name, Decimal price);
    }
}
