using CksysRecruitNew.Server.Options;

using MailKit.Net.Smtp;

using Microsoft.Extensions.Options;

using SqlSugar;

namespace CksysRecruitNew.Server.Extensions;

public static class ServiceCollectionExtensions {
    public static IServiceCollection AddSqlSugarClient(this IServiceCollection services, string connectionString)
      => services.AddSingleton<ISqlSugarClient>((sp) =>
      new SqlSugarScope(new ConnectionConfig {
          DbType = DbType.MySql,
          IsAutoCloseConnection = true,
          ConnectionString = connectionString
      }, db => {
          db.Aop.OnLogExecuted = (sql, parms) => {
              var logger = sp.GetRequiredService<ILogger<ISqlSugarClient>>();
              logger.LogInformation("Executed SQL\n{sql} in {time}ms", sql, db.Ado.SqlExecutionTime.TotalMilliseconds);
          };

      }));

    public static IServiceCollection AddSmtpClient(this IServiceCollection services) {
        services.AddScoped(sp => {
            var client = new SmtpClient {
                ServerCertificateValidationCallback = (s, c, h, e) => true
            };
            client.AuthenticationMechanisms.Remove("XOAUTH2");
            var options = sp.GetRequiredService<IOptions<SmtpOptions>>().Value;
            client.Connect(options.Host, options.Port, options.UseSsl);
            client.Authenticate(options.Username, options.Password);
            return client;
        });

        return services;
    }


}
