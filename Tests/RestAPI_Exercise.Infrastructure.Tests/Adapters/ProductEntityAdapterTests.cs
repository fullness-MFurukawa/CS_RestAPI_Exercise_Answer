using RestAPI_Exercise.Application.Domains.Models;
using RestAPI_Exercise.Application.Exceptions;
using RestAPI_Exercise.Infrastructure.Adapters;
using RestAPI_Exercise.Infrastructure.Entities;
namespace RestAPI_Exercise.Infrastructure.Tests.Adapters;
/// <summary>
/// ドメインオブジェクト:ProductとProductEntityの相互変換クラスの単体テストドライバ
/// </summary>
[TestCategory("Adapters")]
[TestClass]
public class ProductEntityAdapterTests
{
    // テストターゲット
    private ProductEntityAdapter _adapter = null!;
    /// <summary>
    /// テストの前処理
    /// </summary>
    [TestInitialize]
    public void SetUp()
    {
        // テストターゲットを生成する
        _adapter = new ProductEntityAdapter();
    }

    [TestMethod("ProductからProductEntityに変換できる")]
    public async Task ConvertAsync_Should_MapPropertiesCorrectly()
    {
        // 変換対象を生成する
        var uuid = Guid.NewGuid().ToString();
        var domain = new Product(uuid, "ペン", 120);
        // ProductをProductEntityに変換する
        var entity = await _adapter.ConvertAsync(domain);
        // nullでないことを検証する
        Assert.IsNotNull(entity);
        // 商品Idが一致することを検証する
        Assert.AreEqual(uuid, entity.ProductUuid);
        // 商品名がペンであることを検証する
        Assert.AreEqual("ペン", entity.Name);
        // 単価が120であることを検証する
        Assert.AreEqual(120, entity.Price);
    }

    [TestMethod("ConvertAsync()メソッドにnullを渡すとInternalExceptionをスローする")]
    public async Task ConvertAsync_Should_ThrowException_When_Null()
    {
        var ex = await Assert.ThrowsExceptionAsync<InternalException>(async () =>
        {
            _ = await _adapter.ConvertAsync(null!);
        });
        Assert.AreEqual("引数domainがnullです。", ex.Message);
    }

    [TestMethod("ProductEntityからProductを復元できる")]
    public async Task RestoreAsync_Should_MapPropertiesCorrectly()
    {
        // 復元対象を生成する
        var uuid = Guid.NewGuid().ToString();
        var entity = new ProductEntity { ProductUuid = uuid, Name = "ノート", Price = 350 };
        // ProductEntityからProductを復元する
        var domain = await _adapter.RestoreAsync(entity);
        // nullでないことを検証する
        Assert.IsNotNull(domain);
        // 商品Idが一致していることを検証する
        Assert.AreEqual(uuid, domain.ProductUuid);
        // 商品名がノートであることを検証する
        Assert.AreEqual("ノート", domain.Name);
        // 単価が350であることを検証する
        Assert.AreEqual(350, domain.Price);
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