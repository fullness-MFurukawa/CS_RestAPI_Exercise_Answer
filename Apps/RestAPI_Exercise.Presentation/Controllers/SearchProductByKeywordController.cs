using Microsoft.AspNetCore.Mvc;
using RestAPI_Exercise.Application.Domains.Models;
using RestAPI_Exercise.Application.Usecases.Products.Interfaces;
namespace RestAPI_Exercise.Presentation.Controllers;
/// <summary>
/// ユースケース:[商品をキーワード検索する]を実現するコントローラ
/// </summary>
[ApiController]
[Route("api/products")]
public class SearchProductByKeywordController : ControllerBase
{
    private readonly ISearchProductByKeywordUsecase _usecase;
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="usecase">ユースケース:[商品をキーワード検索する]を実現するインターフェイス</param>
    public SearchProductByKeywordController(ISearchProductByKeywordUsecase usecase)
    {
        _usecase = usecase;
    }

    /// <summary>
    /// キーワードで商品を検索する
    /// </summary>
    /// <param name="keyword">検索キーワード</param>
    /// <returns>検索結果の商品一覧</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<Product>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Search([FromQuery] string keyword)
    {
        // 未入力チェック
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return BadRequest(
            new { code = "INVALID_KEYWORD", message = "検索キーワードを入力してください。" });
        }
        // 商品キーワード検索する
        var result = await _usecase.ExecuteAsync(keyword.Trim());
        return Ok(result);
    }
}