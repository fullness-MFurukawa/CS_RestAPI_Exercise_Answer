using Microsoft.AspNetCore.Mvc;
using RestAPI_Exercise.Application.Domains.Models;
using RestAPI_Exercise.Application.Exceptions;
using RestAPI_Exercise.Application.Usecases.Products.Interfaces;
using RestAPI_Exercise.Presentation.Adapters;
using RestAPI_Exercise.Presentation.ViewModels;
namespace RestAPI_Exercise.Presentation.Controllers;
/// <summary>
/// ユースケース:[新商品を登録する]を実現するコントローラ
/// </summary>
[ApiController]
[Route("api/products/register")]
public class RegisterProductController : ControllerBase
{
    private readonly IRegisterProductUsecase _usecase;
    private readonly RegisterProductViewModelAdapter _adapter;
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="usecase">ユースケース:[新商品を登録する]を実現するインターフェイス</param>
    /// <param name="adapter">RegisterProductViewModelからドメインオブジェクト:Productへ変換するアダプタ</param>
    public RegisterProductController(
        IRegisterProductUsecase usecase,
        RegisterProductViewModelAdapter adapter)
    {
        _usecase = usecase;
        _adapter = adapter;
    }

    /// <summary>
    /// 商品カテゴリ一覧の取得
    /// </summary>
    /// <returns></returns>
    [HttpGet("categories")]
    [ProducesResponseType(typeof(List<ProductCategory>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCategories()
    {
        var result = await _usecase.GetCategoriesAsync();
        return Ok(result);
    }

    /// <summary>
    /// 選択された商品カテゴリIdで商品カテゴリを取得する取得する
    /// </summary>
    /// <param name="categoryId">商品カテゴリId(UUID)</param>
    /// <returns>該当するカテゴリが存在すればOK(200)、存在しなければNotFound(404)</returns>
    [HttpGet("categories/{categoryId}")]
    [ProducesResponseType(typeof(ProductCategory), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetCategoryById(string categoryId)
    {
        try
        {
            var category = await _usecase.GetCategoryByIdAsync(categoryId);
            return Ok(category);
        }
        catch (NotFoundException ex)
        {
            // エラーレスポンスを返却する
            return NotFound(new
            { code = "CATEGORY_NOT_FOUND", message = ex.Message });
        }
    }

    /// <summary>
    /// 商品が既に存在するかを検証する
    /// </summary>
    /// <param name="productName">検証対象の商品名</param>
    /// <returns>
    /// 存在しない場合:Ok(200)、存在する場合:Conflict(409) 
    /// </returns>
    [HttpGet("validate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> ValidateProduct([FromQuery] string productName)
    {
        // 商品名がnullか空白
        if (string.IsNullOrWhiteSpace(productName))
        {
            return BadRequest(new
            { code = "INVALID_PRODUCT_NAME", message = "商品名は必須です。" });
        }
        try
        {
            // 商品名の存在有無を調べる
            await _usecase.ExistsByProductNameAsync(productName);
            return Ok(new { exists = false });
        }
        catch (ExistsException ex)
        {
            // 商品が既に存在する場合
            return Conflict(new
            { code = "PRODUCT_ALREADY_EXISTS", message = ex.Message });
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns> <summary>
    [HttpPost]
    [ProducesResponseType(typeof(Product), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterProductViewModel model)
    {
        // サーバーサイドバリデーション
        if (!ModelState.IsValid)
        {
            // プロパティ名をキー、エラーメッセージ配列を値とするディクショナリに変換する
            var details = ModelState
                .Where(kv => kv.Value?.Errors.Count > 0) // エラーがある項目だけを抽出する
                .ToDictionary( // Dictionaryに変換する
                               // キー:プロパティ名 ("Name", "Price" など)
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
            // 存在しない商品カテゴリを受信した(ミスしている)
            await _usecase.GetCategoryByIdAsync(model.CategoryId);
            // 既に登録済みの商品を受信した(ミスしている)
            await _usecase.ExistsByProductNameAsync(model.Name);
            // RegisterProductViewModelからProductを復元する
            var product = await _adapter.RestoreAsync(model);
            // 商品を永続化する
            await _usecase.RegisterProductAsync(product);
            return Created($"/api/products/{product.ProductUuid}", product.ProductUuid);
        }
        catch (ExistsException ex)
        {
            // 既に存在する商品を受信した
            return Conflict(new { code = "PRODUCT_ALREADY_EXISTS", message = ex.Message });
        }
        catch (NotFoundException ex)
        {
            // 存在しない商品カテゴリIdを受信した
            return NotFound(new { code = "CATEGORY_NOT_FOUND", message = ex.Message });
        }
        catch (DomainException ex)
        {
            // 業務ルール違反のデータを受信した
            return BadRequest(new { code = "DOMAIN_RULE_VIOLATION", message = ex.Message });
        }
    }
}