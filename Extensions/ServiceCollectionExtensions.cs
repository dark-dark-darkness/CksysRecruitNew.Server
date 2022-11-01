using CksysRecruitNew.Server.Options;

using FreeRedis;

using Google.Protobuf.WellKnownTypes;

using MailKit.Net.Smtp;

using Microsoft.Extensions.Options;

using SqlSugar;

namespace CksysRecruitNew.Server.Extensions;

/// <summary>
/// 服务容器扩展方法
/// </summary>
public static class ServiceCollectionExtensions {
  /// <summary>
  /// 添加数据库
  /// </summary>
  /// <param name="services"></param>
  /// <param name="connectionString"></param>
  /// <returns></returns>
  public static IServiceCollection AddSqlSugarClient(this IServiceCollection services, string connectionString)
    => services.AddSingleton<ISqlSugarClient>((sp) =>
    new SqlSugarScope(new ConnectionConfig {
      DbType = DbType.MySql,
      IsAutoCloseConnection = true,
      ConnectionString = connectionString
    }, db => {
      db.Aop.OnLogExecuting = (sql, parms) => {
        var logger = sp.GetRequiredService<ILogger<ISqlSugarClient>>();
        logger.LogInformation("Executing SQL:\n{sql}", sql);
      };
      db.Aop.OnLogExecuted = (sql, parms) => {
        var logger = sp.GetRequiredService<ILogger<ISqlSugarClient>>();
        logger.LogInformation("Executed SQL in {time}ms", db.Ado.SqlExecutionTime.TotalMilliseconds);
      };

    }));

  /// <summary>
  /// 添加smtp客户端
  /// </summary>
  /// <param name="services"></param>
  /// <returns></returns>
  public static IServiceCollection AddSmtpClient(this IServiceCollection services) {
    services.AddSingleton<ISmtpClient>(sp => {
      var options = sp.GetRequiredService<IOptions<SmtpOptions>>().Value;
      var logger = sp.GetRequiredService<ILogger<ISmtpClient>>();
      var client = new SmtpClient {
        ServerCertificateValidationCallback = (s, c, h, e) => true
      };
      client.Connect(options.Host, options.Port, options.UseSsl);
      client.Authenticate(options.Username, options.Password);
      client.AuthenticationMechanisms.Remove("XOAUTH2");
      logger.LogInformation("Smtp Connection Success");
      return client;
    });
    return services;
  }

  /// <summary>
  /// 添加redis客户端
  /// </summary>
  /// <param name="services"></param>
  /// <param name="connectionString"></param>
  /// <returns></returns>
  public static IServiceCollection AddFreeRedis(this IServiceCollection services, string connectionString)
    => services.AddSingleton(sp => {
      var logger = sp.GetRequiredService<ILogger<RedisClient>>();
      var redisClient = new RedisClient(connectionString);
      redisClient.Notice += (sender, arg) => logger.LogInformation("@Log", arg.Log);
      return redisClient;
    });

}
