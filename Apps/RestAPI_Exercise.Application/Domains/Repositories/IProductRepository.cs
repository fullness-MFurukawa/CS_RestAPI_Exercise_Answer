using RestAPI_Exercise.Application.Domains.Models;
namespace RestAPI_Exercise.Application.Domains.Repositories;
/// <summary>
///  ドメインオブジェクト:商品のCRUD操作インターフェイス
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// 商品を永続化する
    /// </summary>
    /// <param name="product">永続化する商品</param>
    /// <returns>なし</returns>
    Task CreateAsync(Product product);

    /// <summary>
    /// 商品を更新する
    /// </summary>
    /// <param name="product">更新対象の商品</param>
    /// <returns>true:更新成功 false:更新失敗</returns>
    Task<bool> UpdateByIdAsync(Product product);

    /// <summary>
    /// 指定された商品Idの商品と在庫を返す
    /// </summary>
    /// <param name="id">商品Id</param>
    /// <returns>Product または null</returns>
    Task<Product> SelectByIdWithProductStockAsync(string id);

    /// <summary>
    /// 指定されたキーワードで商品を部分一致検索して商品と在庫を取得する
    /// </summary>
    /// <param name="keyword">検索キーワード</param>
    /// <returns>Prodyctのリスト</returns>
    Task<List<Product>> SelectByNameLikeWithProductStockAsync(string keyword);
}