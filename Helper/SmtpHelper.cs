using CksysRecruitNew.Server.Options;

using MailKit.Net.Smtp;

using Microsoft.Extensions.Options;

namespace CksysRecruitNew.Server.Helper;

public sealed class SmtpHelper {

  private readonly SmtpOptions _options;
  private readonly ILogger<SmtpHelper> _logger;

  public SmtpHelper(SmtpOptions options, ILogger<SmtpHelper> logger) {
    _options = options;
    _logger = logger;
  }


  public ISmtpClient? CreateOrDefault() {
    var client = new SmtpClient {
      ServerCertificateValidationCallback = (s, c, h, e) => true
    };
    client.AuthenticationMechanisms.Remove("XOAUTH2");
    try {
      client.Connect(_options.Host, _options.Port, _options.UseSsl);
      client.Authenticate(_options.Username, _options.Password);
    } catch (Exception ex) {
      _logger.LogError(ex, "连接smtp客户端错误");
      return null;
    }
    return client;
  }
}
