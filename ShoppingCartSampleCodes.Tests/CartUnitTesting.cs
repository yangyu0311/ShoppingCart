using System;
using System.Linq;
using Moq;
using ShoppingCartSampleCodes.BO;
using ShoppingCartSampleCodes.Factory;
using ShoppingCartSampleCodes.Models.Cart;
using ShoppingCartSampleCodes.Models.Product;
using Xunit;


namespace ShoppingCartSampleCodes.Tests {
    public class CartUnitTesting {

        private ICartRepo cartRepo;
        private Mock<IProductRepo> prodcutRepoMock;
        private Mock<IDBContextFactory> dbFactoryMock;
        private readonly FakeShoppingCartDBContext fakeShoppingCartDbContext;
        
        public CartUnitTesting()
        {
            dbFactoryMock = new Mock<IDBContextFactory>();
            fakeShoppingCartDbContext = new FakeShoppingCartDBContext();
            prodcutRepoMock = new Mock<IProductRepo>();
            cartRepo = new CartRepo(prodcutRepoMock.Object, dbFactoryMock.Object );
            dbFactoryMock.Setup(o => o.createDbContext<IShoppingCartDBContext>(It.IsAny<String>()))
                .Returns(fakeShoppingCartDbContext);
        }

        void initCartData()
        {
            var cart = new Cart()
            {
                CreateTime = DateTime.Now,
                Id = 1,
            };
            SaleItem saleItem = new SaleItem()
            {
                Cart = cart,
                CartId = 1,
                Id = 1,
                ProductId = 1,
                Quantity = 10
            };
            SaleItem saleItem2 = new SaleItem()
            {
                Cart = cart,
                CartId = 1,
                Id = 2,
                ProductId = 2,
                Quantity = 10
            };
            cart.SaleItems.Add(saleItem);
            cart.SaleItems.Add(saleItem2);
            fakeShoppingCartDbContext.Carts.Add(cart);
            fakeShoppingCartDbContext.SaleItems.Add(saleItem);
            fakeShoppingCartDbContext.SaleItems.Add(saleItem2);
        }

        [Fact]
        public void Test_CreateCart()
        {
            var result = cartRepo.CreateCart();
            Assert.True(fakeShoppingCartDbContext.Carts.ToArray().Length > 0);
        }

        [Fact]
        public async void Test_AddtoCart()
        {
            initCartData();
            var product = new Product()
            {
                Id = 1,
                Name = "TV",
                Baseprice = 1200.00m
            };

            var product2 = new Product() {
                Id = 2,
                Name = "iPad Pro",
                Baseprice = 120.00m
            };

            var isSucess = await cartRepo.AddToCart(1, product, 10);
            isSucess = await cartRepo.AddToCart(1, product, 20);
            isSucess = await cartRepo.AddToCart(1, product2, 10);
            Assert.True(isSucess);
            Assert.True(fakeShoppingCartDbContext.SaleItems.ToArray().Length == 2);
        }

        [Fact]
        public async void Test_ShowCart()
        {
            initCartData();
            var product = new Product() {
                Id = 1,
                Name = "TV",
                Baseprice = 1200.00m
            };

            var product2 = new Product() {
                Id = 2,
                Name = "iPad Pro",
                Baseprice = 120.00m
            };
            prodcutRepoMock.Setup(o => o.GetProduct(1)).ReturnsAsync(product);
            prodcutRepoMock.Setup(o => o.GetProduct(2)).ReturnsAsync(product2);
            var lists = await cartRepo.ShowShoppingCart(1);
            Assert.True(lists.Count == 2);
            Assert.True(lists.ElementAt(0).Quanity == 10);
            Assert.True(lists.ElementAt(0).productname == "TV");

        }
    }
}
