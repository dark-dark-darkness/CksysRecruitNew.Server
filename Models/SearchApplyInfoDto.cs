using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

using CksysRecruitNew.Server.Entities;

namespace CksysRecruitNew.Server.Models;

public class SearchApplyInfoDto {

  [Display(Name = "学号")]
  public string Id { get; set; } = string.Empty;

  [Display(Name = "姓名")]
  public string Name { get; set; } = string.Empty;

  [Display(Name = "班级")]
  public string ClassName { get; set; } = string.Empty;

  [Display(Name = "手机号")]
  public string Phone { get; set; } = string.Empty;

  [Display(Name = "电子邮箱")]
  public string Email { get; set; } = string.Empty;

  [Display(Name = "简介")]
  public string Profile { get; set; } = string.Empty;

  [Display(Name = "分数")]
  public double Score { get; set; }

  public static ApplyInfo FromEntity(ApplyInfo entity)
    => new() {
      Id = entity.Id,
      Name = entity.Name,
      ClassName = entity.ClassName,
      Phone = entity.Phone,
      Email = entity.Email,
      Profile = entity.Profile,
      Score = entity.Score,
    };

}
