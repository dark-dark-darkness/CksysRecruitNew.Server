using SqlSugar;

namespace CksysRecruitNew.Server.Entities;

[SugarTable("t_PhoneVerificationCode")]
public class PhoneVerificationCode {
  public string Phone { get; set; }

  public string VerificationCode { get; set; }

  public DateTime ExpiresTime { get; set; } = DateTime.UtcNow.AddMinutes(5);
}
