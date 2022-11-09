namespace CksysRecruitNew.Server.Models;

public sealed class TokenResult {
  public int Code { get; set; }

  public string Message { get; set; } = string.Empty;

  public string? Token { get; set; }
}
