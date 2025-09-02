using RestAPI_Exercise.Application.Domains.Models;
namespace RestAPI_Exercise.Application.Domains.Repositories;
/// <summary>
///  ドメインオブジェクト:商品在庫のCRUD操作インターフェイス
/// </summary>
public interface IProductStockRepository
{
    /// <summary>
    /// 商品在庫を永続化する
    /// </summary>
    /// <param name="productStock">商品在庫</param>
    /// <returns>なし</returns>
    Task CreateAsync(ProductStock productStock);
    
    /// <summary>
    /// 商品在庫を更新する
    /// </summary>
    /// <param name="productStock">更新対象の商品在庫</param>
    /// <returns>true:更新成功 false:更新失敗</returns>
    Task<bool> UpdateByIdAsync(ProductStock productStock);
}