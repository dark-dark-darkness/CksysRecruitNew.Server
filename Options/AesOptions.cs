namespace CksysRecruitNew.Server.Options;

public class AesOptions {

  public const string SectionKey = "AesOptions";
  /// <summary>
  /// 加密密钥
  /// </summary>
  public string EncryptKey { get; set; } = string.Empty;
}
