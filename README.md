#An introduction to Unit Testings / XUnit

###The is tutorial for Tribal SchoolEdge internal training.
 
These are the libraries which are used in the project 

1. [Entity Framework reverse POCO Genenerator](https://visualstudiogallery.msdn.microsoft.com/ee4fcff9-0c4c-4179-afd9-7a2fb90f5838)
 
2. [Unity Dependence Injection](https://msdn.microsoft.com/en-us/library/dn178469(v=pandp.30).aspx) 

3. [XUnit](https://xunit.github.io/) 

4. [Moq](https://github.com/Moq/moq4/wiki/Quickstart)
  

**How to run**

* Clone this repository, Open the ShoppingCartSampleCodes solution. 

* Run the DBScript/shoppingcart.sql to create the sample database.

* Edit the Connection String in App.config in the test project, you may also edit the Connection string in Web.config in the Web project. 

* Open the Test Explorer and Run.

```
  <connectionStrings>
    <add name="ShoppingCart" providerName="System.Data.SqlClient" connectionString="Your Own Connection String"/>
  </connectionStrings>
```

### Sample code in details

Why below code is bad for testing?   

```
 public async Task<Product> GetProduct(int id)
        {
            using (var db = ProductDbContext(connectionString))
            {
                return await db.Products.FirstOrDefaultAsync(o => o.Id == id);
            }
        }
```
Because of this line. From programming point of, nothing wrong with this line, even Microsoft sample code uses this way.
```
using (var db = ProductDbContext(connectionString))
```
However, from testing point of view, this declaration creates coupling. You bind your business rule with a specific DB source, whereby this code cannot be tested via TDD. 

**How to modify my existing code, so my function can be tested.**

By introducing DI, the code can be refactored very easily. One line change, now your function can be tested, instead of creating your own context in function level, you inject the DBContext which later on can be replaced by other DBContext like in memory store or your testing data source.  

```
     public async Task<Product> GetProduct(int id)
        {
            using (var db = resolveProductDbContext())
            {
                return await db.Products.FirstOrDefaultAsync(o => o.Id == id);
            }
        }

      public IProductDbContext resolveProductDbContext()
      {
            return factory.createDbContext<IProductDbContext>(defaultconnection);
      }
```

###DI Registration and Resolve

There are two files which make decoupling happen.

* ContainerBootstrapper class


&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; All the DBContext and Repository here are registered as Transit life cycle. For all other life cycle, check the DI website. 

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; In this example, DBContext only lives inside the function level, which is good for 'using(var db = ...)' pattern 

```
 public static void RegisterTypes(IUnityContainer container)
        {
            //DB Factory DI, Singleton life cycle through out the application.  
            container.RegisterType<IDBContextFactory, DBContextFactory>(new ContainerControlledLifetimeManager(), new InjectionConstructor(container));
            container.RegisterType<IShoppingCartDBContext>(new InjectionConstructor(typeof(String)));
            container.RegisterType<IProductDbContext>(new InjectionConstructor(typeof(String)));

            var factory = container.Resolve<IDBContextFactory>();
            container.RegisterType<IProductRepo>(new InjectionConstructor(factory));
            container.RegisterType<IShoppingCartDBContext>(new InjectionConstructor(typeof(IProductRepo), factory));
        }
```



* DBContextFactory class

```
     public T createDbContext<T>(string connection) {
            return unityContainer.Resolve<T>(new ParameterOverride("connectionString", connection));
        }
```
**Usage**

```
var productDbContext = factory.createDbContext<IProductDbContext>(defaultconnection);
var shoppingCartDbContext = factory.createDbContext<IShoppingCartDBContext>(defaultconnection);
```

### Unit Testings

**General rules when designing your code to be test driven**

* Use Interface for separate dependency. Will show you the example later in CartRepo class. 
* Do not mixed business rules or write complex rules in one function. It becomes untestable. Write simple function.
* “Tell, don’t ask” - What this phrase means is that methods should not directly “ask” for (create) system objects that they need. Instead, they should be “told” which system object to use. Avoid creating object which cannot be injected, you have seen the example above. 
 

**The sample code provides both In memory store test and database test.**

* For memory store test, I use the FakeDBContext which creates automatically when you do the reverse POCO. Code below is to create a Mock factory, then setup the createDbContext returns my own memory FakeProductDBContext. Finally, provide initial sample data. 

```
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
```

* For DB test, you can see the code below, which pass the default connection to hook the real database. In the factory mock object, it returns your specified DBContext. The transactionScope is to help us to roll back in the end. 

```
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
```

**Class Dependency** 

In real world software, class dependence is inevitable. In this example, Cart and Product class has dependence, because of one of the function in CartRepo needs to look up the Product detail. If you create a concrete Product class in Cart class, when you test your CartRepo , you have to look after your ProductBo by yourself. It diverts the concept of Unit Testing. 

The Right way is to use Interface.  

```
  public CartRepo(IProductRepo productRepo, IDBContextFactory factory)
        {
            this.productRepo = productRepo;
            this.factory = factory;
        }
```
When you write your Testing function. You can provide your own ProductRepo function to serve CartRepo class
```
  prodcutRepoMock = new Mock<IProductRepo>();
  cartRepo = new CartRepo(prodcutRepoMock.Object, dbFactoryMock.Object);

 var product = new Product() {
                Id = 1,
                Name = "TV",
                Baseprice = 1200.00m
            };
 prodcutRepoMock.Setup(O => O.GetProduct(1)).ReturnsAsync(product);
```

Enjoy Testings!!!


_Footnote: The FakeDBContext provided by Reverse Poco do not carry all Entity Framework feature such as looking after the table relations automatically for you. It will be a bit hassles to setup the initial data all by yourself when you have complicated relations between tables. 
Good news is Entity framework 7 now support memory store out of box which is perfect for unit testings._ 





 



