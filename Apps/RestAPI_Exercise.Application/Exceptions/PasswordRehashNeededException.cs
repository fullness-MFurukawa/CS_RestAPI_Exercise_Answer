namespace RestAPI_Exercise.Application.Exceptions;
/// <summary>
/// パスワードは一致したが、ハッシュの形式や強度が古い 警告的な例外クラス
/// </summary>
public class PasswordRehashNeededException : Exception
{
    public PasswordRehashNeededException(string message) : base(message) { }
}
