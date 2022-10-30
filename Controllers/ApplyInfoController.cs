using System.Security.Claims;

using CksysRecruitNew.Server.Entities;
using CksysRecruitNew.Server.Models;
using CksysRecruitNew.Server.Models.ApplyInfos;
using CksysRecruitNew.Server.Repositories;
using CksysRecruitNew.Server.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using SqlSugar;

namespace CksysRecruitNew.Server.Controllers;

/// <summary>
/// 申请信息控制器
/// </summary>
[ApiController]
[Route("api/apply-info")]
public class ApplyInfoController : ControllerBase {
  private readonly IApplyInfoRepository _repository;

  private readonly EmailService _notify;

  private readonly ILogger<ApplyInfoController> _logger;

  /// <summary>
  /// 
  /// </summary>
  /// <param name="repository"></param>
  /// <param name="notify"></param>
  /// <param name="logger"></param>
  public ApplyInfoController(IApplyInfoRepository repository, EmailService notify, ILogger<ApplyInfoController> logger) {
    _repository = repository;
    _notify = notify;
    _logger = logger;
  }

  /// <summary>
  /// 判断学号为id的申请信息是否存在
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  [HttpGet("exist/{id}")]
  public async Task<Result> ExistAsync(string id) {
    var result = await _repository.GetAsync(id);
    return result is not null ? Result.Ok(true) : Result.NotFound($"学号为{id}的申请没有找到", false);
  }

  /// <summary>
  /// 保存申请信息
  /// </summary>
  /// <param name="dto"></param>
  /// <returns></returns>
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

  /// <summary>
  /// 获取申请信息详细信息
  /// </summary>
  /// <param name="id">学号</param>
  /// <returns></returns>
  [Authorize(Roles = "admin")]
  [HttpGet("{id}")]
  public async Task<Result<ApplyInfo>> GetAsync(string id) {
    var result = await _repository.GetAsync(id);
    return result is not null ? Result<ApplyInfo>.Ok(result) : Result<ApplyInfo>.NotFound($"学号为{id}的申请没有找到");
  }

  /// <summary>
  /// 获取模糊查询条件分页
  /// </summary>
  /// <param name="info"></param>
  /// <param name="pageSize"></param>
  /// <param name="pageNumber"></param>
  /// <returns></returns>
  [Authorize(Roles = "admin")]
  [HttpGet("list")]
  public async Task<Result<ApplyInfoPageDto>> GetPageAsync([FromQuery] SearchApplyInfoDto dto) {
    var totalAsync = new RefAsync<int>();
    var page = await _repository.GetManyAsync(dto.ToExpression(), dto.PageNumber, dto.PageSize, totalAsync);
    if (totalAsync.Value is 0) return Result<ApplyInfoPageDto>.NotFound("没有找到符合条件的数据");
    return Result<ApplyInfoPageDto>.Ok(new() { Page = page, PageNumber = dto.PageNumber, PageSize = dto.PageSize, Total = totalAsync.Value });
  }

  /// <summary>
  /// 管理员更新信息
  /// </summary>
  /// <param name="id"></param>
  /// <param name="dto"></param>
  /// <returns></returns>
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

  /// <summary>
  /// 管理员删除信息
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  [Authorize(Roles = "admin")]
  [HttpDelete("{id}")]
  public async Task<Result> DeleteAsync(string id) {
    var exists = await _repository.ExistsAsync(info => info.Id == id);
    if (!exists) return Result.NotFound($"学号为{id}的申请没有找到");
    await _repository.DeleteAsync(id);
    return Result.Ok();
  }

  /// <summary>
  /// 申请人获取自己的信息
  /// </summary>
  /// <returns></returns>
  [Authorize]
  [HttpGet]
  public async Task<Result<ApplyInfo>> GetByAuthAsync() {
    var phone = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "";
    var result = await _repository.GetAsync(info => info.Phone == phone);
    return result is not null ? Result<ApplyInfo>.Ok(result) : Result<ApplyInfo>.NotFound($"手机号为{phone}的申请没有找到");
  }

  /// <summary>
  /// 申请人更新自己的信息
  /// </summary>
  /// <param name="dto"></param>
  /// <returns></returns>
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
