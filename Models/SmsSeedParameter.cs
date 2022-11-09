namespace CksysRecruitNew.Server.Models;

public sealed class SmsSeedParameter {

  public string SignName { get; set; } = null!;

  public string TemplateCode { get; set; } = null!;

  public string ParameterString { get; set; } = null!;

  public string Phone { get; set; } = null!;



  public static SmsSeedParameter Captcha(string phone, string code)
    => new() {
      SignName = "liangjs",
      TemplateCode = "SMS_256725470",
      Phone = phone,
      ParameterString = $$"""{"code":{{ code}}  }"""
    };

}
