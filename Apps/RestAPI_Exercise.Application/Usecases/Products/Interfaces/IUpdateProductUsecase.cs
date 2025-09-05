using RestAPI_Exercise.Application.Domains.Models;
namespace RestAPI_Exercise.Application.Usecases.Products.Interfaces;
/// <summary>
/// ユースケース:[商品を変更する]を実現するインターフェイス
/// </summary>
public interface IUpdateProductUsecase
{
    /// <summary>
    /// 指定された商品Idの商品を取得する
    /// クライアント側の[入力画面]で利用するため
    /// </summary>
    /// <param name="id">商品Id</param>
    /// <returns>該当商品、商品在庫、商品カテゴリ</returns>
    /// <exception cref="NotFoundException">該当データが存在しない場合にスローされる</exception>
    Task<Product> GetProductByIdAsync(string id);   


    /// <summary>
    /// 商品を変更するする
    /// </summary>
    /// <param name="product">変更対象対象商品</param>
    /// <returns>なし</returns>
    /// <exception cref="NotFoundException">商品が存在しない場合にスローされる</exception>
    Task UpdateProductAsync(Product product);
}