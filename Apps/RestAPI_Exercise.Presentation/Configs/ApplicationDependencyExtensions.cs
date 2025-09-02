namespace RestAPI_Exercise.Presentation.Configs;

/// <summary>
/// 依存関係(DI)の設定
/// インフラストラクチャ層、アプリケーション層、プレゼンテーション層をまとめて追加する拡張クラス
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
        // 各層の依存関係を追加
        services.AddInfrastructureDependencies(config);
        services.AddApplicationLayerDependencies();
        services.AddPresentationLayerDependencies();
        return services;
    }

    /// <summary>
    /// インフラストラクチャ層の依存関係を追加
    /// </summary>
    private static IServiceCollection AddInfrastructureDependencies(
        this IServiceCollection services, IConfiguration config)
    {
        return services;
    }

    /// <summary>
    /// アプリケーション層の依存関係を追加
    /// </summary>
    private static IServiceCollection AddApplicationLayerDependencies(this IServiceCollection services)
    {
        return services;
    }

    /// <summary>
    /// プレゼンテーション層の依存関係を追加
    /// </summary>
    private static IServiceCollection AddPresentationLayerDependencies(this IServiceCollection services)
    {
        return services;
    }
}