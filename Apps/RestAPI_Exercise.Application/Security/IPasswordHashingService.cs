using RestAPI_Exercise.Application.Domains.Models;

namespace RestAPI_Exercise.Application.Security;
/// <summary>
/// パスワードのハッシュ化と検証機能を提供するインターフェイス
/// </summary>
public interface IPasswordHashingService
{
    /// <summary>
    /// 平文のパスワードをハッシュ化する
    /// </summary>
    /// <param name="user">平文のパスワードを保持したUser</param>
    /// <returns></returns>
    void Hash(User user);
    
    /// <summary>
    /// パスワードの比較結果を返す
    /// </summary>
    /// <param name="user">ドメインオブジェクト:ユーザー</param>
    /// <param name="providedPassword">平文のパスワード</param>
    /// <returns>true:一致、false:不一致</returns>
    /// <exception cref="PasswordRehashNeededException">
    /// 　パスワードは一致したが、ハッシュの形式や強度が古い場合にスローされる
    /// </exception>
    bool Verify(User user, string providedPassword);
}