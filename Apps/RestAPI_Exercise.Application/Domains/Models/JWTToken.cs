using RestAPI_Exercise.Application.Exceptions;
namespace RestAPI_Exercise.Application.Domains.Models;
/// <summary>
/// JWTトークンを表すドメインオブジェクト
/// </summary>
public class JWTToken
 {
    /// <summary>
    /// トークン識別子（UUID）
    /// </summary>
    public string TokenId { get; private set; } = string.Empty;
    /// <summary>
    /// トークンに紐づくユーザー識別子(UUID)
    /// </summary>
    public string UserId { get; private set; } = string.Empty;
    /// <summary>
    /// JWTアクセストークン本体(署名付きの文字列)
    /// </summary>
    public string Token { get; private set; } = string.Empty;
    /// <summary>
    /// トークンが発行された日時
    /// </summary>
    public DateTimeOffset IssuedAt { get; private set; }
    /// <summary>
    /// トークンの有効期限(この日時を過ぎると無効)
    /// </summary>
    public DateTimeOffset ExpiresAt { get; private set; }
    /// <summary>
    /// トークンが利用者または管理者によって無効化されたかどうか（ログアウトや強制失効など）
    /// </summary>
    public bool Revoked { get; private set; }
    /// <summary>
    /// トークンを発行したデバイスや環境の識別情報(IPやUAなど)
    /// </summary>
    public string? DeviceInfo { get; private set; }
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="userId">トークンに紐づくユーザー識別子(UUID)</param>
    /// <param name="token">JWTアクセストークン本体(署名付きの文字列)</param>
    /// <param name="issuedAt">トークンが発行された日時</param>
    /// <param name="expiresAt">トークンの有効期限(この日時を過ぎると無効)</param>
    /// <param name="deviceInfo">トークンを発行したデバイスや環境の識別情報(IPやUAなど)</param>
    public JWTToken(string userId, string token, DateTimeOffset issuedAt, DateTimeOffset expiresAt, string? deviceInfo = null)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            throw new DomainException("ユーザーIdは必須です。");
        }
        if (string.IsNullOrWhiteSpace(token))
        {
            throw new DomainException("JWTアクセストークンは必須です。");
        }
        if (expiresAt <= issuedAt)
        {
            throw new DomainException("トークンの有効期限は将来の日付である必要があります。");
        }
        TokenId = Guid.NewGuid().ToString();
        UserId = userId;
        Token = token;
        IssuedAt = issuedAt;
        ExpiresAt = expiresAt;
        DeviceInfo = deviceInfo;
    }

    /// <summary>
    /// このアクセストークンを「失効済み（Revoked）」状態にする
    /// ログアウト処理や管理者による強制無効化時に呼び出される
    /// </summary>
    public void Revoke()
    {
        Revoked = true;
    } 

    /// <summary>
    /// トークンが有効期限内かどうかを判定する
    /// </summary>
    public bool IsValid(DateTimeOffset now)
    {
        return !Revoked && now < ExpiresAt;
    }

    /// <summary>
    /// 識別子の等価性判定
    /// </summary>
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
            return obj is JWTToken other && TokenId == other.TokenId;
    }
    public override int GetHashCode() => TokenId.GetHashCode();

    /// <summary>
    /// インスタンスの内容
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        var preview = Token.Length > 16 ? Token[..16] + "..." : Token;
        return $"TokenId={TokenId}, UserId={UserId}, Token={preview}, "
        +"IssuedAt ={IssuedAt:O}, ExpiresAt={ExpiresAt:O}, "
        +"Revoked ={Revoked}, DeviceInfo={DeviceInfo}";
    }
}