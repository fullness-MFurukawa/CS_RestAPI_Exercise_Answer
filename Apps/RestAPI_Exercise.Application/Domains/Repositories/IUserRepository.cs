using RestAPI_Exercise.Application.Domains.Models;
namespace RestAPI_Exercise.Application.Domains.Repositories;
/// <summary>
/// ドメインオブジェクト:User(ユーザー)のCRUD操作インターフェイス
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// ユーザーを永続化する
    /// </summary>
    /// <param name="user">永続化するユーザー</param>
    /// <returns>なし</returns>
    Task CreateAsync(User user);

    /// <summary>
    /// ユーザー名またはメールアドレスが既に存在するか確認する
    /// </summary>
    /// <param name="username">ユーザー名</param>
    /// <param name="email">メールアドレス</param>
    /// <returns>true:存在する false:存在しない</returns>
    Task<bool> ExistsByUsernameOrEmailAsync(string username, string email);

    /// <summary>
    /// メールアドレスからユーザーを取得する(ログイン用)
    /// </summary>
    Task<User?> SelectByEmailAsync(string email);

    /// <summary>
    /// ユーザーId(UUID)からユーザーを取得する
    /// </summary>
    /// <returns>存在する場合:ドメインオブジェクト:User 存在しない場合:null</returns>
    Task<User?> SelectByIdAsync(string useruuid);

    /// <summary>
    /// ユーザー名またはパスワードからユーザーを取得する
    /// </summary>
    /// <param name="usernameOrEmail">ユーザー名またはメールアドレス</param>
    /// <returns>存在する場合:ドメインオブジェクト:User 存在しない場合:null</returns>
    Task<User?> FindByUsernameOrEmailAsync(string usernameOrEmail);
}