using SqlSugar;

namespace CksysRecruitNew.Server.Entities;

[SugarTable("t_PhoneCaptcha")]
public class PhoneCaptcha {
  public string Phone { get; set; }

  public string Captcha { get; set; }

  public DateTime ExpiresTime { get; set; } = DateTime.UtcNow.AddMinutes(5);
}
