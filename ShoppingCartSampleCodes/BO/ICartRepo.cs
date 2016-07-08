using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ShoppingCartSampleCodes.Models.Product;
using ShoppingCartSampleCodes.Models.Cart;
using ShoppingCartSampleCodes.ViewModels;


namespace ShoppingCartSampleCodes.BO {
    public interface ICartRepo
    {
        Task<int> CreateCart();
        Task<Boolean> AddToCart(int cartId, Product product, int Quantity);
        Task<ICollection<SaleItemDataWrapper>> ShowShoppingCart(int cartId);
        Task removeCart(int cartid);
    }
}
