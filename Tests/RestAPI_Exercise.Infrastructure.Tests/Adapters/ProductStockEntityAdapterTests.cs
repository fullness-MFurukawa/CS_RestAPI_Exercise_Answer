using RestAPI_Exercise.Application.Domains.Models;
using RestAPI_Exercise.Application.Exceptions;
using RestAPI_Exercise.Infrastructure.Adapters;
using RestAPI_Exercise.Infrastructure.Entities;
namespace RestAPI_Exercise.Infrastructure.Tests.Adapters;
/// <summary>
/// ドメインオブジェクト:DepartmentとDepartmentEntityの相互変換クラスの単体テストドライバ
/// </summary>
[TestCategory("Adapters")]
[TestClass]
public class ProductStockEntityAdapterTests
{
    // テストターゲット
    private ProductStockEntityAdapter _adapter = null!;

    /// <summary>
    ///  テストの前処理
    /// </summary>
    [TestInitialize]
    public void SetUp()
    {
        // テストターゲットの生成
        _adapter = new ProductStockEntityAdapter();
    }

    [TestMethod("ProductStockからProductStockEntityに変換できる")]
    public async Task ConvertAsync_Should_MapPropertiesCorrectly()
    {
        // ProductStockを生成する
        var stockUuid = Guid.NewGuid().ToString();
        var domain = new ProductStock(stockUuid, 25);
        // ProductStockをProductStockEntityに変換する
        var entity = await _adapter.ConvertAsync(domain);
        // nullでないことを検証する
        Assert.IsNotNull(entity);
        // 在庫IdがProductStcokと同じであるこを検証する
        Assert.AreEqual(stockUuid, entity.StockUuid);
        // 在庫数が25であることを検証する
        Assert.AreEqual(25, entity.Stock);
    }

    [TestMethod("ConvertAsync()メソッドに nullを渡すとInternalExceptionをスローする")]
    public async Task ConvertAsync_Should_ThrowException_When_Null()
    {
        var exception = await Assert.ThrowsExceptionAsync<InternalException>(async () =>
        {
            _ = await _adapter.ConvertAsync(null!);
        });
        Assert.AreEqual("引数domainがnullです。", exception.Message);
    }

    [TestMethod("ProductStockEntityからProductStockを復元できる")]
    public async Task RestoreAsync_Should_MapPropertiesCorrectly()
    {
        // ProductStcokEntityを生成する
        var stockUuid = Guid.NewGuid().ToString();
        var entity = new ProductStockEntity { StockUuid = stockUuid, Stock = 10 };
        // ProductStockEntityからProductStockを復元する
        var domain = await _adapter.RestoreAsync(entity);
        // nullでないことを検証する
        Assert.IsNotNull(domain);
        // 在庫Idが一致していることを検証する
        Assert.AreEqual(stockUuid, domain.StockUuid);
        // 在庫数が10であることを検証する
        Assert.AreEqual(10, domain.Stock);
    }

    [TestMethod("RestoreAsync()メソッドにnullを渡すとInternalExceptionをスローする")]
    public async Task RestoreAsync_Should_ThrowException_When_Null()
    {
        var exception = await Assert.ThrowsExceptionAsync<InternalException>(async () =>
        {
            _ = await _adapter.RestoreAsync(null!);
        });
        Assert.AreEqual("引数targetがnullです。", exception.Message);        
    }
}