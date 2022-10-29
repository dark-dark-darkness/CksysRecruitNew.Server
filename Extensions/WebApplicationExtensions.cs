using SqlSugar;

namespace CksysRecruitNew.Server.Extensions;

public static class WebApplicationExtensions
{
  public static WebApplication InitDatabase(this WebApplication app, params Type[] entities)
  {
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ISqlSugarClient>();
    db.DbMaintenance.CreateDatabase();
    db.CodeFirst.InitTables(entities);
    return app;
  }

}
