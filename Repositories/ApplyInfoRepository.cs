using System.Linq.Expressions;

using CksysRecruitNew.Server.Entities;

using SqlSugar;

namespace CksysRecruitNew.Server.Repositories;

public class ApplyInfoRepository : IApplyInfoRepository {
  protected readonly ISqlSugarClient db;

  public ApplyInfoRepository(ISqlSugarClient db) {
    this.db = db;
  }

  public Task<bool> DeleteAsync(string phone)
    => db.Deleteable<ApplyInfo>().Where(info => info.Phone == phone).ExecuteCommandHasChangeAsync();

  public Task<ApplyInfo?> GetAsync(string phone)
    => db.Queryable<ApplyInfo>().FirstAsync(info => info.Phone == phone)!;

  public Task<List<ApplyInfo>> GetManyAsync(ApplyInfo? info = null, int pageNumber = 1, int pageSize = int.MaxValue, RefAsync<int>? total = null)
    => db.Queryable<ApplyInfo>()
         .WhereIF(!string.IsNullOrWhiteSpace(info?.Id), e => e.Id.Contains(info!.Id))
         .WhereIF(!string.IsNullOrWhiteSpace(info?.Name), e => e.Name.Contains(info!.Name))
         .WhereIF(!string.IsNullOrWhiteSpace(info?.ClassName), e => e.ClassName.Contains(info!.ClassName))
         .WhereIF(!string.IsNullOrWhiteSpace(info?.Phone), e => e.Phone.Contains(info!.Phone))
         .WhereIF(!string.IsNullOrWhiteSpace(info?.Email), e => e.Email.Contains(info!.Email))
         .ToPageListAsync(pageNumber, pageSize, total);

  public Task<bool> SaveAsync(ApplyInfo info)
    => db.Insertable(info).ExecuteCommandIdentityIntoEntityAsync();

  public Task<bool> UpdateAsync(ApplyInfo info)
    => db.Updateable(info).ExecuteCommandHasChangeAsync();

  public async Task<bool> ExistsAsync(string phone)
    => await db.Queryable<ApplyInfo>().Where(info => info.Phone == phone).CountAsync() != 0;

  public Task<int> CountAsync(ApplyInfo? info = null)
    => db.Queryable<ApplyInfo>()
         .WhereIF(!string.IsNullOrWhiteSpace(info?.Id), e => e.Id.Contains(info!.Id))
         .WhereIF(!string.IsNullOrWhiteSpace(info?.Name), e => e.Name.Contains(info!.Name))
         .WhereIF(!string.IsNullOrWhiteSpace(info?.ClassName), e => e.ClassName.Contains(info!.ClassName))
         .WhereIF(!string.IsNullOrWhiteSpace(info?.Phone), e => e.Phone.Contains(info!.Phone))
         .WhereIF(!string.IsNullOrWhiteSpace(info?.Email), e => e.Email.Contains(info!.Email))
         .CountAsync();

  public async Task<bool> ExistsAsync(Expression<Func<ApplyInfo, bool>> whereExpr)
    => await db.Queryable<ApplyInfo>().Where(whereExpr).CountAsync() != 0;

  public Task<ApplyInfo?> GetAsync(Expression<Func<ApplyInfo, bool>> whereExpr)
    => db.Queryable<ApplyInfo>().FirstAsync(whereExpr)!;

  public Task<bool> DeleteAsync(Expression<Func<ApplyInfo, bool>> whereExpr)
    => db.Deleteable(whereExpr).ExecuteCommandHasChangeAsync();

  public Task<int> CountAsync(Expression<Func<ApplyInfo, bool>>? whereExpr = null)
    => db.Queryable<ApplyInfo>().WhereIF(whereExpr is not null, whereExpr).CountAsync();
}
