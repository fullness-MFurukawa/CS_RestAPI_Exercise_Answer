using Microsoft.EntityFrameworkCore;
using RestAPI_Exercise.Application.Domains.Repositories;
using RestAPI_Exercise.Infrastructure.Adapters;
using RestAPI_Exercise.Infrastructure.Contexts;
using RestAPI_Exercise.Infrastructure.Repositories;

namespace RestAPI_Exercise.Infrastructure.Tests.Repositories;

/// <summary>
///  ドメインオブジェクト:商品在庫のCRUD操作インターフェイスの実装の単体テストドライバ
/// </summary>
[TestClass]
[TestCategory("Repositories")]
public class ProductStockRepositoryTests
{
    private static TestContext? _testContext;
    private static AppDbContext _dbContext;
    // テストターゲット
    private static IProductStockRepository _productStockRepository = null!;
    /// <summary>
    /// テストの前処理
    /// </summary>
    [ClassInitialize]
    public static void SetUp(TestContext context)
    {
        // MSTestテスト用ログ出力ハンドルを設定する
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
        // ProductStockEntityAdapterの生成
        var adapter = new ProductStockEntityAdapter();
        // テストターゲットを生成する   
        _productStockRepository = new ProductStockRepository(_dbContext, adapter);
    }    
}