namespace CksysRecruitNew.Server.Options;

public class SmsOptions {

  public string SignName { get; set; } = null!;
  public string TemplateCode { get; set; } = null!;
  public string AccessKeyId { get; set; } = null!;
  public string AccessKeySecret { get; set; } = null!;
  public string Endpoint { get; set; } = null!;

}
