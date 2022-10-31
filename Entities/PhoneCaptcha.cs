using SqlSugar;

namespace CksysRecruitNew.Server.Entities;

[SugarTable(TableNameConsts.PhoneCaptcha)]
public class PhoneCaptcha {
  public string Phone { get; set; } = string.Empty;

  public string Captcha { get; set; } = string.Empty;

  public DateTime ExpiresTime { get; set; } = DateTime.Now.AddMinutes(1);
}
