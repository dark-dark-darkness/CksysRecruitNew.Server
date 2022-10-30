using SqlSugar;

namespace CksysRecruitNew.Server.Extensions;

/// <summary>
/// 
/// </summary>
public static class WebApplicationExtensions {
  /// <summary>
  /// 初始化数据库
  /// </summary>
  /// <param name="app"></param>
  /// <param name="entities"></param>
  /// <returns></returns>
  public static WebApplication InitDatabase(this WebApplication app, params Type[] entities) {
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<ISqlSugarClient>>();
    logger.LogInformation("开始初始化数据库");
    db.DbMaintenance.CreateDatabase();
    db.CodeFirst.InitTables(entities);
    return app;
  }

}
