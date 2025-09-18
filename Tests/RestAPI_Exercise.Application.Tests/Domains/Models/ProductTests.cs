using RestAPI_Exercise.Application.Domains.Models;
using RestAPI_Exercise.Application.Exceptions;
namespace RestAPI_Exercise.Application.Tests.Domains.Models;
/// <summary>
/// Productクラスの単体テストドライバ
/// </summary>
[TestClass]
[TestCategory("Domains/Models")]
public class ProductTests
{
    // ヘルパー：有効なカテゴリ
    private ProductCategory CreateCategory(string name = "雑貨") => new ProductCategory(name);
    // ヘルパー：有効な在庫
    private ProductStock CreateStock(int stock = 10) => new ProductStock(stock);

    [TestMethod("コンストラクタに正常値を指定するとインスタンス生成される")]
    public void Constructor_WithValidValues_ShouldCreateInstance()
    {
        // データを用意する
        var uuid = Guid.NewGuid().ToString();
        var name = "ノート";
        var price = 150;
        var category = CreateCategory();
        var stock = CreateStock();
        // インスタンスを生成する
        var product = new Product(uuid, name, price);
        product.ChangeCategory(category);
        product.ChangeStock(stock);
        // 商品Idを検証する
        Assert.AreEqual(uuid, product.ProductUuid);
        // 商品名を検証する
        Assert.AreEqual(name, product.Name);
        // 単価を検証する
        Assert.AreEqual(price, product.Price);
        // 商品カテゴリを検証する
        Assert.AreEqual(category, product.Category);
        // 商品在庫を検証する
        Assert.AreEqual(stock, product.Stock);
    }

    [TestMethod("新規作成の場合UUIDが自動生成される")]
    public void NewInstance_ShouldGenerateUuidAutomatically()
    {
        // データを用意する
        var name = "ボールペン";
        var price = 100;
        var category = CreateCategory();
        var stock = CreateStock();
        // インスタンスを生成する
        var product = new Product(name, price);
        product.ChangeCategory(category);
        product.ChangeStock(stock);
        // 商品IdがUUID形式かどうかを検証する
        Assert.IsTrue(Guid.TryParse(product.ProductUuid, out _));
        // 商品名を検証する
        Assert.AreEqual(name, product.Name);
        // 単価を検証する
        Assert.AreEqual(price, product.Price);
        // 商品カテゴリを検証する
        Assert.AreEqual(category, product.Category);
        // 商品在庫を検証する
        Assert.AreEqual(stock, product.Stock);
    }

    [TestMethod("不正なUUIDの場合、DomainExceptionがスローされる")]
    public void InvalidUuid_ShouldThrowDomainException()
    {
        // 不正なUUIDを用意する
        var invalidUuid = "abcde";
        var name = "商品";
        var price = 100;
        var ex = Assert.ThrowsException<DomainException>(() =>
        {
            _ = new Product(invalidUuid, name, price);
        });
        // 例外メッセージを検証する
        Assert.AreEqual("UUIDの形式が正しくありません。", ex.Message);
    }

    [TestMethod("商品が空白の場合、DomainExceptionがスローされる")]
    public void EmptyProductName_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsException<DomainException>(() =>
        {
            _ = new Product(Guid.NewGuid().ToString(), "", 100);
        });
        // 例外メッセージを検証する
        Assert.AreEqual("商品名は必須です。", ex.Message);
    }

    [TestMethod("商品名が31文字以上の場合、DomainExceptionがスローされる")]
    public void CategoryNameLongerThan30Chars_ShouldThrowDomainException()
    {
        var name = new string('あ', 31); // 31文字
        var ex = Assert.ThrowsException<DomainException>(() =>
        {
            _ = new Product(Guid.NewGuid().ToString(), name, 100);
        });
        // 例外メッセージを検証する
        Assert.AreEqual("商品名は30文字以内である必要があります。", ex.Message);
    }

    [TestMethod("単価がマイナスの場合場合、DomainExceptionをスローする")]
    public void UnitPrice_WithNegativeValue_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsException<DomainException>(() =>
        {
            _ = new Product(Guid.NewGuid().ToString(), "正しい商品", -1);
        });
        // 例外メッセージを検証する
        Assert.AreEqual("価格は0円以上である必要があります。", ex.Message);
    }

    [TestMethod("有効な商品名に変更できる")]
    public void ProductName_WithValidValue_ShouldSucceed()
    {
        // インスタンスを生成する
        var product = new Product("旧商品", 500);
        // 商品名を変更する
        product.ChangeName("新商品");
        // 変更結果を検証する
        Assert.AreEqual("新商品", product.Name);
    }

    [TestMethod("有効な単価に変更できる")]
    public void ProductPrice_WithValidValue_ShouldSucceed()
    {
        // インスタンスを生成する
        var product = new Product("商品", 500);
        // 単価を変更する
        product.ChangePrice(800);
        // 変更結果を検証する
        Assert.AreEqual(800, product.Price);
    }

    [TestMethod("有効な商品カテゴリに変更できる")]
    public void ProductCategory_WithValidValue_ShouldSucceed()
    {
        // インスタンスを生成する
        var newCategory = CreateCategory("新カテゴリ");
        var product = new Product("商品", 500);
        // 商品カテゴリを変更する
        product.ChangeCategory(newCategory);
        // 商品カテゴリを検証する
        Assert.AreEqual("新カテゴリ", product.Category!.Name);
    }

    
    [TestMethod("有効な商品在庫に変更できる")]
    public void ProductStock_WithValidValue_ShouldSucceed()
    {
        // インスタンスを生成する
        var newStock = CreateStock(30);
        var product = new Product("商品", 500);
        // 商品在庫を変更する
        product.ChangeStock(newStock);
        // 商品在庫を検証する
        Assert.AreEqual(30, product.Stock!.Stock);
    }

    [TestMethod("UUIDで等価と判定される")]
    public void Equals_WithSameUuid_ShouldReturnTrue()
    {
        // インスタンスを生成する
        var uuid = Guid.NewGuid().ToString();
        var p1 = new Product(uuid, "A", 100);
        var p2 = new Product(uuid, "B", 200);
        // 等価性を検証する
        var result = p1.Equals(p2);
        // 検証結果を評価する
        Assert.IsTrue(result);
    }

    [TestMethod("異なるUUIDで非等価と判定される")]
    public void Equals_WithDifferentUuid_ShouldReturnFalse()
    {
        // インスタンスを生成する
        var p1 = new Product("A", 100);
        var p2 = new Product("B", 200);
        // 等価性を検証する
        var result = p1.Equals(p2);
        // 非等価であることを評価する
        Assert.IsFalse(result);
    }
}