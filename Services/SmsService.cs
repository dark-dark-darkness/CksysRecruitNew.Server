using CksysRecruitNew.Server.Models;
using CksysRecruitNew.Server.Options;

using Microsoft.Extensions.Options;

namespace CksysRecruitNew.Server.Services;

public sealed class SmsService {
  private readonly ILogger<SmsService> _logger;

  private readonly SmsOptions _options;

  public SmsService(ILogger<SmsService> logger, IOptions<SmsOptions> options) {
    _logger = logger;
    _options = options.Value;
  }

  public async Task SeedAsync(SmsSeedParameter parameter) {
    var client = CreateClient(_options.AccessKeyId, _options.AccessKeySecret);

    var sendSmsRequest = new AlibabaCloud.SDK.Dysmsapi20170525.Models.SendSmsRequest {
      SignName = parameter.SignName,
      TemplateCode = parameter.TemplateCode,
      PhoneNumbers = parameter.Phone,
      TemplateParam = parameter.ParameterString
    };

    var runtime = new AlibabaCloud.TeaUtil.Models.RuntimeOptions();

    var resp = await client.SendSmsWithOptionsAsync(sendSmsRequest, runtime);

    _logger.LogInformation("seed success {@resp}", resp);
  }

  private AlibabaCloud.SDK.Dysmsapi20170525.Client CreateClient(string accessKeyId, string accessKeySecret) {

    var config = new AlibabaCloud.OpenApiClient.Models.Config {
      // 您的 AccessKey ID
      AccessKeyId = accessKeyId,
      // 您的 AccessKey Secret
      AccessKeySecret = accessKeySecret
    };

    // 访问的域名
    config.Endpoint = _options.Endpoint;
    return new AlibabaCloud.SDK.Dysmsapi20170525.Client(config);

  }
}
