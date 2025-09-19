using Microsoft.AspNetCore.Identity;
using RestAPI_Exercise.Application.Domains.Models;
using RestAPI_Exercise.Application.Exceptions;
namespace RestAPI_Exercise.Application.Security;
/// <summary>
///  PBKDF2アルゴリズムを利用
/// パスワードのハッシュ化と検証機能を提供するインターフェイスの実装
/// </summary>
public class PBKDF2PasswordHashingService : IPasswordHashingService
{
    private readonly IPasswordHasher<User> _passwordHasher;
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="passwordHasher">ASP.NET Core Identityのパスワードハッシュ化・検証</param>
    public PBKDF2PasswordHashingService(IPasswordHasher<User> passwordHasher)
    {
        _passwordHasher = passwordHasher;
    }

    /// <summary>
    /// 平文のパスワードをハッシュ化する
    /// </summary>
    /// <param name="user">平文のパスワードを保持したUser</param>
    /// <returns></returns>
    public void Hash(User user)
    {
        // パスワードをハッシュ化する
        var hashPassword = _passwordHasher.HashPassword(user, user.Password);
        // ハッシュ化したパスワードに変更する
        user.ChangePassword(hashPassword);
    }

    /// <summary>
    /// パスワードの比較結果を返す
    /// </summary>
    /// <param name="user">ドメインオブジェクト:ユーザー</param>
    /// <param name="providedPassword">平文のパスワード</param>
    /// <returns>true:一致、false:不一致</returns>
    /// <exception cref="PasswordRehashNeededException">
    /// 　パスワードは一致したが、ハッシュの形式や強度が古い場合にスローされる
    /// </exception>
    public bool Verify(User user, string providedPassword)
    {
        // パスワードを比較検証する
        var result =
        _passwordHasher.VerifyHashedPassword(user, user.Password, providedPassword);
        return result switch
        {
            // 一致したのtrueを返す
            PasswordVerificationResult.Success => true,
            // 不一致なのでfalseを返す
            PasswordVerificationResult.Failed => false,
            // 一致したが形式や強度が古いので、 PasswordRehashNeededExceptionをスローする
            PasswordVerificationResult.SuccessRehashNeeded =>
                throw new PasswordRehashNeededException("パスワードは認証されたが、再ハッシュが必要です。"),
                _ => false
        };
    }
}