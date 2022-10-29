namespace CksysRecruitNew.Server.Options;

public class JwtOptions {

  public const string SectionKey = "JwtOptions";

  public string Issuer { get; set; } = null!;

  public string Audience { get; set; } = null!;

  public string SecretKey { get; set; } = null!;

  public int ExpiresMinutes { get; set; } = 5;
}
