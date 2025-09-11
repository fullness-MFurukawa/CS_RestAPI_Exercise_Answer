using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestAPI_Exercise.Infrastructure.Contexts;
using RestAPI_Exercise.Infrastructure.Entities;
using RestAPI_Exercise.Presentation.Configs;
namespace RestAPI_Exercise.Infrastructure.Tests.Contexts;
/// <summary>
/// アプリケーション用DbContextの単体テストドライバ
/// </summary>
[TestClass]
public class AddDbContextTests
{
    private static TestContext? _testContext;
    private static ServiceProvider? _provider;
    private IServiceScope? _scope;
    private static AppDbContext? _dbContext;

    /// <summary>
    /// テストクラスの初期化
    /// </summary>
    /// <param name="context"></param>
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
    /// <summary>
    /// テストクラスクリーンアップ
    /// </summary>
    [ClassCleanup]
    public static void ClassCleanup()
    {
        _provider?.Dispose();
    }

    /// <summary>
    /// テストメソッド実行の前処理
    /// </summary>
    [TestInitialize]
    public void TestInit()
    {
        _scope = _provider!.CreateScope();
        _dbContext =
        _scope.ServiceProvider.GetRequiredService<AppDbContext>();
    }
    /// <summary>
    /// テストメソッド実行後の後処理
    /// </summary> 
    [TestCleanup]
    public void TestCleanup()
    {
        _scope!.Dispose();
    }

    [TestMethod("データベース接続ができる")]
    public void DbConnect_ShouldSucceed()
    {
        try
        {
            // データベースに接続する
            _dbContext!.Database.OpenConnection();
            _testContext?.WriteLine("DB接続成功しました。");
            Assert.IsTrue(true);
        }
        catch (Exception ex)
        {
            _testContext?.WriteLine($"例外が発生しました: {ex.Message}");
            _testContext?.WriteLine($"スタックトレース:\n{ex.StackTrace}");
            Assert.Fail("接続に失敗しました。");
        }
        finally
        {
            // データベース接続を解除する
            _dbContext!.Database.CloseConnection();
            _dbContext!.Dispose(); // AppDbContxetを破棄する
        }
    }

    [TestMethod("DbSetプロパティにアクセスできる")]
    public void DbSet_Properties_ShouldBeAccessible()
    {
        // DbSetプロパティにアクセスできることを検証する
        Assert.IsNotNull(_dbContext!.Products, "Products DbSet にアクセスできません。");
        Assert.IsNotNull(_dbContext!.ProductCategories, "ProductCategories DbSet にアクセスできません。");
        Assert.IsNotNull(_dbContext!.ProductStocks, "ProductStocks DbSet にアクセスできません。");

        // 型が期待どおりであることを検証する
        Assert.IsInstanceOfType(_dbContext!.Products, typeof(DbSet<ProductEntity>));
        Assert.IsInstanceOfType(_dbContext!.ProductCategories, typeof(DbSet<ProductCategoryEntity>));
        Assert.IsInstanceOfType(_dbContext!.ProductStocks, typeof(DbSet<ProductStockEntity>));

        // クエリが実行できることを検証する
        // データが空でも例外なくCount()が返ればOKとする
        try
        {
            var _ = _dbContext!.Products.Count(); // 例外が出ないことを確認
            var __ = _dbContext!.ProductCategories.Count();
            var ___ = _dbContext!.ProductStocks.Count();
            Assert.IsTrue(true);
        }
        catch (Exception ex)
        {
            Assert.Fail($"DbSetに対する基本的なクエリ実行に失敗: {ex.Message}");
        }
    }
}