using RestAPI_Exercise.Application.Domains.Repositories;
using RestAPI_Exercise.Application.Exceptions;
using RestAPI_Exercise.Application.Usecases.Products.Interfaces;
using RestAPI_Exercise.Application.Domains.Models;
using Microsoft.Extensions.Configuration;
using RestAPI_Exercise.Presentation.Configs;
using Microsoft.Extensions.DependencyInjection;
namespace RestAPI_Exercise.Application.Tests.Usecase.Products.Interactors;
/// <summary>
/// ユースケース:[新商品を登録する]を実現するインターフェイスの実装のテストドライバ
/// </summary>
[TestClass]
[TestCategory("Usecase/Products/Interactor")]
public class RegisterProductUsecaseTests
{
    private static TestContext? _testContext;
    private static ServiceProvider? _provider;
    private IServiceScope? _scope;
    // テストターゲット
    private static IRegisterProductUsecase? _uscase;
    // 商品リポジトリ
    private static IProductRepository? _productRepository;


    /// <summary>
    /// テストクラスの初期化
    /// </summary>
    /// <param name="_"></param>
    [ClassInitialize]
    public static void ClassInit(TestContext context)
    {
        // MSTestテスト用ログ出力ハンドルを設定する
        _testContext = context;
        var config = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
        _provider = ApplicationDependencyExtensions.BuildAppProvider(config);
    }

     [ClassCleanup]
    public static void ClassCleanup()
    {
        _provider?.Dispose();
    }

    /// <summary>
    /// テストの前処理
    /// </summary>
    [TestInitialize]
    public void TestInit()
    {
        _scope = _provider!.CreateScope();
        _uscase =
        _scope.ServiceProvider.GetRequiredService<IRegisterProductUsecase>(); 
        _productRepository =
        _scope.ServiceProvider.GetRequiredService<IProductRepository>();  
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _scope!.Dispose();
    }

    [TestMethod("すべての商品カテゴリを取得できる")]
    public async Task GetCategoriesAsync_ShouldReturnAllCategories()
    {
        // すべてのカテゴリを取得する
        var categories = await _uscase!.GetCategoriesAsync();
        // nullでないことを検証する
        Assert.IsNotNull(categories);
        // 件数が3件であることを検証する
        Assert.AreEqual(3, categories.Count());
        // 取得内容を検証する
        Assert.AreEqual("2f4d3e51-6f6b-11f0-954a-00155d1bd29a", categories[0].CategoryUuid);
        Assert.AreEqual("文房具", categories[0].Name);
        Assert.AreEqual("2f5016b6-6f6b-11f0-954a-00155d1bd29a", categories[1].CategoryUuid);
        Assert.AreEqual("雑貨", categories[1].Name);
        Assert.AreEqual("2f501b67-6f6b-11f0-954a-00155d1bd29a", categories[2].CategoryUuid);
        Assert.AreEqual("パソコン周辺機器", categories[2].Name);
        foreach (var category in categories)
        {
            _testContext?.WriteLine(category.ToString());
        }
    }

    [TestMethod("存在する商品カテゴリIdで商品カテゴリを取得できる")]
    public async Task GetCategoryByIdAsync_ShouldReturnCategory_WhenIdExists()
    {
        // 商品カテゴリ雑貨を取得する
        var category = await _uscase!.GetCategoryByIdAsync("2f5016b6-6f6b-11f0-954a-00155d1bd29a");
        // nullでないことを検証する
        Assert.IsNotNull(category);
        // 商品カテゴリIdと商品カテゴリ名を検証する
        Assert.AreEqual("2f5016b6-6f6b-11f0-954a-00155d1bd29a", category.CategoryUuid);
        Assert.AreEqual("雑貨", category.Name);
    }

    [TestMethod("存在しない商品カテゴリIdを指定するとNotFoundExceptionがスローされる")]
    public async Task GetCategoryByIdAsync_ShouldThrowNotFoundException_WhenIdDoesNotExist()
    {
        var ex = await Assert.ThrowsExceptionAsync<NotFoundException>(async () =>
        {
            // 存在しない商品カテゴリIdでカテゴリを取得する
            await _uscase!.GetCategoryByIdAsync("2f5016b6-6f6b-11f0-954a-00155d1bd30a");
        });
        Assert.AreEqual("商品カテゴリId:2f5016b6-6f6b-11f0-954a-00155d1bd30aの商品カテゴリは存在しません。", ex.Message);
    }

    [TestMethod("存在する商品名を指定すると例外はスローされない")]
    public async Task ExistsByProductNameAsync_ShouldNotThrow_WhenNameExists()
    {
        await _uscase!.ExistsByProductNameAsync("油性ボールペン");
        Assert.IsTrue(true);
    }

    [TestMethod("存在しない商品名を指定するとExistsExceptionがスローされる")]
    public async Task ExistsByProductNameAsync_ShouldThrowExistsException_WhenNameDoesNotExist()
    {
        var ex = await Assert.ThrowsExceptionAsync<ExistsException>(async () =>
        {
            await _uscase!.ExistsByProductNameAsync("油性ボールペン(赤)");
        });
        Assert.AreEqual("商品名:油性ボールペン(赤)は既に存在します。", ex.Message);
    }

    [TestMethod("新商品を登録できる")]
    public async Task RegisterProductAsync_ShouldCreateNewProduct()
    {
        // テストデータを用意する
        var category = new ProductCategory("2f4d3e51-6f6b-11f0-954a-00155d1bd29a", "文房具");
        var stock = new ProductStock(Guid.NewGuid().ToString(), 20);
        var product = new Product(Guid.NewGuid().ToString(), "商品-A", 150);
        product.ChangeCategory(category);
        product.ChangeStock(stock);
        // 新商品を登録する
        await _uscase!.RegisterProductAsync(product);
        // 登録された商品を取得する
        var result = await _productRepository!
            .SelectByIdWithProductStockAndProductCategoryAsync(product.ProductUuid);
        // nullでないことを検証する
        Assert.IsNotNull(result);
        // 商品Idを検証する
        Assert.AreEqual(product.ProductUuid, result.ProductUuid);
        // 商品名を検証する
        Assert.AreEqual(product.Name, result.Name);
        // 単価を検証する
        Assert.AreEqual(product.Price, result.Price);
        // 商品在庫Idを検証する
        Assert.AreEqual(product.Stock!.StockUuid, result.Stock!.StockUuid);
        // 商品在庫数を検証する
        Assert.AreEqual(product.Stock!.Stock, result.Stock!.Stock);
        // 追加したデータをクリーニングする
        await _productRepository!.DeleteByIdAsync(product.ProductUuid);
    }
}