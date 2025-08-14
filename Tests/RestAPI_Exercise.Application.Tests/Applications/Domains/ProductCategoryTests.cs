using RestAPI_Exercise.Application.Domains.Models;
using RestAPI_Exercise.Application.Exceptions;
namespace RestAPI_Exercise.Application.Tests.Applications.Domains;

/// <summary>
/// ProductCategoryクラスの単体テストドライバ
/// </summary>
[TestClass]
public class ProductCategoryTests
{
    [TestMethod]
    public void コンストラクタ_正常値で生成されること()
    {
        // Arrange
        var uuid = Guid.NewGuid().ToString();
        var name = "文房具";
        // Act
        var category = new ProductCategory(uuid, name);
        // Assert
        Assert.AreEqual(uuid, category.CategoryUuid);
        Assert.AreEqual(name, category.Name);
    }

    [TestMethod]
    public void 新規作成コンストラクタ_UUIDが自動生成されること()
    {
        // Arrange
        var name = "パソコン";
        // Act
        var category = new ProductCategory(name);
        // Assert
        Assert.IsTrue(Guid.TryParse(category.CategoryUuid, out _));
        Assert.AreEqual(name, category.Name);
    }

    [TestMethod]
    [ExpectedException(typeof(DomainException))]
    public void コンストラクタ_UUIDが不正なら例外()
    {
        // Arrange
        var invalidUuid = "abcde"; // 不正なUUID
        var name = "カテゴリ";
        // Act
        _ = new ProductCategory(invalidUuid, name);
    }

    [TestMethod]
    [ExpectedException(typeof(DomainException))]
    public void コンストラクタ_カテゴリ名が空白なら例外()
    {
        // Arrange
        var uuid = Guid.NewGuid().ToString();
        var name = "   ";
        // Act
        _ = new ProductCategory(uuid, name);
    }

    [TestMethod]
    [ExpectedException(typeof(DomainException))]
    public void コンストラクタ_カテゴリ名が21文字以上なら例外()
    {
        // Arrange
        var uuid = Guid.NewGuid().ToString();
        var name = new string('あ', 21); // 21文字
        // Act
        _ = new ProductCategory(uuid, name);
    }

    [TestMethod]
    public void ChangeName_有効な名前に変更できること()
    {
        // Arrange
        var category = new ProductCategory("アクセサリ");
        var newName = "雑貨";
        // Act
        category.ChangeName(newName);
        // Assert
        Assert.AreEqual(newName, category.Name);
    }

    [TestMethod]
    [ExpectedException(typeof(DomainException))]
    public void ChangeName_空文字に変更すると例外()
    {
        // Arrange
        var category = new ProductCategory("周辺機器");
        // Act
        category.ChangeName("");
    }

    [TestMethod]
    public void Equals_同一UUIDで等価と判定されること()
    {
        // Arrange
        var uuid = Guid.NewGuid().ToString();
        var category1 = new ProductCategory(uuid, "A");
        var category2 = new ProductCategory(uuid, "B");
        // Act & Assert
        Assert.AreEqual(category1, category2);
    }

    [TestMethod]
    public void Equals_異なるUUIDで非等価と判定されること()
    {
        // Arrange
        var category1 = new ProductCategory("周辺機器");
        var category2 = new ProductCategory("雑貨");
        // Act & Assert
        Assert.AreNotEqual(category1, category2);
    }
}