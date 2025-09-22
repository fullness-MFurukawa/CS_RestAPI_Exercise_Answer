using RestAPI_Exercise.Application.Domains.Models;
using RestAPI_Exercise.Application.Exceptions;

namespace RestAPI_Exercise.Application.Tests.Domains.Models;

/// <summary>
/// JWTトークンを表すドメインオブジェクトの単体テストドライバ
/// </summary>
[TestClass]
[TestCategory("Domains/Models")]
public class JWTTokenTests
{
    [TestMethod("コンストラクタに正常値を指定するとインスタンス生成される")]
    public void Constructor_WithValidValues_ShouldCreateInstance()
    {
        // データを用意する
        var userId = Guid.NewGuid().ToString();
        var token = "eyDummy.jwt.token";
        var issuedAt = DateTimeOffset.UtcNow;
        var expiresAt = issuedAt.AddMinutes(10);

        // インスタンスを生成する
        var jwt = new JWTToken(userId, token, issuedAt, expiresAt, "UA:Test");
        // nullでないことを検証する
        Assert.IsNotNull(jwt);
        // UUID形式であることを検証する
        Assert.IsTrue(Guid.TryParse(jwt.TokenId, out _));
        // ユーザーIdを検証する
        Assert.AreEqual(userId, jwt.UserId);
        // アクセストークンを検証する
        Assert.AreEqual(token, jwt.Token);
        // トークンが発行された日時を検証する
        Assert.AreEqual(issuedAt, jwt.IssuedAt);
        // トークンの有効期限を検証する
        Assert.AreEqual(expiresAt, jwt.ExpiresAt);
        // トークンを発行したデバイスや環境の識別情報を検証する
        Assert.AreEqual("UA:Test", jwt.DeviceInfo);
        // トークンが利用者または管理者によって無効化されたかどうかを検証する
        Assert.IsFalse(jwt.Revoked);
    }

    [TestMethod("userIdが空の場合、DomainExceptionがスローされる")]
    public void Ctor_ShouldThrow_WhenUserIdEmpty()
    {
        var issuedAt = DateTimeOffset.UtcNow;
        var expiresAt = issuedAt.AddMinutes(5);
        // DomainExceptionがスローされることを検証する
        Exception ex = Assert.ThrowsException<DomainException>(() =>
        {
            _ = new JWTToken("", "tkn", issuedAt, expiresAt);
        });
        // メッセージを検証する
        Assert.AreEqual("ユーザーIdは必須です。", ex.Message);
    }
    [TestMethod("アクセストークンが空の場合、DomainExceptionがスローされる")]
    public void Ctor_ShouldThrow_WhenTokenEmpty()
    {
        var issuedAt = DateTimeOffset.UtcNow;
        var expiresAt = issuedAt.AddMinutes(5);
        // DomainExceptionがスローされることを検証する
        Exception ex = Assert.ThrowsException<DomainException>(() =>
        {
            _ = new JWTToken(Guid.NewGuid().ToString(), "", issuedAt, expiresAt);
        });
        // メッセージを検証する
        Assert.AreEqual("JWTアクセストークンは必須です。", ex.Message);
    }

    [TestMethod("トークン有効期限よりトークン発行日時が大きい場合、DomainExceptionをスローする")]
    public void Ctor_ShouldThrow_WhenExpiresNotAfterIssued()
    {
        var issuedAt = DateTimeOffset.UtcNow;
        var same = issuedAt;
        // トークン有効期限 == トークン発行日時
        Exception ex = Assert.ThrowsException<DomainException>(() =>
        {
            _ = new JWTToken(Guid.NewGuid().ToString(), "t", issuedAt, same);
        });
        // メッセージを検証する
        Assert.AreEqual("トークンの有効期限は将来の日付である必要があります。", ex.Message);
        // トークン有効期限 < トークン発行日時
        ex = Assert.ThrowsException<DomainException>(() =>
        {
            _ = new JWTToken(Guid.NewGuid().ToString(), "t", issuedAt, issuedAt.AddSeconds(-1));
        });
        // メッセージを検証する
        Assert.AreEqual("トークンの有効期限は将来の日付である必要があります。", ex.Message);
    }

    [TestMethod("トークンが有効期限内の場合trueを返し、期限外の場合falseを返す")]
    public void IsValid_ShouldBeTrueBeforeExpiry_AndFalseAfter()
    {
        var issuedAt = DateTimeOffset.UtcNow;
        var expiresAt = issuedAt.AddSeconds(2);
        var jwt = new JWTToken(Guid.NewGuid().ToString(), "tkn", issuedAt, expiresAt);
        // 期限内のためtrueを返すことを検証する
        Assert.IsTrue(jwt.IsValid(DateTimeOffset.UtcNow));
        // 10ミリ秒時間を進める
        var after = expiresAt.AddMilliseconds(10);
        // 期限外のためfalseを返すことを検証する
        Assert.IsFalse(jwt.IsValid(after));
    }

    [TestMethod("インスタンスが同一の場合trueを返し、異なる場合falseを返す")]
    public void Equals_ShouldBeReferenceAwareAndUseTokenId()
    {
        var issuedAt = DateTimeOffset.UtcNow;
        var expiresAt = issuedAt.AddMinutes(1);
        var a = new JWTToken(Guid.NewGuid().ToString(), "tknA", issuedAt, expiresAt);
        var b = new JWTToken(Guid.NewGuid().ToString(), "tknB", issuedAt, expiresAt);

        // 同一イスンタンスなのでtrueになることを検証する
        Assert.IsTrue(a.Equals(a));
        // 異なるインスタンスなのでfalseになることを検証する
        Assert.IsFalse(a.Equals(b));
            Assert.AreNotEqual(a.GetHashCode(), b.GetHashCode());
    }
}