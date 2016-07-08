using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using ShoppingCartSampleCodes.Factory;
using ShoppingCartSampleCodes.Models.Cart;
using ShoppingCartSampleCodes.Models.Product;
using ShoppingCartSampleCodes.Utility;
using ShoppingCartSampleCodes.ViewModels;

namespace ShoppingCartSampleCodes.BO {

    public class CartRepo : ICartRepo
    {
        private IProductRepo productRepo;
        private readonly IDBContextFactory factory;
        private readonly String defaultConnection = SystemUtility.defaultConnectionString();
        public CartRepo(IProductRepo productRepo, IDBContextFactory factory)
        {
            this.productRepo = productRepo;
            this.factory = factory;
        }

        private IShoppingCartDBContext ResolveDBContextFactory()
        {
            return factory.createDbContext<IShoppingCartDBContext>(defaultConnection);
        }

        public async Task<Boolean> AddToCart(int cartId, Product product, int Quantity)
        {
            using (var db = ResolveDBContextFactory())
            {
                var cart = db.Carts.FirstOrDefault(o => o.Id == cartId);
                if (cart != null)
                {
                    var existingsaleitem =
                        await db.SaleItems.FirstOrDefaultAsync(o => o.CartId == cart.Id && o.ProductId == product.Id);
                    if (existingsaleitem != null)
                    {
                        existingsaleitem.Quantity += Quantity;
                    }
                    else
                    {
                        SaleItem item = new SaleItem() {
                            ProductId = product.Id,
                            Quantity = Quantity,
                            CartId = cartId,
                            Cart = cart
                        };
                        db.SaleItems.Add(item); 
                    }
                    await db.SaveChangesAsync();
                    return true;
                }
                throw  new Exception("no active shopping cart is found");
            }
        }

        public async Task<ICollection<SaleItemDataWrapper>> ShowShoppingCart(int cartId)
        {
            using (var db = ResolveDBContextFactory()) {
                var cart = db.Carts.FirstOrDefault(o => o.Id == cartId);
                if (cart != null)
                {
                    var lists = new List<SaleItemDataWrapper>();
                    foreach (var sale in cart.SaleItems)
                    {
                        var product = await productRepo.GetProduct(sale.ProductId);

                        SaleItemDataWrapper saleItem = new SaleItemDataWrapper
                        {
                            productname = product.Name,
                            basePrice = product.Baseprice,
                            Quanity =  sale.Quantity,
                        };
                        lists.Add(saleItem);
                    }
                    return lists;
                }
                throw new Exception("no active shopping cart is found");
            }
        }

        public async Task<int> CreateCart() {
            using (var db = ResolveDBContextFactory())
            {
                var cart = new Cart()
                {
                    CreateTime = DateTime.Now
                };
                db.Carts.Add(cart);
                await db.SaveChangesAsync();
                return cart.Id;
            }
        }

        public async Task removeCart(int cartid) {
            using (var db = ResolveDBContextFactory())
            {
                var cart = await db.Carts.FirstOrDefaultAsync(o => o.Id == cartid);
                db.Carts.Remove(cart);
                await db.SaveChangesAsync();
            }
        }
    }
}