namespace CksysRecruitNew.Server.Options;

public sealed class SmsOptions {
  public const string SectionKey = "SmsOptions";

  /// <summary>
  /// 
  /// </summary>
  public string AccessKeyId { get; set; } = null!;

  /// <summary>
  /// 
  /// </summary>
  public string AccessKeySecret { get; set; } = null!;

  /// <summary>
  /// 
  /// </summary>
  public string Endpoint { get; set; } = null!;
}
