using System.ComponentModel.DataAnnotations;

using CksysRecruitNew.Server.Entities;

using SqlSugar;

namespace CksysRecruitNew.Server.Models;

public class CreateApplyInfoDto {

  [Display(Name = "学号")]
  [Required(ErrorMessage = "请填写{0}")]
  [RegularExpression(@"\d{10}", ErrorMessage = "请输入正确的{0}！")]
  public string Id { get; set; } = string.Empty;

  [Display(Name = "姓名")]
  [Required(ErrorMessage = "请填写{0}")]
  [MinLength(1, ErrorMessage = "请输入正确的{0}！")]
  public string Name { get; set; } = string.Empty;

  [Display(Name = "班级")]
  [Required(ErrorMessage = "请填写{0}")]
  [MinLength(1, ErrorMessage = "请输入正确的{0}！")]
  public string ClassName { get; set; } = string.Empty;

  [Display(Name = "手机号")]
  [Required(ErrorMessage = "请填写{0}")]
  [Phone(ErrorMessage = "请输入正确的{0}！")]
  public string Phone { get; set; } = string.Empty;

  [Display(Name = "电子邮箱")]
  [Required(ErrorMessage = "请填写{0}")]
  [EmailAddress(ErrorMessage = "请输入正确的邮箱地址！")]
  public string Email { get; set; } = string.Empty;

  [Display(Name = "简介")]
  [Required(ErrorMessage = "请填写{0}")]
  [MinLength(25, ErrorMessage = "请再多写点！")]
  public string Profile { get; set; } = string.Empty;


  public ApplyInfo ToEntity()
    => new() {
      Id = Id,
      Name = Name,
      ClassName = ClassName,
      Phone = Phone,
      Email = Email,
      Profile = Profile,
    };

}
