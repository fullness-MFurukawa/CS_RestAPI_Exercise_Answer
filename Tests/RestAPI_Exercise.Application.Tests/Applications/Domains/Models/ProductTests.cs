using RestAPI_Exercise.Application.Domains.Models;
using RestAPI_Exercise.Application.Exceptions;
namespace RestAPI_Exercise.Application.Tests.Applications.Domains;
/// <summary>
/// Productクラスの単体テストドライバ
/// </summary>
[TestClass]
public class ProductTests
{
    // ヘルパー：有効なカテゴリ
    private ProductCategory CreateCategory(string name = "雑貨") => new ProductCategory(name);
    // ヘルパー：有効な在庫
    private ProductStock CreateStock(int stock = 10) => new ProductStock(stock);

    [TestMethod]
    public void コンストラクタ_正常な引数で生成される()
    {
        // Arrange
        var uuid = Guid.NewGuid().ToString();
        var name = "ノート";
        var price = 150;
        var category = CreateCategory();
        var stock = CreateStock();
        // Act
        var product = new Product(uuid, name, price, category, stock);
        // Assert
        Assert.AreEqual(uuid, product.ProductUuid);
        Assert.AreEqual(name, product.Name);
        Assert.AreEqual(price, product.Price);
        Assert.AreEqual(category, product.Category);
        Assert.AreEqual(stock, product.Stock);
    }

    [TestMethod]
    public void 新規作成用コンストラクタ_UUIDが自動生成される()
    {
        // Arrange
        var name = "ボールペン";
        var price = 100;
        var category = CreateCategory();
        var stock = CreateStock();
        // Act
        var product = new Product(name, price, category, stock);
        // Assert
        Assert.IsTrue(Guid.TryParse(product.ProductUuid, out _));
        Assert.AreEqual(name, product.Name);
        Assert.AreEqual(price, product.Price);
    }

    [TestMethod]
    [ExpectedException(typeof(DomainException))]
    public void コンストラクタ_UUIDが不正な場合は例外()
    {
        // Arrange
        var invalidUuid = "abcde";
        var name = "商品";
        var price = 100;
        var category = CreateCategory();
        var stock = CreateStock();
        // Act
        _ = new Product(invalidUuid, name, price, category, stock);
    }

    [TestMethod]
    [ExpectedException(typeof(DomainException))]
    public void コンストラクタ_商品名が空文字の場合は例外()
    {
        _ = new Product(Guid.NewGuid().ToString(), "", 100,
            CreateCategory(), CreateStock());
    }

    [TestMethod]
    [ExpectedException(typeof(DomainException))]
    public void コンストラクタ_商品名が31文字以上の場合は例外()
    {
        var name = new string('あ', 31); // 31文字
        _ = new Product(Guid.NewGuid().ToString(), name, 100,
            CreateCategory(), CreateStock());
    }

    [TestMethod]
    [ExpectedException(typeof(DomainException))]
    public void コンストラクタ_価格がマイナスの場合は例外()
    {
        _ = new Product(Guid.NewGuid().ToString(), "正しい商品", -1,
            CreateCategory(), CreateStock());
    }

    [TestMethod]
    [ExpectedException(typeof(DomainException))]
    public void コンストラクタ_カテゴリがnullの場合は例外()
    {
        _ = new Product(Guid.NewGuid().ToString(), "正しい商品", 100, null!, CreateStock());
    }

    [TestMethod]
    [ExpectedException(typeof(DomainException))]
    public void コンストラクタ_在庫がnullの場合は例外()
    {
        _ = new Product(Guid.NewGuid().ToString(), "正しい商品", 100, CreateCategory(), null!);
    }

    [TestMethod]
    public void 商品名を変更できる()
    {
        var product = new Product("旧商品", 500, CreateCategory(), CreateStock());
        product.ChangeName("新商品");
        Assert.AreEqual("新商品", product.Name);
    }
    [TestMethod]
    public void 価格を変更できる()
    {
        var product = new Product("商品", 500, CreateCategory(), CreateStock());
        product.ChangePrice(800);
        Assert.AreEqual(800, product.Price);
    }

    [TestMethod]
    public void カテゴリを変更できる()
    {
        var oldCategory = CreateCategory("旧カテゴリ");
        var newCategory = CreateCategory("新カテゴリ");
        var product = new Product("商品", 500, oldCategory, CreateStock());
        product.ChangeCategory(newCategory);
        Assert.AreEqual("新カテゴリ", product.Category!.Name);
    }

    [TestMethod]
    public void 在庫を変更できる()
    {
        var oldStock = CreateStock(5);
        var newStock = CreateStock(30);
        var product = new Product("商品", 500, CreateCategory(), oldStock);
        product.ChangeStock(newStock);
        Assert.AreEqual(30, product.Stock!.Stock);
    }

    [TestMethod]
    public void Equals_UUIDが同じであれば等価とみなされる()
    {
        var uuid = Guid.NewGuid().ToString();
        var p1 = new Product(uuid, "A", 100, CreateCategory(), CreateStock());
        var p2 = new Product(uuid, "B", 200, CreateCategory(), CreateStock());
        Assert.AreEqual(p1, p2);
    }

    [TestMethod]
    public void Equals_UUIDが異なれば非等価()
    {
        var p1 = new Product("A", 100, CreateCategory(), CreateStock());
        var p2 = new Product("B", 200, CreateCategory(), CreateStock());
        Assert.AreNotEqual(p1, p2);
    }
}