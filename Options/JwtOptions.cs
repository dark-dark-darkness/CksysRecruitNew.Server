namespace CksysRecruitNew.Server.Options;

public class JwtOptions {

  public const string SectionKey = "JwtOptions";
  /// <summary>
  /// 
  /// </summary>
  public string Issuer { get; set; } = null!;
  /// <summary>
  /// 
  /// </summary>
  public string Audience { get; set; } = null!;
  /// <summary>
  /// 密钥
  /// </summary>
  public string SecretKey { get; set; } = null!;
  /// <summary>
  /// 过期时间
  /// </summary>
  public int ExpiresMinutes { get; set; } = 5;
}
