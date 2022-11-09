using CksysRecruitNew.Server.Entities;
using CksysRecruitNew.Server.Helper;
using CksysRecruitNew.Server.Models;
using CksysRecruitNew.Server.Repositories;
using CksysRecruitNew.Server.Services;

using FreeRedis;

using Microsoft.AspNetCore.Mvc;

using SqlSugar;

namespace CksysRecruitNew.Server.Controllers;

[Route("api/captcha")]
public sealed class CaptchaController : ControllerBase {

  private readonly SmsService _smsService;

  private readonly JwtHelper _jwtHelper;

  private readonly RedisClient _redisClient;

  public CaptchaController(SmsService smsService, JwtHelper jwtHelper, RedisClient redisClient) {
    _smsService = smsService;
    _jwtHelper = jwtHelper;
    _redisClient = redisClient;
  }


  /// <summary>
  /// 向phone发送手机验证码
  /// </summary>
  /// <param name="phone"></param>
  /// <returns></returns>
  [HttpPost("{phone}")]
  public async Task<Result> SeedAsync(string phone) {

    var ttl = await _redisClient.TtlAsync(phone);

    if (ttl >= 4 * 60) return Result.BadRequest("获取太过频繁");

    var code = new Random().Next(10_0000, 99_9999).ToString();

    await _smsService.SeedAsync(SmsSeedParameter.Captcha(phone, code));

    await _redisClient.SetAsync(phone, code, 5 * 60);

    return Result.Ok();

  }

  /// <summary>
  /// 获取用户token
  /// </summary>
  /// <param name="phone"></param>
  /// <param name="captcha"></param>
  /// <returns></returns>
  [HttpGet("{phone}/{captcha}")]
  public async Task<TokenResult> GetApplicantTokenAsync(string phone, string captcha) {

    var code = await _redisClient.GetAsync(phone);

    if (code is null) return new TokenResult { Code = 400, Message = "验证码过期或不存在！" };

    if (code != captcha) return new TokenResult { Code = 400, Message = "验证码错误！" };

    var token = _jwtHelper.CreateToken(phone, "applicant");

    await _redisClient.DelAsync(phone);

    return new TokenResult { Code = 200, Message = "获取成功！", Token = token };

  }

}
