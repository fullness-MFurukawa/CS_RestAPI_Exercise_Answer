using System.Security.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RestAPI_Exercise.Application.Security;
using RestAPI_Exercise.Application.Usecases.Authenticate.Interfaces;
using RestAPI_Exercise.Presentation.ViewModels;
using Swashbuckle.AspNetCore.Annotations;

namespace RestAPI_Exercise.Presentation.Controllers;
/// <summary>
/// ユースケース:[ログイン/ログアウト]を実現するコントローラ
/// </summary>
[ApiController]
[Route("api/auth")]
[SwaggerTag("ユーザー認証（ログイン/ログアウト）処理")]
public class AuthenticateController : ControllerBase
{
    private readonly IAuthenticateUserUsecase _usecase;
    private readonly IJwtTokenProvider _provider;
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="usecase">ユースケース:[ログインする]を実現するインターフェイス</param>
    /// <param name="provider">JWTの発行・検証インターフェイス</param>
    public AuthenticateController(
        IAuthenticateUserUsecase usecase, IJwtTokenProvider provider)
    {
        _usecase = usecase;
        _provider = provider;
    }

    /// <summary>
    /// ログイン認証し、成功したらJWTトークンを返す
    /// </summary>
    /// <param name="model">ログイン情報ViewModel</param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("login")]
    [SwaggerOperation(
        Summary = "ユーザーのログイン認証",
        Description = "ユーザー名またはメールアドレスとパスワードでログインを行い、JWTトークンを発行します。")]
    [SwaggerResponse(StatusCodes.Status200OK, "認証成功（JWTトークン返却）", typeof(TokenResponse))]
    [SwaggerResponse(StatusCodes.Status401Unauthorized, "認証失敗（ユーザーが存在しない、またはパスワード不一致）")]
    [SwaggerResponse(StatusCodes.Status400BadRequest, "バリデーションエラーまたは業務ルール違反")]
    public async Task<IActionResult> Login([FromBody] LoginViewModel model)
    {
        // サーバーサイドバリデーション
        if (!ModelState.IsValid)
        {
            // プロパティ名をキー、エラーメッセージ配列を値とするディクショナリに変換する
            var details = ModelState
                .Where(kv => kv.Value?.Errors.Count > 0) // エラーがある項目だけを抽出する
                .ToDictionary( // Dictionaryに変換する
                               // キー:プロパティ名 ("Username", "Email" など)
                    kv => kv.Key,
                    // 値: 当該プロパティのエラーメッセージ一覧
                    kv => kv.Value!.Errors
                        // エラーメッセージが空やnullの場合は "Invalid value."に置換する
                        .Select(e => string.IsNullOrWhiteSpace(e.ErrorMessage)
                            ? "Invalid value." : e.ErrorMessage)
                        .ToArray()
                );
            return BadRequest(new
            { code = "VALIDATION_ERROR", message = "入力内容に誤りがあります。", details });
        }
        try
        {
            // 認証ユーザーを取得する
            var user = await _usecase.AuthenticateAsync(model.UsernameOrEmail, model.Password);
            // JWTトークンを発行する
            var token = _provider.IssueAccessToken(user);
            return Ok(new TokenResponse { Token = token });
        }
        catch (AuthenticationException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    /// <summary>
    /// ログアウト(ステートレス: バックエンド側では何もせず204返却)
    /// </summary>
    [Authorize]
    [HttpPost("logout")]
    [SwaggerOperation(
        Summary = "ユーザーのログアウト",
        Description = "JWTはステートレスなため、バックエンド側で無効化処理は行いません。クライアント側でトークンを破棄してください。")]
    [SwaggerResponse(StatusCodes.Status204NoContent, "ログアウト成功（処理なし）")]
    public IActionResult Logout()
    {
        return NoContent();
    }
}