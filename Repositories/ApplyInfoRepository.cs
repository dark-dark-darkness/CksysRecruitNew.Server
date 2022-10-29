using CksysRecruitNew.Server.Entities;
using SqlSugar;

namespace CksysRecruitNew.Server.Repositories;

public class ApplyInfoRepository : IApplyInfoRepository {
  protected readonly ISqlSugarClient db;

  public ApplyInfoRepository(ISqlSugarClient db) {
    this.db = db;
  }

  public Task<bool> DeleteAsync(string id)
    => db.Deleteable<ApplyInfo>().Where(info => info.Id == id).ExecuteCommandHasChangeAsync();

  public Task<ApplyInfo?> GetAsync(string id)
    => db.Queryable<ApplyInfo>().FirstAsync(info => info.Id == id)!;

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

  public async Task<bool> ExistsAsync(string id)
    => await db.Queryable<ApplyInfo>().Where(info => info.Id == id).CountAsync() != 0;

  public Task<int> CountAsync(ApplyInfo? info = null)
    => db.Queryable<ApplyInfo>()
         .WhereIF(!string.IsNullOrWhiteSpace(info?.Id), e => e.Id.Contains(info!.Id))
         .WhereIF(!string.IsNullOrWhiteSpace(info?.Name), e => e.Name.Contains(info!.Name))
         .WhereIF(!string.IsNullOrWhiteSpace(info?.ClassName), e => e.ClassName.Contains(info!.ClassName))
         .WhereIF(!string.IsNullOrWhiteSpace(info?.Phone), e => e.Phone.Contains(info!.Phone))
         .WhereIF(!string.IsNullOrWhiteSpace(info?.Email), e => e.Email.Contains(info!.Email))
         .CountAsync();
}
