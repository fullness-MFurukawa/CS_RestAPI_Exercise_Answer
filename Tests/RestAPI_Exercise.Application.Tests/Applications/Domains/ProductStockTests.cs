using RestAPI_Exercise.Application.Domains.Models;
using RestAPI_Exercise.Application.Exceptions;

namespace RestAPI_Exercise.Application.Tests.Applications.Domains;
/// <summary>
/// ProductStockクラスの単体テストドライバ
/// </summary>
[TestClass]
public class ProductStockTests
{
    [TestMethod]
    public void コンストラクタ_正常なUUIDと在庫数で生成できる()
    {
        // Arrange
        var uuid = Guid.NewGuid().ToString();
        var stock = 10;
        // Act
        var productStock = new ProductStock(uuid, stock);
        // Assert
        Assert.AreEqual(uuid, productStock.StockUuid);
        Assert.AreEqual(stock, productStock.Stock);
    }

    [TestMethod]
    public void 新規作成コンストラクタ_UUIDが自動生成される()
    {
        // Arrange
        var stock = 5;
        // Act
        var productStock = new ProductStock(stock);
        // Assert
        Assert.IsTrue(Guid.TryParse(productStock.StockUuid, out _));
        Assert.AreEqual(stock, productStock.Stock);
    }

    [TestMethod]
    [ExpectedException(typeof(DomainException))]
    public void コンストラクタ_不正なUUIDなら例外()
    {
        // Arrange
        var invalidUuid = "abcde";
        var stock = 1;
        // Act
        _ = new ProductStock(invalidUuid, stock);
    }
    [TestMethod]
    [ExpectedException(typeof(DomainException))]
    public void コンストラクタ_在庫数がマイナスなら例外()
    {
        // Arrange
        var uuid = Guid.NewGuid().ToString();
        var stock = -1;
        // Act
        _ = new ProductStock(uuid, stock);
    }
    [TestMethod]
    public void ChangeStock_正常に在庫数を更新できる()
    {
        // Arrange
        var productStock = new ProductStock(10);
        var newStock = 50;
        // Act
        productStock.ChangeStock(newStock);
        // Assert
        Assert.AreEqual(newStock, productStock.Stock);
    }
    [TestMethod]
    [ExpectedException(typeof(DomainException))]
    public void ChangeStock_マイナス在庫に変更すると例外()
    {
        // Arrange
        var productStock = new ProductStock(10);
        // Act
        productStock.ChangeStock(-5);
    }

    [TestMethod]
    public void Equals_同一UUIDなら等価と判定される()
    {
        // Arrange
        var uuid = Guid.NewGuid().ToString();
        var stock1 = new ProductStock(uuid, 10);
        var stock2 = new ProductStock(uuid, 20); // stock数が違っても等価とみなす
        // Act & Assert
        Assert.AreEqual(stock1, stock2);
    }

    [TestMethod]
    public void Equals_UUIDが異なる場合は非等価()
    {
        // Arrange
        var stock1 = new ProductStock(10);
        var stock2 = new ProductStock(10);
        // Act & Assert
        Assert.AreNotEqual(stock1, stock2);
    }
}