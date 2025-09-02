using Microsoft.EntityFrameworkCore;
using RestAPI_Exercise.Application.Domains.Models;
using RestAPI_Exercise.Application.Domains.Repositories;
using RestAPI_Exercise.Application.Exceptions;
using RestAPI_Exercise.Infrastructure.Adapters;
using RestAPI_Exercise.Infrastructure.Contexts;
namespace RestAPI_Exercise.Infrastructure.Repositories;
/// <summary>
///  ドメインオブジェクト:商品在庫のCRUD操作インターフェイスの実装
/// </summary>
public class ProductStockRepository : IProductStockRepository
{
    private readonly AppDbContext _context;
    private readonly ProductStockEntityAdapter _adapter;
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="context">アプリケーション用データベースコンテキスト</param>
    /// <param name="adapter">ドメインオブジェクト:ProductStockとProductStockEntityの相互変換クラス</param>
    public ProductStockRepository(
        AppDbContext context,
        ProductStockEntityAdapter adapter)
    {
        _context = context;
        _adapter = adapter;
    }

    /// <summary>
    /// 商品在庫を永続化する
    /// </summary>
    /// <param name="productStock">商品在庫</param>
    /// <returns>なし</returns>
    public async Task CreateAsync(ProductStock productStock)
    {
        try
        {
            // ProductStockをProductStockEntityに変換する
            var entity = await _adapter.ConvertAsync(productStock);
            // ProductStcokEntityが保持する値を登録する
            await _context.ProductStocks.AddAsync(entity);
            // 登録したレコードをデータベースに永続化する
            await _context.SaveChangesAsync();
        }
        catch (DomainException)
        {
            throw; // DomainException例外はそのまま再スローする
        }
        catch (Exception ex)
        {
            // InternalExceptionにラップしてスローする
            throw new InternalException("商品在庫の永続化中に予期しないエラーが発生しました。", ex);
        }
    }

    /// <summary>
    /// 商品在庫を更新する
    /// </summary>
    /// <param name="productStock">更新対象の商品在庫</param>
    /// <returns>true:更新成功 false:更新失敗</returns>
    public async Task<bool> UpdateByIdAsync(ProductStock productStock)
    {
        try
        {
            // 変更対象の商品在庫をUUIDをキーに取得する
            var entity = await _context.ProductStocks
                .FirstOrDefaultAsync(s => s.StockUuid == productStock.StockUuid);
            if (entity is null)
            {
                return false;  // 変更対象データが存在しない場合falseを返す
            }
            // 商品在庫数を変更する
            entity.Stock = productStock.Stock;
            // 変更したレコードをデータベースに永続化する
            await _context.SaveChangesAsync();
            return true;// 変更が成功した場合trueを返す
        }
        catch (Exception ex)
        {
            // InternalExceptionにラップしてスローする
            throw new InternalException("商品在庫の変更中に予期しないエラーが発生しました。", ex);
        }
    }
}