using CksysRecruitNew.Server.Entities;
using CksysRecruitNew.Server.Helper;
using CksysRecruitNew.Server.Models;
using CksysRecruitNew.Server.Repositories;
using CksysRecruitNew.Server.Services;

using Microsoft.AspNetCore.Mvc;

using SqlSugar;

namespace CksysRecruitNew.Server.Controllers;

public class VerificationCodeController {

  private readonly SmsService _smsNotifyService;
  private readonly ISqlSugarClient _db;
  private readonly JwtHelper _jwtHelper;
  private readonly IApplyInfoRepository _applyInfoRepository;

  public VerificationCodeController(SmsService smsNotifyService, ISqlSugarClient db, JwtHelper jwtHelper, IApplyInfoRepository applyInfoRepository) {
    _smsNotifyService = smsNotifyService;
    _db = db;
    _jwtHelper = jwtHelper;
    _applyInfoRepository = applyInfoRepository;
  }


  [HttpPost("{phone}")]
  public async Task<Result> SeedAsync(string phone) {
    var code = new Random().Next(100000, 999999).ToString();
    await _smsNotifyService.SeedAsync(SmsSeedParameter.VerificationCode(phone, code));
    _db.Insertable(new PhoneVerificationCode { Phone = phone, VerificationCode = code });
    return Result.Ok();
  }

  [HttpGet("{phone}/{verification}")]
  public async Task<Result> VerificationAsync(string phone, string verification) {
    var result = await _db.Queryable<PhoneVerificationCode>()
      .Where(e => e.ExpiresTime > DateTime.Now)
      .FirstAsync(e => e.Phone == phone && e.VerificationCode == verification);
    if (result is null) {
      return Result.BadRequest("验证码错误");
    }
    var info = await _applyInfoRepository.GetAsync(info => info.Phone == phone);
    var token = _jwtHelper.CreateToken(info!.Id, "updater");
    return Result.Ok(token);
  }

}
