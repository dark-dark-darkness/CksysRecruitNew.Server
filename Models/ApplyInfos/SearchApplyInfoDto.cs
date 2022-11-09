using System.ComponentModel;
using System.Linq.Expressions;

using CksysRecruitNew.Server.Entities;

using SqlSugar;

namespace CksysRecruitNew.Server.Models.ApplyInfos;

public sealed class SearchApplyInfoDto {

  [DisplayName("学号")]
  public string Id { get; set; } = string.Empty;

  [DisplayName("姓名")]
  public string Name { get; set; } = string.Empty;

  [DisplayName("班级")]
  public string ClassName { get; set; } = string.Empty;

  [DisplayName("手机号")]
  public string Phone { get; set; } = string.Empty;

  [DisplayName("电子邮箱")]
  public string Email { get; set; } = string.Empty;

  [DisplayName("分数下限")]
  public double MinScore { get; set; } = 0d;

  [DisplayName("分数上限")]
  public double MaxScore { get; set; } = 0d;


  public Expression<Func<ApplyInfo, bool>> ToExpression()
    => Expressionable.Create<ApplyInfo>()
                    .AndIF(!string.IsNullOrWhiteSpace(Id), e => e.Id.Contains(Id))
                    .AndIF(!string.IsNullOrWhiteSpace(Name), e => e.Name.Contains(Name))
                    .AndIF(!string.IsNullOrWhiteSpace(ClassName), e => e.ClassName.Contains(ClassName))
                    .AndIF(!string.IsNullOrWhiteSpace(Phone), e => e.Phone.Contains(Phone))
                    .AndIF(!string.IsNullOrWhiteSpace(Email), e => e.Email.Contains(Email))
                    .AndIF(MinScore is not 0, e => e.Score >= MinScore)
                    .AndIF(MaxScore is not 0, e => e.Score <= MaxScore)
                    .ToExpression();


}
