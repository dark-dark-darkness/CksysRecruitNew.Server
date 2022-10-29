namespace CksysRecruitNew.Server.Options;

public class SmtpOptions {

  public const string SectionKey = "SmtpOptions";

  public string Host { get; set; } = null!;

  public int Port { get; set; }

  public bool UseSsl { get; set; } = true;

  public string Username { get; set; } = null!;

  public string Password { get; set; } = null!;

  public string Name { get; set; } = null!;

  public string Address { get; set; } = null!;

}
