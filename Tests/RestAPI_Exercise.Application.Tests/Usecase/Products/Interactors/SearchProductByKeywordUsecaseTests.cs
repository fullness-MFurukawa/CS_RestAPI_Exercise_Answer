using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestAPI_Exercise.Application.Usecases.Products.Interfaces;
using RestAPI_Exercise.Presentation.Configs;
using RestAPI_Exercise.Application.Exceptions;
namespace RestAPI_Exercise.Application.Tests.Usecase.Products.Interactors;
/// <summary>
/// ユースケース:[商品をキーワード検索する]を実現するインターフェイスの実装のテストドライバ
/// </summary>
[TestClass]
[TestCategory("Usecase/Products/Interactor")]
public class SearchProductByKeywordUsecaseTests
{
    private static TestContext? _testContext;
    private static ServiceProvider? _provider;
    private IServiceScope? _scope;
    // テストターゲット
    private static ISearchProductByKeywordUsecase? _usecase;

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
        _usecase =
        _scope.ServiceProvider.GetRequiredService<ISearchProductByKeywordUsecase>();
    }

    [TestCleanup]
    public void TestCleanup()
    {
        _scope!.Dispose();
    }

    [TestMethod("存在する商品キーワードで商品を取得できる")]
    public async Task ExecuteAsync_ShouldReturnProducts_WhenKeywordExists()
    {
        var results = await _usecase!.ExecuteAsync("蛍光");
        // nullでないことを検証する
        Assert.IsNotNull(results);
        // 件数が4件であること検証する
        Assert.AreEqual(4, results.Count);
        // 商品Idを検証する
        Assert.AreEqual("dc7243af-c2ce-4136-bd5d-c6b28ee0a20a", results[0].ProductUuid);
        // 商品名を検証する
        Assert.AreEqual("蛍光ペン(黄)", results[0].Name);
        // 単価を検証する
        Assert.AreEqual(130, results[0].Price);
        // 在庫数を検証する
        Assert.AreEqual(100, results[0].Stock!.Stock);

        // 商品Idを検証する
        Assert.AreEqual("83fbc81d-2498-4da6-b8c2-54878d3b67ff", results[1].ProductUuid);
        // 商品名を検証する
        Assert.AreEqual("蛍光ペン(赤)", results[1].Name);
        // 単価を検証する
        Assert.AreEqual(130, results[1].Price);
        // 在庫数を検証する
        Assert.AreEqual(100, results[1].Stock!.Stock);

        // 商品Idを検証する
        Assert.AreEqual("ee4b3752-3fbd-45fc-afb5-8f37c3f701c9", results[2].ProductUuid);
        // 商品名を検証する
        Assert.AreEqual("蛍光ペン(青)", results[2].Name);
        // 単価を検証する
        Assert.AreEqual(130, results[2].Price);
        // 在庫数を検証する
        Assert.AreEqual(100, results[2].Stock!.Stock);

        // 商品Idを検証する
        Assert.AreEqual("35cb51a7-df79-4771-9939-7f32c19bca45", results[3].ProductUuid);
        // 商品名を検証する
        Assert.AreEqual("蛍光ペン(緑)", results[3].Name);
        // 単価を検証する
        Assert.AreEqual(130, results[3].Price);
        // 在庫数を検証する
        Assert.AreEqual(100, results[3].Stock!.Stock);
    }

    [TestMethod("存在しない商品キーワードの場合、NotFoundExceptionがスローされる")]
    public async Task ExecuteAsync_ShouldThrowNotFoundException_WhenKeywordDoesNotExist()
    {
        var ex = await Assert.ThrowsExceptionAsync<NotFoundException>(async () =>
        {
            // 商品を変更する
            await _usecase!.ExecuteAsync("ゴム");
        });
        // nullでないことを検証する
        Assert.IsNotNull(ex);
        // 例外メッセージを検証する
        Assert.AreEqual("キーワード:ゴムが含まれる商品は存在しません。", ex.Message);
    }

}