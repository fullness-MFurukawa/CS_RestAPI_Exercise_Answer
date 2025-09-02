using Microsoft.EntityFrameworkCore;
using RestAPI_Exercise.Infrastructure.Entities;
namespace RestAPI_Exercise.Infrastructure.Contexts;
/// <summary>
/// アプリケーション用データベースコンテキスト（MySQL対応）
/// </summary>
public class AppDbContextBackup : DbContext
{
    /// <summary>
    /// コンストラクタ
    /// </summary>
    /// <param name="options"></param>
    public AppDbContextBackup(DbContextOptions<AppDbContextBackup> options) : base(options) { }
    /// <summary>
    /// 商品テーブルアクセスプロパティ
    /// </summary>
    public DbSet<ProductEntity> Products => Set<ProductEntity>();
    /// <summary>
    /// 商品カテゴリテーブルアクセスプロパティ
    /// </summary>
    public DbSet<ProductCategoryEntity> ProductCategories => Set<ProductCategoryEntity>();
    /// <summary>
    /// 商品在庫テーブルアクセスプロパティ
    /// </summary>
    public DbSet<ProductStockEntity> ProductStocks => Set<ProductStockEntity>();

    // TODO: Fluent API でマッピングを定義する
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // 1) Category(1) – Product(N)
        modelBuilder.Entity<ProductEntity>()
            .HasOne(p => p.ProductCategory)       // Productは1つのCategoryを参照
            .WithMany(c => c.Products!)           // Categoryは複数Productを持つ(Category側にコレクションがある)
            .HasForeignKey(p => p.ProductCategoryId) // 外部キー結合定義
            .HasConstraintName("product_ibfk_category");// 外部キー名

        // ProductとProductStockの1:1結合
        modelBuilder.Entity<ProductEntity>()
            .HasOne(p => p.ProductStock)          // Productは1つの Stockを持つ
            .WithOne(s => s.Product!)             // Stockは1つのProductに対応する
            .HasForeignKey<ProductStockEntity>(s => s.ProductId) // 外部キー結合定義
            .HasConstraintName("product_stock_ibfk_product"); // 外部キー名

        // ユニークキー等の制約
        modelBuilder.Entity<ProductEntity>()
            .HasIndex(p => p.ProductUuid).IsUnique();
        modelBuilder.Entity<ProductCategoryEntity>()
            .HasIndex(c => c.CategoryUuid).IsUnique();
        modelBuilder.Entity<ProductStockEntity>()
            .HasIndex(s => s.StockUuid).IsUnique();
    }
}