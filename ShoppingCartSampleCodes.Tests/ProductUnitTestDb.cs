using System;
using System.Transactions;
using Moq;
using ShoppingCartSampleCodes.BO;
using ShoppingCartSampleCodes.Factory;
using ShoppingCartSampleCodes.Models.Product;
using ShoppingCartSampleCodes.Utility;
using Xunit;

namespace ShoppingCartSampleCodes.Tests {
    public class ProductUnitTestDb : IDisposable {

        private IProductRepo productRepo;
        private String ConnectionString = SystemUtility.defaultConnectionString();
        private Mock<IDBContextFactory> factoryMock;
        private ProductDbContext productDbContext;
        private readonly TransactionScope scope;

        public ProductUnitTestDb()
        {
            scope = new TransactionScope(TransactionScopeOption.Required,
              new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
              TransactionScopeAsyncFlowOption.Enabled);
          
            productDbContext = new ProductDbContext(ConnectionString); 
            factoryMock = new Mock<IDBContextFactory>();
            productRepo = new ProductRepo(factoryMock.Object);
            factoryMock.Setup(o => o.createDbContext<IProductDbContext>(It.IsAny<String>()))
                .Returns(productDbContext);
        }

        public void Dispose() {
            scope.Dispose();
        }

        [Theory] 
        [InlineData("Sony Television", 1200.00)]
        [InlineData("Microwave", 120.00)]   
        public async void Test_InsertProductMethod(String name, decimal price)                         
        {
            var newid = await productRepo.InsertProduct(name, price);
            Assert.True(newid >= 1);
        }
 
    }
}
