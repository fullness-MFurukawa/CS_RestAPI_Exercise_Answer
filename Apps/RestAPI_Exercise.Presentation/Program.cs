//using Microsoft.AspNetCore.Identity;
//using RestAPI_Exercise.Application.Domains.Models;
//using RestAPI_Exercise.Application.Security;
using RestAPI_Exercise.Presentation.Configs;


// --- ここから：一時的なハッシュ生成用コード ---
//var hasher = new PasswordHasher<User>();
//var hashingService = new PBKDF2PasswordHashingService(hasher);

//string hash1 = hashingService.Hash("passYamada");
//string hash2 = hashingService.Hash("passTanaka");

//Console.WriteLine("\n========== テスト用パスワードハッシュ ==========");
//Console.WriteLine($"ユーザー1 (passYamada) : {hash1}");
//Console.WriteLine($"ユーザー2 (passTanaka) : {hash2}");
//Console.WriteLine("================================================\n");
// --- ここまで ---


var builder = WebApplication.CreateBuilder(args);


// 依存関係(DI)の設定
ApplicationDependencyExtensions
    .AddApplicationDependencies(builder.Services, builder.Configuration);
// JWT認証ミドルウェアをサービス登録する
builder.Services.AddJwtAuthentication(builder.Configuration);
// Swagger(Open API)のサービス登録する
builder.Services.AddSwaggerWithJwt();

// Kestrelの設定をappsettings.jsonから読取り設定する
builder.WebHost.ConfigureKestrel(options =>
{
    options.Configure(builder.Configuration.GetSection("Kestrel"));
});

// WebApplicationを生成する
var app = builder.Build();


// 開発環境のみSwaggerを有効化
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "RestAPI Exercise v1");
        c.RoutePrefix = string.Empty; // ルートURLでUIを開く

        // UseRequestInterceptor メソッドを使用する
        c.UseRequestInterceptor("(request) => { request.credentials = 'include'; return request; }");
    });
}

// 例外ハンドリングを登録する
app.UseExceptionHandling();

// HTTPリクエストをHTTPSへ自動リダイレクトするKestrelミドルウェアを有効化
app.UseHttpsRedirection();
// HSTSを有効化
app.UseHsts();

// CORSを有効化する
app.UseCors(CorsServiceExtensions.GetPolicyName());

// 認証(Authentication)を有効化する
app.UseAuthentication();
// 認可(Authorization)を有効化する
app.UseAuthorization();

// Controllerのルーティングを有効化
app.MapControllers();
// アプリケーションを実行する
app.Run();