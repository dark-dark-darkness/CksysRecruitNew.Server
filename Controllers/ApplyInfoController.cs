using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

using CksysRecruitNew.Server.Entities;
using CksysRecruitNew.Server.Models;
using CksysRecruitNew.Server.Repositories;
using CksysRecruitNew.Server.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using MiniExcelLibs;

using SqlSugar;

namespace CksysRecruitNew.Server.Controllers;

[ApiController]
[Route("api/apply-info")]
public class ApplyInfoController : ControllerBase {
  private readonly IApplyInfoRepository _repository;

  private readonly EmailService _notify;

  private readonly ILogger<ApplyInfoController> _logger;

  public ApplyInfoController(IApplyInfoRepository repository, EmailService notify, ILogger<ApplyInfoController> logger) {
    _repository = repository;
    _notify = notify;
    _logger = logger;
  }


  [HttpGet("exist/{id}")]
  public async Task<Result> ExistAsync(string id) {
    var result = await _repository.GetAsync(id);
    return result is not null ? Result.Ok(true) : Result.NotFound($"学号为{id}的申请没有找到", false);
  }

  [HttpPost]
  public async Task<Result> SaveAsync(CreateApplyInfoDto dto) {
    var exists = await _repository.ExistsAsync(info => info.Id == dto.Id);
    if (exists) return Result.BadRequest($"学号为{dto.Id}的申请已经存在");
    var entity = dto.ToEntity();
    entity.IdAddress = HttpContext.Connection?.RemoteIpAddress?.ToString();
    var result = await _repository.SaveAsync(entity);
    try {
      await _notify.SeedAsync(dto.Email, dto.Name);
    } catch (Exception ex) {
      _logger.LogError(ex, "发送到 {string} 的邮箱 {string} 错误", dto.Name, dto.Email);
      throw;
    }
    return result ? Result.Ok(result) : Result.BadRequest();
  }


  [Authorize(Roles = "admin")]
  [HttpGet("{id}")]
  public async Task<Result<ApplyInfo>> GetAsync(string id) {
    var result = await _repository.GetAsync(id);
    return result is not null ? Result<ApplyInfo>.Ok(result) : Result<ApplyInfo>.NotFound($"学号为{id}的申请没有找到");
  }


  [Authorize(Roles = "admin")]
  [HttpGet("list")]
  public async Task<Result<SearchApplyInfoPageDto>> GetPageAsync([FromQuery] ApplyInfo info, [FromQuery] int pageSize = 20, [FromQuery] int pageNumber = 1) {
    var total = 0;
    var totalAsync = new RefAsync<int>(total);
    var list = await _repository.GetManyAsync(info, pageNumber, pageSize, totalAsync);
    if (totalAsync.Value is 0) Result.NotFound("没有符合条件的数据");
    var result = new SearchApplyInfoPageDto { Page = list, PageNumber = pageNumber, PageSize = pageSize, Total = totalAsync.Value };
    return Result<SearchApplyInfoPageDto>.Ok(result);
  }


  [Authorize(Roles = "admin")]
  [HttpPut("{id}")]
  public async Task<Result> UpdateAsync(string id, [FromBody] UpdateApplyInfoDto dto) {
    var exists = await _repository.ExistsAsync(info => info.Id == id);
    if (!exists) return Result.NotFound($"学号为{id}的申请不存在");
    var e = dto.ToEntity();
    e.IdAddress = HttpContext.Connection?.RemoteIpAddress?.ToString();
    var result = await _repository.UpdateAsync(e);

    return result ? Result.Ok(result) : Result.BadRequest();
  }


  [Authorize(Roles = "admin")]
  [HttpDelete("{id}")]
  public async Task<Result> DeleteAsync(string id) {
    var exists = await _repository.ExistsAsync(info => info.Id == id);
    if (!exists) return Result.NotFound($"学号为{id}的申请没有找到");
    await _repository.DeleteAsync(id);
    return Result.Ok();
  }


  [Authorize]
  [HttpGet]
  public async Task<Result<ApplyInfo>> GetByAuthAsync() {
    var phone = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "";
    var result = await _repository.GetAsync(info => info.Phone == phone);
    return result is not null ? Result<ApplyInfo>.Ok(result) : Result<ApplyInfo>.NotFound($"手机号为{phone}的申请没有找到");
  }


  [Authorize]
  [HttpPut]
  public async Task<Result> UpdateByAuthAsync(UpdateApplyInfoDto dto) {
    var phone = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "";
    if (await _repository.ExistsAsync(phone)) {
      var e = dto.ToEntity(phone);
      await _repository.UpdateAsync(e);
      return Result.Ok(e);
    }
    return Result.NotFound($"手机号为{phone}的申请不存在");
  }
}
