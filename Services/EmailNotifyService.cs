using CksysRecruitNew.Server.Options;

using MailKit.Net.Smtp;

using Microsoft.Extensions.Options;

using MimeKit;

namespace CksysRecruitNew.Server.Services;

public class EmailService {

    private readonly SmtpClient _client;

    private readonly SmtpOptions _options;

    private readonly ILogger<EmailService> _logger;

    public EmailService(SmtpClient client, IOptions<SmtpOptions> options, ILogger<EmailService> logger) {
        _client = client;
        _options = options.Value;
        _logger = logger;
    }

    public async Task SeedAsync(string email, string name) {
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(_options.Name, _options.Address));

        message.To.Add(new MailboxAddress("申请人", name));

        message.Subject = "创客实验室招新";

        var bodyBuilder = new BodyBuilder();

        bodyBuilder.HtmlBody = $"""
      <div style="height: 260px;border-color: rgb(248, 135, 7);border-style: solid;border-width: 0 1px 5px 1px;">
      	<div style="height: auto;text-align: end;background-color: rgb(248, 135, 7);color: aliceblue; margin-bottom: 0;">
      		<div style="line-height: 50px;height: 50px;padding-right: 2rem;">软件开发创客实验室</div>
      	</div>
      	<div style="background-color: white; border-color: rgb(248, 135, 7); border: 2px;">
      		<div style="height: 100px;margin-left: 1rem;">
      			<div style="margin-top: 1rem;"><b>发送给</b> <b style="color: rgb(127, 140, 255);">{name}</b></div>
      			<div style="font-size: 14px;">
      				<div><small>软件开发创客实验室报名通知：</small></div>
      				<div style="margin-left: 1rem;padding-top: 2rem;"><small>您的申请已成功提交，请等待后续通知。</small></div>
      				<div style="margin-left: 1rem;"><small>如有疑问请联系管理员QQ：572917108</small></small></div>
      				<div style="text-align: end;margin-right: 2rem;margin-top: 1rem;"><small>此邮件为系统邮件，请勿直接回复</small></div>
      				<div style="text-align: start;"><b><small>软件开发创客实验室</small></b></div>
      			</div>
      		</div>
      	</div>
      </div>
      """;

        message.Body = bodyBuilder.ToMessageBody();
        await _client.SendAsync(message);

        _logger.LogInformation("Send to {Name}({Address}) success", name, email);

    }

}
