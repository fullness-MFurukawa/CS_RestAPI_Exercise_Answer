using Microsoft.EntityFrameworkCore;
using RestAPI_Exercise.Infrastructure.Contexts;
using RestAPI_Exercise.Infrastructure.Adapters;
using RestAPI_Exercise.Application.Domains.Repositories;
using RestAPI_Exercise.Infrastructure.Repositories;
using RestAPI_Exercise.Application.Usecases;
using RestAPI_Exercise.Infrastructure.Shared;
using RestAPI_Exercise.Application.Usecases.Products.Interfaces;
using RestAPI_Exercise.Application.Usecases.Products.Interactors;
namespace RestAPI_Exercise.Presentation.Configs;
/// <summary>
/// 依存関係(DI)の設定
/// インフラストラクチャ層、アプリケーション層、プレゼンテーション層
/// をまとめて追加する拡張クラス
/// </summary>
public static class ApplicationDependencyExtensions
{
    /// <summary>
    /// アプリ全体の依存関係を一括追加する拡張メソッド
    /// </summary>
    /// <param name="services">サービスコレクション</param>
    /// <param name="config">構成情報</param>
    /// <returns>IServiceCollection(チェーン可能)</returns>
    public static IServiceCollection AddApplicationDependencies(
        this IServiceCollection services, IConfiguration config)
    {
        // インフラストラクチャ層の依存関係を追加
        services.AddInfrastructureDependencies(config);
        // アプリケーション層の依存関係を追加
        services.AddApplicationLayerDependencies();
        // プレゼンテーション層の依存関係を追加
        services.AddPresentationLayerDependencies();
        return services;
    }

    /// <summary>
    /// インフラストラクチャ層の依存関係を追加
    /// </summary>
    /// <param name="services">依存関係注入(DI)のサービスコレクション</param>
    /// <param name="config">アプリケーションの設定情報を管理</param>
    /// <returns></returns>
    private static IServiceCollection AddInfrastructureDependencies(
        this IServiceCollection services, IConfiguration config)
    {
        // MySQLの接続文字列を設定ファイルから取得する
        var connectstr = config.GetConnectionString("MySqlConnection");
        // AddDbContextをサービスコレクションに登録する
        services.AddDbContext<AppDbContext>(options =>
        {
            // データベース操作ログをデバッグレベルでコンソールに出力する
            options.LogTo(Console.WriteLine, LogLevel.Debug);
            // MySQLのデータベースを指定された接続文字列を使用して構成
            // AutoDetectは接続文字列を基にMySQLのサーバーバージョンを自動的に検出
            options.UseMySql(connectstr, ServerVersion.AutoDetect(connectstr));
        });
        // ドメインオブジェクト:ProductSctockとProductStockEntityの相互変換クラス
        services.AddScoped<ProductStockEntityAdapter>();
        // ドメインオブジェクト:ProductCategoryとProductCategoryEntityの相互変換クラス
        services.AddScoped<ProductCategoryEntityAdapter>();
        // ドメインオブジェクト:ProductとProductEntityの相互変換クラス
        services.AddScoped<ProductEntityAdapter>();
        // 商品、商品カテゴリ、商品在庫オブジェクトの相互変換Factoryクラス
        services.AddScoped<ProductFactory>();
        // ドメインオブジェクト:商品カテゴリのCRUD操作Repositoryインターフェイス
        services.AddScoped<IProductCategoryRepository, ProductCategoryRepository>();
        // ドメインオブジェクト:商品のCRUD操作Repositoryインターフェイス
        services.AddScoped<IProductRepository, ProductRepository>();
        // Unit of Workパターンを利用したトランザクション制御インターフェイス
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        return services;
    }

    /// <summary>
    /// アプリケーション層の依存関係を追加
    /// </summary>
    /// <param name="services">依存関係注入(DI)のサービスコレクション</param>
    /// <returns></returns>
    private static IServiceCollection AddApplicationLayerDependencies(
        this IServiceCollection services)
    {
        // ユースケース:[新商品を登録する]を実現するインターフェイス
        services.AddScoped<IRegisterProductUsecase, RegisterProductUsecase>();
        // ユースケース:[商品を変更する]を実現するインターフェイス
        services.AddScoped<IUpdateProductUsecase, UpdateProductUsecase>();
        // ユースケース:[商品をキーワード検索する]を実現するインターフェイス
        services.AddScoped<ISearchProductByKeywordUsecase, SearchProductByKeywordUsecase>();
        return services;
    }

    /// <summary>
    /// プレゼンテーション層の依存関係を追加
    /// </summary>
    private static IServiceCollection AddPresentationLayerDependencies(this IServiceCollection services)
    {
        return services;
    }

    /// <summary>
    /// テストプロジェクトにServiceProviderを提供するヘルパメソッド
    /// </summary>
    /// <param name="config"></param>
    /// <param name="configureServices"></param>
    /// <param name="configureLogging"></param>
    /// <returns></returns>
    public static ServiceProvider BuildAppProvider(
       IConfiguration config,
       Action<IServiceCollection>? configureServices = null,
       Action<ILoggingBuilder>? configureLogging = null)
    {
        var services = new ServiceCollection();
        services.AddLogging(b =>
        {
            if (configureLogging is not null) configureLogging(b);
            else b.AddConsole().SetMinimumLevel(LogLevel.Warning);
        });
        services.AddApplicationDependencies(config);
        configureServices?.Invoke(services);

        return services.BuildServiceProvider(validateScopes: true);
    }
}