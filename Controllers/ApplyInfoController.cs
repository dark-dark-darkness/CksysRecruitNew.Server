using System.ComponentModel.DataAnnotations;

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

  private readonly INotifyService _notify;

  private readonly ILogger<ApplyInfoController> _logger;

  public ApplyInfoController(IApplyInfoRepository repository, INotifyService notify, ILogger<ApplyInfoController> logger) {
    _repository = repository;
    _notify = notify;
    _logger = logger;
  }

  [Authorize]
  [HttpGet("{id}")]
  public async Task<Result> GetAsync(string id) {
    var result = await _repository.GetAsync(id);
    return result is not null ? Result.Ok() : Result.NotFound($"学号为{id}的申请没有找到");
  }

  [Authorize]
  [HttpGet]
  public async Task<Result> GetPageAsync([FromQuery] ApplyInfo info, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20) {
    var total = 0;
    var totalAsync = new RefAsync<int>(total);
    var list = await _repository.GetManyAsync(info, pageNumber, pageSize, totalAsync);
    if (totalAsync.Value is 0) Result.NotFound("没有符合条件的数据");
    var result = new SearchApplyInfoPageDto { Page = list, PageNumber = pageNumber, PageSize = pageSize, Total = totalAsync.Value };
    return Result.Ok(result);
  }

  [HttpPost]
  public async Task<Result> SaveAsync(CreateUpdateApplyInfoDto dto) {
    var exists = await _repository.ExistsAsync(dto.Id);
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

  [Authorize]
  [HttpPut]
  public async Task<Result> UpdateAsync(CreateUpdateApplyInfoDto dto) {
    var exists = await _repository.ExistsAsync(dto.Id);
    if (!exists) return Result.BadRequest($"学号为{dto.Id}的申请不存在");
    var e = dto.ToEntity();
    e.IdAddress = HttpContext.Connection?.RemoteIpAddress?.ToString();
    var result = await _repository.UpdateAsync(e);

    return result ? Result.Ok(result) : Result.BadRequest();
  }

  [Authorize]
  [HttpDelete("{id}")]
  public async Task<Result> DeleteAsync(string id) {
    var exists = await _repository.ExistsAsync(id);
    if (!exists) return Result.NotFound($"学号为{id}的申请没有找到");
    await _repository.DeleteAsync(id);
    return Result.Ok();
  }

  // 测试用
  //[HttpGet("export")]
  //public async Task<ActionResult> Export()
  //{
  //  var result = await _repository.GetManyAsync();
  //  var ms = new MemoryStream();
  //  await ms.SaveAsAsync(result);
  //  ms.Position = 0;
  //  return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"data-{DateTime.Now}.xlsx");
  //}

  //// 测试用
  //[HttpGet("seed/{email}")]
  //public async Task<ActionResult> TestEmail([EmailAddress] string email)
  //{
  //  await _email.SeedAsync(email, "胡学费");
  //  return Ok();
  //}
}
