using CksysRecruitNew.Server.Options;

using Microsoft.Extensions.Options;

namespace CksysRecruitNew.Server.Services;

public class SmsNotifyService : INotifyService {
  private readonly ILogger<SmsNotifyService> _logger;
  private readonly SmsOptions _options;

  public SmsNotifyService(ILogger<SmsNotifyService> logger, IOptions<SmsOptions> options) {
    _logger = logger;
    _options = options.Value;
  }

  public async Task SeedAsync(string phone, string name) {
    var client = CreateClient(_options.AccessKeyId, _options.AccessKeySecret);

    var sendSmsRequest = new AlibabaCloud.SDK.Dysmsapi20170525.Models.SendSmsRequest {
      SignName = _options.SignName,
      TemplateCode = _options.TemplateCode,
      PhoneNumbers = phone,
    };

    var runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions();

    await client.SendSmsWithOptionsAsync(sendSmsRequest, runtime);
  }

  private AlibabaCloud.SDK.Dysmsapi20170525.Client CreateClient(string accessKeyId, string accessKeySecret) {

    var config = new AlibabaCloud.OpenApiClient.Models.Config {
      // 您的 AccessKey ID
      AccessKeyId = accessKeyId,
      // 您的 AccessKey Secret
      AccessKeySecret = accessKeySecret,
    };

    // 访问的域名
    config.Endpoint = _options.Endpoint;
    return new AlibabaCloud.SDK.Dysmsapi20170525.Client(config);

  }
}
