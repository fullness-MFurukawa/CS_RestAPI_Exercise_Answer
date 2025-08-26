using RestAPI_Exercise.Application.Domains.Models;
using RestAPI_Exercise.Application.Exceptions;
using RestAPI_Exercise.Infrastructure.Adapters;
using RestAPI_Exercise.Infrastructure.Entities;

namespace RestAPI_Exercise.Infrastructure.Tests.Adapters;

/// <summary>
/// ドメインオブジェクト:ProductCategoryとProductCategoryEntityの相互変換クラスの単体テストドライバ
/// </summary>
[TestClass]
[TestCategory("Adapters")]
public class ProductCategoryEntityAdapterTests
{
    // テストターゲット
    private ProductCategoryEntityAdapter _adapter = null!;
    /// <summary>
    /// テストの前処理
    /// </summary>
    [TestInitialize]
    public void SetUp()
    {
        // テストターゲットの生成
        _adapter = new ProductCategoryEntityAdapter();
    }

    [TestMethod("ProductCategoryからProductCategoryEntityに変換できる")]
    public async Task ConvertAsync_Should_MapPropertiesCorrectly()
    {
        // 変換対象を生成する
        var uuid = Guid.NewGuid().ToString();
        var domain = new ProductCategory(uuid, "文房具");
        // ProductCategroyをProductCategoryEntityに変換する
        var entity = await _adapter.ConvertAsync(domain);
        // nullでないことを検証する
        Assert.IsNotNull(entity);
        // カテゴリIdが一致していることを検証する
        Assert.AreEqual(uuid, entity.CategoryUuid);
        // カテゴリ名が文房具であることを検証する
        Assert.AreEqual("文房具", entity.Name);
    }

    [TestMethod("ConvertAsync()メソッドにnullを渡すとInternalExceptionをスローする")]
    public async Task ConvertAsync_Should_ThrowException_When_Null()
    {
        var exception = await Assert.ThrowsExceptionAsync<InternalException>(async () =>
        {
            _ = await _adapter.ConvertAsync(null!);
        });
        Assert.AreEqual("引数domainがnullです。", exception.Message);
    }

    [TestMethod("ProductCategoryEntityからProductCategoryを復元できる（プロパティが一致する）")]
    public async Task RestoreAsync_Should_MapPropertiesCorrectly()
    {
        // 復元対象を生成する
        var uuid = Guid.NewGuid().ToString();
        var entity = new ProductCategoryEntity { CategoryUuid = uuid, Name = "家電" };
        // ProductCategoryEntityからProductCategoryを復元する
        var domain = await _adapter.RestoreAsync(entity);

        // nullでないことを検証する
        Assert.IsNotNull(domain);
        // カテゴリIdが一致していることを検証する
        Assert.AreEqual(uuid, domain.CategoryUuid);
        // カテゴリ名が家電であることを検証する
        Assert.AreEqual("家電", domain.Name);
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