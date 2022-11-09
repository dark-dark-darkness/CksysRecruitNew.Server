using System.Linq.Expressions;

using CksysRecruitNew.Server.Entities;
using CksysRecruitNew.Server.Models;

using SqlSugar;

namespace CksysRecruitNew.Server.Repositories;

public sealed class ApplyInfoRepository : IApplyInfoRepository {
  private readonly ISqlSugarClient _db;


  public ApplyInfoRepository(ISqlSugarClient db) {
    this._db = db;
  }

  public Task<bool> DeleteAsync(string phone)
    => _db.Deleteable<ApplyInfo>().Where(info => info.Phone == phone).ExecuteCommandHasChangeAsync();

  public Task<ApplyInfo?> GetAsync(string phone)
    => _db.Queryable<ApplyInfo>().FirstAsync(info => info.Phone == phone)!;

  public Task<List<ApplyInfo>> GetManyAsync(ApplyInfo? info = null, int pageNumber = 1, int pageSize = int.MaxValue, RefAsync<int>? total = null, OrderBy orderByScore = OrderBy.None)
    => _db.Queryable<ApplyInfo>()
         .WhereIF(!string.IsNullOrWhiteSpace(info?.Id), e => e.Id.Contains(info!.Id))
         .WhereIF(!string.IsNullOrWhiteSpace(info?.Name), e => e.Name.Contains(info!.Name))
         .WhereIF(!string.IsNullOrWhiteSpace(info?.ClassName), e => e.ClassName.Contains(info!.ClassName))
         .WhereIF(!string.IsNullOrWhiteSpace(info?.Phone), e => e.Phone.Contains(info!.Phone))
         .WhereIF(!string.IsNullOrWhiteSpace(info?.Email), e => e.Email.Contains(info!.Email))
         .OrderByIF(orderByScore is not OrderBy.None, e => e.Score, (OrderByType)(orderByScore - 1))
         .ToPageListAsync(pageNumber, pageSize, total ?? new RefAsync<int>());

  public Task<List<ApplyInfo>> GetManyAsync(Expression<Func<ApplyInfo, bool>> whereExpr, int pageNumber = 1, int pageSize = int.MaxValue, RefAsync<int>? total = null, OrderBy orderByScore = OrderBy.None)
  => _db.Queryable<ApplyInfo>()
       .Where(whereExpr)
       .OrderByIF(orderByScore is not OrderBy.None, e => e.Score, (OrderByType)(orderByScore - 1))
       .ToPageListAsync(pageNumber, pageSize, total);

  public Task<bool> SaveAsync(ApplyInfo info)
    => _db.Insertable(info).ExecuteCommandIdentityIntoEntityAsync();

  public Task<bool> UpdateAsync(ApplyInfo info)
    => _db.Updateable(info).ExecuteCommandHasChangeAsync();

  public async Task<bool> ExistsAsync(string phone)
    => await _db.Queryable<ApplyInfo>().Where(info => info.Phone == phone).CountAsync() != 0;

  public Task<int> CountAsync(ApplyInfo? info = null)
    => _db.Queryable<ApplyInfo>()
         .WhereIF(!string.IsNullOrWhiteSpace(info?.Id), e => e.Id.Contains(info!.Id))
         .WhereIF(!string.IsNullOrWhiteSpace(info?.Name), e => e.Name.Contains(info!.Name))
         .WhereIF(!string.IsNullOrWhiteSpace(info?.ClassName), e => e.ClassName.Contains(info!.ClassName))
         .WhereIF(!string.IsNullOrWhiteSpace(info?.Phone), e => e.Phone.Contains(info!.Phone))
         .WhereIF(!string.IsNullOrWhiteSpace(info?.Email), e => e.Email.Contains(info!.Email))
         .CountAsync();

  public async Task<bool> ExistsAsync(Expression<Func<ApplyInfo, bool>> whereExpr)
    => await _db.Queryable<ApplyInfo>().Where(whereExpr).CountAsync() != 0;

  public Task<ApplyInfo?> GetAsync(Expression<Func<ApplyInfo, bool>> whereExpr)
    => _db.Queryable<ApplyInfo>().FirstAsync(whereExpr)!;

  public Task<bool> DeleteAsync(Expression<Func<ApplyInfo, bool>> whereExpr)
    => _db.Deleteable(whereExpr).ExecuteCommandHasChangeAsync();

  public Task<int> CountAsync(Expression<Func<ApplyInfo, bool>>? whereExpr = null)
    => _db.Queryable<ApplyInfo>().WhereIF(whereExpr is not null, whereExpr).CountAsync();
}
