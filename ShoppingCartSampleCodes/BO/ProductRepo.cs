using System;
using System.Data.Entity;
using System.Threading.Tasks;
using ShoppingCartSampleCodes.Factory;
using ShoppingCartSampleCodes.Models.Product;
using ShoppingCartSampleCodes.Utility;

namespace ShoppingCartSampleCodes.BO {

    public class ProductRepo : IProductRepo
    {
        private IDBContextFactory factory;
        private String defaultconnection = SystemUtility.defaultConnectionString();
        
        public ProductRepo(IDBContextFactory factory)
        {
            this.factory = factory;
        }

        public IProductDbContext resolveProductDbContext()
        {
            return factory.createDbContext<IProductDbContext>(defaultconnection);
        }

        public async Task<Product> GetProduct(int id)
        {
            using (var db = resolveProductDbContext())
            {
                return await db.Products.FirstOrDefaultAsync(o => o.Id == id);
            }
        }

        public async Task<int> InsertProduct(string name, Decimal price)
        {
            using (var db = resolveProductDbContext())
            {
                var p = new Product()
                {
                    Baseprice = price,
                    Name = name
                };
                db.Products.Add(p);
                await db.SaveChangesAsync();
                return p.Id;
            }
        }
    }
}