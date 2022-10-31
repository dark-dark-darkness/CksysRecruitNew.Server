using System.IO;
using System.Security.Claims;

using CksysRecruitNew.Server.Entities;
using CksysRecruitNew.Server.Models;
using CksysRecruitNew.Server.Models.ApplyInfos;
using CksysRecruitNew.Server.Repositories;
using CksysRecruitNew.Server.Services;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using MiniExcelLibs;

using SqlSugar;

namespace CksysRecruitNew.Server.Controllers;

/// <summary>
/// 申请信息控制器
/// </summary>
[ApiController]
[Route("api/apply-info")]
public class ApplyInfoController : ControllerBase {

  #region 私有字段+构造函数
  private readonly IApplyInfoRepository _repository;
  private readonly EmailService _notify;
  private readonly ILogger<ApplyInfoController> _logger;
  private readonly ISqlSugarClient _db;

  /// <summary>
  /// 
  /// </summary>
  /// <param name="repository"></param>
  /// <param name="notify"></param>
  /// <param name="logger"></param>
  public ApplyInfoController(IApplyInfoRepository repository, EmailService notify, ILogger<ApplyInfoController> logger, ISqlSugarClient db) {
    _repository = repository;
    _notify = notify;
    _logger = logger;
    _db = db;
  }
  #endregion

  #region 公开接口
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
    var result = await _repository.SaveAsync(entity);

    try {
      await _notify.SeedAsync(dto.Email, dto.Name);
    } catch (Exception ex) {
      _logger.LogError(ex, "发送到 {string} 的邮箱 {string} 错误", dto.Name, dto.Email);
      throw;
    }

    return result ? Result.Ok(result) : Result.BadRequest();
  }
  #endregion

  #region 管理员接口
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
  public async Task<Result<ApplyInfoPageDto>> GetPageAsync([FromQuery] SearchApplyInfoDto dto, OrderBy scoreOrderBy = OrderBy.None, int pageNumber = 1, int pageSize = 20) {
    var totalAsync = new RefAsync<int>();
    var page = await _repository.GetManyAsync(dto.ToExpression(), pageNumber, pageSize, totalAsync, scoreOrderBy);
    if (totalAsync.Value is 0) return Result<ApplyInfoPageDto>.NotFound("没有找到符合条件的数据");
    return Result<ApplyInfoPageDto>.Ok(new() { Page = page, PageNumber = pageNumber, PageSize = pageSize, Total = totalAsync.Value });
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
  /// 管理员导出信息
  /// </summary>
  /// <param name="id"></param>
  /// <returns></returns>
  [Authorize(Roles = "admin")]
  [HttpGet("export")]
  public async Task<ActionResult> ExportAsync() {
    var ms = new MemoryStream();
    await ms.SaveAsAsync(await _repository.GetManyAsync());
    ms.Seek(0, SeekOrigin.Begin);
    return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"data-{DateTime.Now}");
  }
  #endregion

  #region 申请人接口
  /// <summary>
  /// 申请人获取自己的信息
  /// </summary>
  /// <returns></returns>
  [Authorize(Roles = "user")]
  [HttpGet]
  public async Task<Result<ApplyInfo>> GetByPhoneAsync() {
    var phone = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "";
    var result = await _repository.GetAsync(info => info.Phone == phone);
    return result is not null ? Result<ApplyInfo>.Ok(result) : Result<ApplyInfo>.NotFound($"手机号为{phone}的申请没有找到");
  }

  /// <summary>
  /// 申请人更新自己的信息
  /// </summary>
  /// <param name="dto"></param>
  /// <returns></returns>
  [Authorize(Roles = "user")]
  [HttpPut]
  public async Task<Result> UpdateByPhoneAsync(UpdateApplyInfoDto dto) {
    var phone = HttpContext.User.Claims?.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "";
    if (await _repository.ExistsAsync(phone)) {
      var e = dto.ToEntity(phone);
      await _repository.UpdateAsync(e);
      return Result.Ok(e);
    }
    return Result.NotFound($"手机号为{phone}的申请不存在");
  }
  #endregion

  #region 管理员报表接口

  [HttpGet("report/score/{span}")]
  public async Task<Result> GetByScoreBucket(int span = 10) {
    var scoreMin = Enumerable.Range(0, 100 / span).Select(v => v * span).ToList();
    var t = _db.Reportable(scoreMin).ToQueryable<int>();

    var result =
      await _db.Queryable<ApplyInfo>()
               .InnerJoin(t, (x1, x2) => x1.Score > x2.ColumnName && x1.Score <= x2.ColumnName + span)
               .GroupBy((x1, x2) => x2.ColumnName)
               .OrderBy((x1, x2) => x2.ColumnName)
               .Select((x1, x2) => new {
                 ScoreSpan = "[" + x2.ColumnName.ToString() + "," + (x2.ColumnName + span).ToString() + ")",
                 Count = SqlFunc.AggregateCount(1)
               })
               .ToListAsync();

    return Result.Ok(result);
  }

  [HttpGet("report/class")]
  public async Task<Result> GetByClassNameBucket() {

    var result =
          await _db.Queryable<ApplyInfo>()
                    .GroupBy(info => info.ClassName)
                    .Select(info => new {
                      ClassName = info.ClassName,
                      Count = SqlFunc.AggregateCount(1)
                    })
                    .ToListAsync();

    return Result.Ok(result);
  }


  #endregion

}
