using System;
using System.Linq;
using System.Transactions;
using Moq;
using ShoppingCartSampleCodes.BO;
using ShoppingCartSampleCodes.Factory;
using ShoppingCartSampleCodes.Models.Cart;
using ShoppingCartSampleCodes.Models.Product;
using ShoppingCartSampleCodes.Utility;
using Xunit;

namespace ShoppingCartSampleCodes.Tests {
    public class CartUnitTestingDB : IDisposable {
        private ICartRepo cartRepo;
        private Mock<IProductRepo> prodcutRepoMock;
        private Mock<IDBContextFactory> dbFactoryMock;
        private readonly IShoppingCartDBContext shoppingCartDbContext;
        private String ConnectionString = SystemUtility.defaultConnectionString();
        private readonly TransactionScope scope;

        public CartUnitTestingDB()
        {

            scope = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions {IsolationLevel = IsolationLevel.ReadCommitted},
                TransactionScopeAsyncFlowOption.Enabled);
            dbFactoryMock = new Mock<IDBContextFactory>();
            shoppingCartDbContext = new ShoppingCartDBContext(ConnectionString);
            prodcutRepoMock = new Mock<IProductRepo>();
            cartRepo = new CartRepo(prodcutRepoMock.Object, dbFactoryMock.Object);
            dbFactoryMock.Setup(o => o.createDbContext<IShoppingCartDBContext>(It.IsAny<String>()))
                .Returns(shoppingCartDbContext);
        }

        public void Dispose()
        {
            scope.Dispose();
        }

        [Fact]
        public async void Test_CreateCart()
        {
            var newcart = await cartRepo.CreateCart();
            Assert.True(newcart > 0);
        }


        void InitData()
        {
            //Init Cart data
            Cart cart = new Cart()
            {
                CreateTime = DateTime.Now,
                Id = 1
            };
            shoppingCartDbContext.Carts.Add(cart);
            //Init Sale Item
            SaleItem sale = new SaleItem()
            {
                Cart = cart,
                CartId = 1,
                ProductId = 1,
                Id = 1,
                Quantity = 10
            };
            shoppingCartDbContext.SaleItems.Add(sale);
            shoppingCartDbContext.SaveChanges();
        }

        [Fact]
        public async void Test_AddtoCart()
        {
            InitData();
            var product = new Product() {
                Id = 1,
                Name = "TV",
                Baseprice = 1200.00m
            };
            var carts = shoppingCartDbContext.Carts.ToArray();
            Assert.True(carts.Length > 0);

            var cartid = carts.ElementAt(0).Id;
            var result = await cartRepo.AddToCart(cartid, product, 10);
            Assert.True(result);
            var context = new ShoppingCartDBContext(ConnectionString);
            var firstOrDefault = context.SaleItems.FirstOrDefault();
            Assert.True(firstOrDefault != null && firstOrDefault.Quantity == 20);
        }

        [Fact]
        public async void Test_ShowCart() {
            InitData();
            var product = new Product() {
                Id = 1,
                Name = "TV",
                Baseprice = 1200.00m
            };
            prodcutRepoMock.Setup(O => O.GetProduct(1)).ReturnsAsync(product);
            var carts = shoppingCartDbContext.Carts.ToArray();
            Assert.True(carts.Length > 0);
            var cartid = carts.ElementAt(0).Id;
            var result = await cartRepo.ShowShoppingCart(cartid);
            Assert.True(result.Count == 1);
            Assert.True(result.ElementAt(0).Quanity == 10);
            Assert.True(result.ElementAt(0).productname.Equals("TV"));
            Assert.True(result.ElementAt(0).Subtotal == 12000);     
        }
    }
}
