using System.ComponentModel;
using System.Text.Json.Serialization;

using SqlSugar;

namespace CksysRecruitNew.Server.Entities;

[SugarTable(TableNameConsts.ApplyInfo)]
public class ApplyInfo {

  [SugarColumn(IsPrimaryKey = true)]
  [DisplayName("手机号")]
  public string Phone { get; set; } = string.Empty;

  [DisplayName("学号")]
  public string Id { get; set; } = string.Empty;

  [DisplayName("姓名")]
  public string Name { get; set; } = string.Empty;

  [DisplayName("班级")]
  public string ClassName { get; set; } = string.Empty;

  [DisplayName("电子邮箱")]
  public string Email { get; set; } = string.Empty;

  [DisplayName("简介")]
  public string Profile { get; set; } = string.Empty;

  [DisplayName("分数")]
  [SugarColumn(IsNullable = true)]
  public double Score { get; set; }
}
