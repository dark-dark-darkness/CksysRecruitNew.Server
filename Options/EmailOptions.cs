namespace CksysRecruitNew.Server.Options;

public class SmtpOptions {

  public const string SectionKey = "SmtpOptions";

  /// <summary>
  /// 服务主机
  /// </summary>
  public string Host { get; set; } = null!;

  /// <summary>
  /// 服务端口
  /// </summary>
  public int Port { get; set; }

  /// <summary>
  /// 是否使用ssl
  /// </summary>
  public bool UseSsl { get; set; } = true;

  /// <summary>
  /// 账号姓名
  /// </summary>
  public string Username { get; set; } = null!;

  /// <summary>
  /// 密码
  /// </summary>
  public string Password { get; set; } = null!;

  /// <summary>
  /// 姓名
  /// </summary>
  public string Name { get; set; } = null!;

  /// <summary>
  /// 邮箱地址
  /// </summary>
  public string Address { get; set; } = null!;

}
