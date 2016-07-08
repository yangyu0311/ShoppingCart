using System;
using System.Linq;
using Moq;
using ShoppingCartSampleCodes.BO;
using ShoppingCartSampleCodes.Factory;
using ShoppingCartSampleCodes.Models.Product;
using Xunit;

namespace ShoppingCartSampleCodes.Tests {
    public class ProductUnitTest
    {
        private IProductRepo productRepo;
        private readonly FakeProductDbContext fakeProductDbContext;

        public ProductUnitTest()
        {
            var factoryMock = new Mock<IDBContextFactory>();
            fakeProductDbContext = new FakeProductDbContext();
            productRepo = new ProductRepo(factoryMock.Object);
            factoryMock.Setup(o => o.createDbContext<IProductDbContext>(It.IsAny<String>()))
                .Returns(fakeProductDbContext);
            initData();
        }

        void initData()
        {
            var product = new Product()
            {
                Baseprice = 120.00m,
                Id = 1,
                Name = "Sony Television"
            };
            fakeProductDbContext.Products.Add(product);
        }

        [Theory]
        [InlineData(1,"Sony Television",120.00)]
        [InlineData(2, "Sony Television UHD", 2000.00)]
        
        public async void Test_GetProductMethod(int id, String name, Decimal priceDecimal)
        {
            var product = await productRepo.GetProduct(id);
            if (product != null)
            {
                Assert.True(product.Name.Equals(name));
                Assert.True(product.Baseprice == priceDecimal);    
            }
        }

        [Theory]
        [InlineData("Sony Television", 1200.00)]
        [InlineData("Microwave", 120.00)]   
        public async void Test_InsertProductMethod(String name, decimal price)
        {
            await productRepo.InsertProduct(name, price);
            var products = fakeProductDbContext.Products.ToArray(); 
            Assert.True(products.Length > 0);
        }

    }
}
