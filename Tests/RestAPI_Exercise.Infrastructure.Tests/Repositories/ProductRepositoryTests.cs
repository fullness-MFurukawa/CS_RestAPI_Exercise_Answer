using Microsoft.EntityFrameworkCore;
using RestAPI_Exercise.Application.Domains.Models;
using RestAPI_Exercise.Application.Domains.Repositories;
using RestAPI_Exercise.Infrastructure.Adapters;
using RestAPI_Exercise.Infrastructure.Contexts;
using RestAPI_Exercise.Infrastructure.Repositories;
namespace RestAPI_Exercise.Infrastructure.Tests.Repositories;
/// <summary>
///  ドメインオブジェクト:商品のCRUD操作インターフェイスの実装の単体テストドライバ
/// </summary>
[TestClass]
[TestCategory("Repositories")]
public class ProductRepositoryTests
{
    private static TestContext? _testContext;
    private static AppDbContext? _dbContext;
    // テストターゲット
    private static IProductRepository _productRepository = null!;
    /// <summary>
    /// テストクラスの初期化処理
    /// </summary>
    [ClassInitialize]
    public static void SetUp(TestContext context)
    {
        _testContext = context;
        // AppDbContextの生成
        var connectionString =
        "Server=localhost;Port=3306;Database=restapi_exercise;User Id=root;Password=root;";
        // データベース接続オプションを生成する
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36)),
                mySqlOptions => mySqlOptions.EnableRetryOnFailure())
            .Options;
        _dbContext = new AppDbContext(options);
        // ProductFactoryの生成
        var factory = new ProductFactory(
            new ProductEntityAdapter(), new ProductCategoryEntityAdapter(),
            new ProductStockEntityAdapter());
        // テストターゲットを生成する   
        _productRepository = new ProductRepository(_dbContext, factory);
    }

    [TestMethod("存在する商品Idで商品、商品在庫、商品カテゴリを取得できる")]
    public async Task SelectByIdWithProductStockAndProductCategoryAsync_WhenIdExists_ShouldReturnProductWithStockAndCategory()
    {
        var product = await _productRepository
        .SelectByIdWithProductStockAndProductCategoryAsync("8f81a72a-58ef-422b-b472-d982e8665292");
        // nullでないことを検証する
        Assert.IsNotNull(product);
        // 商品Idを検証する
        Assert.AreEqual("8f81a72a-58ef-422b-b472-d982e8665292", product.ProductUuid);
        // 商品名を検証する
        Assert.AreEqual("水性ボールペン(赤)", product.Name);
        // 単価を検証する
        Assert.AreEqual(120, product.Price);
        // 商品在庫がnullでないことを検証する
        Assert.IsNotNull(product.Stock);
        // 商品在庫Idを検証する
        Assert.AreEqual("828fc152-6f6b-11f0-954a-00155d1bd29a", product.Stock.StockUuid);
        // 在庫数を検証する
        Assert.AreEqual(100, product.Stock.Stock);
        // 商品カテゴリIdを検証する
        Assert.AreEqual("2f4d3e51-6f6b-11f0-954a-00155d1bd29a", product.Category!.CategoryUuid);
        // 商品カテゴリ名を検証する
        Assert.AreEqual("文房具", product.Category!.Name);
    }

    [TestMethod("存在しない商品Idの場合nullが返される")]
    public async Task SelectByIdWithProductStockAndProductCategoryAsync_WhenIdDoesNotExist_ShouldReturnNull()
    {
        var product = await _productRepository
        .SelectByIdWithProductStockAndProductCategoryAsync("8f81a72a-58ef-422b-b472-d982e8665282");
        // nullであることを検証する
        Assert.IsNull(product);
    }


    [TestMethod("商品と商品在庫を永続化できる")]
    public async Task CreateAsync_WithStock_ShouldPersistBoth()
    {
        // 登録データを用意する
        var productCategory = new ProductCategory("2f4d3e51-6f6b-11f0-954a-00155d1bd29a", "文房具");
        var productStock = new ProductStock(Guid.NewGuid().ToString(), 20);
        var product = new Product(Guid.NewGuid().ToString(), "商品-A", 300);
        product.ChangeStock(productStock);
        product.ChangeCategory(productCategory);

        // MySQLプロバイダは接続エラー時に自動リトライを行うが、手動でトランザクションを開始する場合は、
        // この戦略の下で全処理を一つの「再試行可能な単位」として実行する必要がある。
        var strategy = _dbContext!.Database.CreateExecutionStrategy();
        await strategy!.ExecuteAsync(async () =>
        {
            // トランザクションを開始する
            await using var tx = await _dbContext!.Database.BeginTransactionAsync();
            try
            {
                // 商品と商品在庫を永続化する
                await _productRepository.CreateAsync(product);
                // 登録された商品と商品在庫を取得して値を検証する
                var result = await _productRepository
                .SelectByIdWithProductStockAndProductCategoryAsync(product.ProductUuid);
                // nullでないことを検証する
                Assert.IsNotNull(result);
                // 商品Idを検証する
                Assert.AreEqual(result.ProductUuid, product.ProductUuid);
                // 商品名を検証する
                Assert.AreEqual(result.Name, product.Name);
                // 単価を検証する
                Assert.AreEqual(result.Price, product.Price);
                // 商品在庫がnullでないことを検証する
                Assert.IsNotNull(result.Stock);
                // 商品在庫Idを検証する
                Assert.AreEqual(result.Stock.StockUuid, product.Stock!.StockUuid);
                // 在庫数を検証する
                Assert.AreEqual(result.Stock.Stock, product.Stock.Stock);
            }
            finally
            {
                tx.Rollback(); // トランザクションをロールバックする
                tx.Dispose();  // トランザクションリソースを開放する
                _testContext!.WriteLine("トランザクションをロールバックしました。");
            }
        });
    }

    [TestMethod("商品名が存在するとtrueが返される")]
    public async Task ExistsByName_WhenNameExists_ShouldReturnTrue()
    {
        var result = await _productRepository.ExistsByNameAsync("蛍光ペン(赤)");
        Assert.IsTrue(result);
    }

    [TestMethod("商品名が存在しないとfalseが返される")]
    public async Task ExistsByName_WhenNameDoesNotExist_ShouldReturnFalse()
    {
        var result = await _productRepository.ExistsByNameAsync("蛍光ペン(黒)");
        Assert.IsFalse(result);
    }

    [TestMethod("存在する商品のキーワードを指定すると、該当する商品のリストが返される")]
    public async Task SelectByNameLikeWithProductStockAsync_WithExistingKeyword_ShouldReturnMatchingProducts()
    {
        var products = await _productRepository.SelectByNameLikeWithProductStockAsync("蛍光ペン");
        // nullでないことを検証する
        Assert.IsNotNull(products);
        // 件数が4件であることを検証する
        Assert.AreEqual(4, products.Count);
        // 商品Idを検証する
        Assert.AreEqual("dc7243af-c2ce-4136-bd5d-c6b28ee0a20a", products[0].ProductUuid);
        // 商品名を検証する
        Assert.AreEqual("蛍光ペン(黄)", products[0].Name);
        // 単価を検証する
        Assert.AreEqual(130, products[0].Price);
        // 商品在庫Idを検証する
        Assert.AreEqual("828fc727-6f6b-11f0-954a-00155d1bd29a", products[0].Stock!.StockUuid);
        // 商品在庫数を検証する
        Assert.AreEqual(100, products[0].Stock!.Stock);

        // 商品Idを検証する
        Assert.AreEqual("83fbc81d-2498-4da6-b8c2-54878d3b67ff", products[1].ProductUuid);
        // 商品名を検証する
        Assert.AreEqual("蛍光ペン(赤)", products[1].Name);
        // 単価を検証する
        Assert.AreEqual(130, products[1].Price);
        // 商品在庫Idを検証する
        Assert.AreEqual("828fc78f-6f6b-11f0-954a-00155d1bd29a", products[1].Stock!.StockUuid);
        // 商品在庫数を検証する
        Assert.AreEqual(100, products[1].Stock!.Stock);

        // 商品Idを検証する
        Assert.AreEqual("ee4b3752-3fbd-45fc-afb5-8f37c3f701c9", products[2].ProductUuid);
        // 商品名を検証する
        Assert.AreEqual("蛍光ペン(青)", products[2].Name);
        // 単価を検証する
        Assert.AreEqual(130, products[2].Price);
        // 商品在庫Idを検証する
        Assert.AreEqual("828fc805-6f6b-11f0-954a-00155d1bd29a", products[2].Stock!.StockUuid);
        // 商品在庫数を検証する
        Assert.AreEqual(100, products[2].Stock!.Stock);

        // 商品Idを検証する
        Assert.AreEqual("35cb51a7-df79-4771-9939-7f32c19bca45", products[3].ProductUuid);
        // 商品名を検証する
        Assert.AreEqual("蛍光ペン(緑)", products[3].Name);
        // 単価を検証する
        Assert.AreEqual(130, products[3].Price);
        // 商品在庫Idを検証する
        Assert.AreEqual("828fc869-6f6b-11f0-954a-00155d1bd29a", products[3].Stock!.StockUuid);
        // 商品在庫数を検証する
        Assert.AreEqual(100, products[3].Stock!.Stock);
    }

    [TestMethod("存在しない商品のキーワードを指定すると、空の商品のリストが返される")]
    public async Task SelectByNameLikeWithProductStockAsync_WithNonExistingKeyword_ShouldReturnEmptyList()
    {
        var products = await _productRepository.SelectByNameLikeWithProductStockAsync("商品-X");
        // nullでないことを検証する
        Assert.IsNotNull(products);
        // 件数が0であることを検証する
        Assert.AreEqual(0, products.Count);
    }

    [TestMethod("存在する商品を変更するとtrueが返される")]
    public async Task UpdateProduct_WhenProductExists_ShouldReturnTrue()
    {
        // 変更データを準備する
        var productStock = new ProductStock("828fb567-6f6b-11f0-954a-00155d1bd29a", 50);
        var product = new Product("ac413f22-0cf1-490a-9635-7e9ca810e544", "ボールペン(黒)", 150);
        product.ChangeStock(productStock);

        // MySQLプロバイダは接続エラー時に自動リトライを行うが、手動でトランザクションを開始する場合は、
        // この戦略の下で全処理を一つの「再試行可能な単位」として実行する必要がある。
        var strategy = _dbContext!.Database.CreateExecutionStrategy();
        await strategy!.ExecuteAsync(async () =>
        {
            // トランザクションを開始する
            await using var tx = await _dbContext!.Database.BeginTransactionAsync();
            try
            {
                // 商品を変更する
                var result = await _productRepository.UpdateByIdAsync(product);
                // trueであることを検証する
                Assert.IsTrue(result);
                // 変更された商品を取得する
                var updateResult = await _productRepository
                    .SelectByIdWithProductStockAndProductCategoryAsync(product.ProductUuid);
                // 商品名を検証する
                Assert.AreEqual(product.Name, updateResult!.Name);
                // 単価を検証する
                Assert.AreEqual(product.Price, updateResult!.Price);
                // 商品在庫数を検証する
                Assert.AreEqual(product.Stock!.Stock, updateResult.Stock!.Stock);
            }
            finally
            {
                tx.Rollback(); // トランザクションをロールバックする
                tx.Dispose();  // トランザクションリソースを開放する
                _testContext!.WriteLine("トランザクションをロールバックしました。");
            }
        });
    }

    [TestMethod("存在しない商品を変更するとfalseが返される")]
    public async Task UpdateProduct_WhenProductDoesNotExist_ShouldReturnFalse()
    {
        // 変更データを準備する
        var productStock = new ProductStock("828fb567-6f6b-11f0-954a-00155d1bd30a", 50);
        var product = new Product("ac413f22-0cf1-490a-9635-7e9ca810e555", "ボールペン(黒)", 150);
        product.ChangeStock(productStock);
        // 商品を変更する
        var result = await _productRepository.UpdateByIdAsync(product);
        // falseが返されることを検証する
        Assert.IsFalse(result);
    }
}