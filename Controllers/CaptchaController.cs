using CksysRecruitNew.Server.Entities;
using CksysRecruitNew.Server.Helper;
using CksysRecruitNew.Server.Models;
using CksysRecruitNew.Server.Repositories;
using CksysRecruitNew.Server.Services;

using Microsoft.AspNetCore.Mvc;

using SqlSugar;

namespace CksysRecruitNew.Server.Controllers;

[Route("api/captcha")]
public class CaptchaController {

  private readonly SmsService _smsService;

  private readonly ISqlSugarClient _db;

  private readonly JwtHelper _jwtHelper;

  private readonly IApplyInfoRepository _applyInfoRepository;

  public CaptchaController(SmsService smsService, ISqlSugarClient db, JwtHelper jwtHelper, IApplyInfoRepository applyInfoRepository) {
    _smsService = smsService;
    _db = db;
    _jwtHelper = jwtHelper;
    _applyInfoRepository = applyInfoRepository;
  }

  /// <summary>
  /// 向phone发送手机验证码
  /// </summary>
  /// <param name="phone"></param>
  /// <returns></returns>
  [HttpPost("{phone}")]
  public async Task<Result> SeedAsync(string phone) {
    var lastCode = await _db.Queryable<PhoneCaptcha>()
                            .Where(e => e.Phone == phone)
                            .FirstAsync();

    if (lastCode is not null && lastCode.ExpiresTime > DateTime.Now.AddMinutes(4)) return Result.BadRequest("发送的太过频繁！");

    var code = new Random().Next(100000, 999999).ToString();
    //await _smsService.SeedAsync(SmsSeedParameter.Captcha(phone, code));
    if (lastCode is not null) {
      lastCode.Captcha = code;
      _db.Updateable(lastCode).ExecuteCommandAsync();
    } else {
      _db.Insertable(new PhoneCaptcha { Phone = phone, Captcha = code }).ExecuteCommandAsync();
    }
    return Result.Ok();
  }

  /// <summary>
  /// 获取用户token
  /// </summary>
  /// <param name="phone"></param>
  /// <param name="captcha"></param>
  /// <returns></returns>
  [HttpGet("{phone}/{captcha}")]
  public async Task<Result> GetUpdateTokenAsync(string phone, string captcha) {
    var result = await _db.Queryable<PhoneCaptcha>()
                          .Where(e => e.Phone == phone)
                          .FirstAsync();

    if (result is null) return Result.NotFound("验证码不存在！");

    if (result.ExpiresTime < DateTime.Now) {
      await _db.Deleteable(result).ExecuteCommandAsync();
      return Result.NotFound("验证码已过期！");
    }

    if (result.Captcha != captcha) return Result.BadRequest("验证码错误！");

    var token = _jwtHelper.CreateToken(phone, "user");

    return Result.Ok(new { Code = 200, Message = "获取成功", Token = token });
  }

}
