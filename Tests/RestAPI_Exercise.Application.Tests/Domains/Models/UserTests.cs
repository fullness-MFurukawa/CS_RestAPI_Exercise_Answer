using RestAPI_Exercise.Application.Domains.Models;
using RestAPI_Exercise.Application.Exceptions;
namespace RestAPI_Exercise.Application.Tests.Domains.Models;
/// <summary>
/// Userクラスの単体テストドライバ
/// </summary>
[TestClass]
[TestCategory("Domains/Models")]
public class UserTests
{
    // ヘルパー：有効値の定義
    private static string ValidUsername => "taro";
    private static string ValidEmail => "taro@example.com";
    private static string ValidPasswordHash => "hashed-password";

    [TestMethod("コンストラクタに正常値を指定するとインスタンス生成される")]
    public void Constructor_WithValidValues_ShouldCreateInstance()
    {
        // 新しいユーザーIdを生成する
        var id = Guid.NewGuid().ToString();
        // Userを生成する
        var user = new User(id, ValidUsername, ValidEmail, ValidPasswordHash);
        // ユーザーIdを検証する
        Assert.AreEqual(id, user.UserUuid);
        // ユーザー名を検証する
        Assert.AreEqual(ValidUsername, user.Username);
        // メールアドレスを検証する
        Assert.AreEqual(ValidEmail, user.Email);
        // パスワードを検証する
        Assert.AreEqual(ValidPasswordHash, user.Password);
    }

    [TestMethod("新規作成の場合UUIDが自動生成される")]
    public void NewInstance_ShouldGenerateUuidAutomatically()
    {
        // 新しいUserを生成する
        var user = new User(ValidUsername, ValidEmail, ValidPasswordHash);
        // ユーザーIdがUUIDであることを検証する
        Assert.IsTrue(Guid.TryParse(user.UserUuid, out _));
        // ユーザー名を検証する
        Assert.AreEqual(ValidUsername, user.Username);
        // メールアドレスを検証する
        Assert.AreEqual(ValidEmail, user.Email);
        // パスワードを検証する
        Assert.AreEqual(ValidPasswordHash, user.Password);
    }

    [TestMethod("不正なUUIDの場合、DomainExceptionがスローされる")]
    public void InvalidUuid_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsException<DomainException>(() =>
        {
            _ = new User("invalid-uuid", ValidUsername, ValidEmail, ValidPasswordHash);
        });
        Assert.AreEqual("ユーザーIdはUUID形式でなければなりません。", ex.Message);
    }

    [TestMethod("ユーザー名が空白の場合、DomainExceptionがスローされる")]
    public void EmptyUsername_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsException<DomainException>(() =>
        {
            _ = new User(Guid.NewGuid().ToString(), "", ValidEmail, ValidPasswordHash);
        });
        Assert.AreEqual("ユーザー名は必須です。", ex.Message);
    }

    [TestMethod("ユーザー名が31文字以上の場合、DomainExceptionがスローされる")]
    public void UsernameLongerThan30_ShouldThrowDomainException()
    {
        var longName = new string('a', 31);
        var ex = Assert.ThrowsException<DomainException>(() =>
        {
            _ = new User(Guid.NewGuid().ToString(), longName, ValidEmail, ValidPasswordHash);
        });
        Assert.AreEqual("ユーザー名は30文字以内で指定してください。", ex.Message);
    }

    [TestMethod("メールアドレスが不正な形式の場合、DomainExceptionがスローされる")]
    public void InvalidEmail_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsException<DomainException>(() =>
        {
            _ = new User(Guid.NewGuid().ToString(), ValidUsername, "invalidEmail", ValidPasswordHash);
        });
        Assert.AreEqual("メールアドレスの形式が不正です。", ex.Message);
    }

    [TestMethod("メールアドレスが101文字以上の場合、DomainExceptionがスローされる")]
    public void EmailLongerThan100_ShouldThrowDomainException()
    {
        var longEmail = new string('a', 95) + "@x.com"; // 101文字以上
        var ex = Assert.ThrowsException<DomainException>(() =>
        {
            _ = new User(Guid.NewGuid().ToString(), "Taro", longEmail, "pwd");
        });
        Assert.AreEqual("メールアドレスは100文字以内で指定してください。", ex.Message);
    }

    [TestMethod("パスワードが空白の場合、DomainExceptionがスローされる")]
    public void EmptyPassword_ShouldThrowDomainException()
    {
        var ex = Assert.ThrowsException<DomainException>(() =>
        {
            _ = new User(Guid.NewGuid().ToString(), ValidUsername, ValidEmail, "");
        });
        Assert.AreEqual("パスワードハッシュは必須です。", ex.Message);
    }
}