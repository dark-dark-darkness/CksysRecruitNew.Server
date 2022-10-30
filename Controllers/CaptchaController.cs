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

  private readonly SmsService _smsNotifyService;

  private readonly ISqlSugarClient _db;

  private readonly JwtHelper _jwtHelper;

  private readonly IApplyInfoRepository _applyInfoRepository;

  public CaptchaController(SmsService smsNotifyService, ISqlSugarClient db, JwtHelper jwtHelper, IApplyInfoRepository applyInfoRepository) {
    _smsNotifyService = smsNotifyService;
    _db = db;
    _jwtHelper = jwtHelper;
    _applyInfoRepository = applyInfoRepository;
  }

  [HttpPost("{phone}")]
  public async Task<Result> SeedAsync(string phone) {
    var lastCode = await _db.Queryable<PhoneCaptcha>()
                            .Where(e => e.Phone == phone)
                            .OrderBy(pc => pc.ExpiresTime, OrderByType.Desc)
                            .FirstAsync();

    if (lastCode is not null && lastCode.ExpiresTime > DateTime.Now) return Result.BadRequest("发送的太过频繁！");

    var code = new Random().Next(100000, 999999).ToString();
    await _smsNotifyService.SeedAsync(SmsSeedParameter.Captcha(phone, code));
    await _db.Insertable(new PhoneCaptcha { Phone = phone, Captcha = code }).ExecuteCommandAsync();
    return Result.Ok();
  }

  [HttpGet("{phone}/{captcha}")]
  public async Task<Result> GetUpdateTokenAsync(string phone, string captcha) {
    var result = await _db.Queryable<PhoneCaptcha>()
                          .Where(e => e.Phone == phone)
                          .OrderBy(pc => pc.ExpiresTime, OrderByType.Desc)
                          .FirstAsync();

    if (result is null) return Result.NotFound("验证码不存在！");

    if (result.ExpiresTime < DateTime.Now) return Result.NotFound("验证码已过期！");

    if (result.Captcha != captcha) return Result.BadRequest("验证码错误！");

    var info = await _applyInfoRepository.GetAsync(info => info.Phone == phone);

    if (info is null) return Result.BadRequest("该手机号没有申请信息！");

    var token = _jwtHelper.CreateToken(info!.Id, "updater");

    return Result.Ok(token);
  }

}
