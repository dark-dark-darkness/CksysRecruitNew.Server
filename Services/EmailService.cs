using CksysRecruitNew.Server.Helper;
using CksysRecruitNew.Server.Options;

using MailKit.Net.Smtp;

using Microsoft.Extensions.Options;

using MimeKit;

namespace CksysRecruitNew.Server.Services;

public sealed class EmailService {

  private readonly ISmtpClient _smtpClient;

  private readonly SmtpOptions _options;

  private readonly ILogger<EmailService> _logger;

  public EmailService(IOptions<SmtpOptions> options, ILogger<EmailService> logger, ISmtpClient smtpClient) {
    _options = options.Value;
    _logger = logger;
    _smtpClient = smtpClient;
  }

  public async Task<bool> SeedAsync(string email, string name) {
    var message = new MimeMessage();

    message.From.Add(new MailboxAddress(_options.Name, _options.Address));

    message.To.Add(new MailboxAddress("申请人", name));

    message.Subject = "创客实验室招新";

    var bodyBuilder = new BodyBuilder();

    bodyBuilder.HtmlBody = EmailHelper.GetEmail(name);

    message.Body = bodyBuilder.ToMessageBody();

    await _smtpClient.SendAsync(message);

    _logger.LogInformation("Send to {Name}({Address}) success", name, email);

    return true;
  }

}
