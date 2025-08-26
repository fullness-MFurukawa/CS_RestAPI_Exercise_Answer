using Microsoft.EntityFrameworkCore;
using RestAPI_Exercise.Infrastructure.Contexts;
using RestAPI_Exercise.Infrastructure.Entities;
namespace RestAPI_Exercise.Infrastructure.Tests.Contexts;
/// <summary>
/// アプリケーション用DbContextの単体テストドライバ
/// </summary>
[TestClass]
public class AddDbContextTests
{
    private static TestContext? _testContext;
    [ClassInitialize]
    public static void ClassInit(TestContext context)
    {
        // MSTestテスト用ログ出力ハンドルを設定する
        _testContext = context;
    }

    [TestMethod("データベース接続ができる")]
    public void DbConnect_ShouldSucceed()
    {
        // 接続文字列
        var connectionString =
        "Server=localhost;Port=3306;Database=restapi_exercise;User Id=root;Password=root;";
        // データベース接続オプションを生成する
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36)),
                mySqlOptions => mySqlOptions.EnableRetryOnFailure())
            .Options;
        // AppDbContextを生成する
        var context = new AppDbContext(options);
        try
        {
            // データベースに接続する
            context.Database.OpenConnection();
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
            context.Database.CloseConnection();
            context.Dispose(); // AppDbContxetを破棄する
        }
    }

    [TestMethod("DbSetプロパティにアクセスできる")]
    public void DbSet_Properties_ShouldBeAccessible()
    {
        var connectionString =
            "Server=localhost;Port=3306;Database=restapi_exercise;User Id=root;Password=root;";
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 36)))
            .Options;
        using var context = new AppDbContext(options);

        // DbSetプロパティにアクセスできることを検証する
        Assert.IsNotNull(context.Products, "Products DbSet にアクセスできません。");
        Assert.IsNotNull(context.ProductCategories, "ProductCategories DbSet にアクセスできません。");
        Assert.IsNotNull(context.ProductStocks, "ProductStocks DbSet にアクセスできません。");

        // 型が期待どおりであることを検証する
        Assert.IsInstanceOfType(context.Products, typeof(DbSet<ProductEntity>));
        Assert.IsInstanceOfType(context.ProductCategories, typeof(DbSet<ProductCategoryEntity>));
        Assert.IsInstanceOfType(context.ProductStocks, typeof(DbSet<ProductStockEntity>));

        // クエリが実行できることを検証する
        // データが空でも例外なくCount()が返ればOKとする
        try
        {
            var _ = context.Products.Count(); // 例外が出ないことを確認
            var __ = context.ProductCategories.Count();
            var ___ = context.ProductStocks.Count();
            Assert.IsTrue(true);
        }
        catch (Exception ex)
        {
            Assert.Fail($"DbSetに対する基本的なクエリ実行に失敗: {ex.Message}");
        }
    }
}