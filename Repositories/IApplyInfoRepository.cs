using System.Linq.Expressions;

using CksysRecruitNew.Server.Entities;

using SqlSugar;

namespace CksysRecruitNew.Server.Repositories;

public interface IApplyInfoRepository
{
  public Task<bool> ExistsAsync(string id);

  public Task<ApplyInfo?> GetAsync(string id);

  public Task<List<ApplyInfo>> GetManyAsync(ApplyInfo? info = null, int pageNumber = 1, int pageSize = int.MaxValue, RefAsync<int>? total = null);

  public Task<bool> DeleteAsync(string id);

  public Task<bool> UpdateAsync(ApplyInfo info);

  public Task<bool> SaveAsync(ApplyInfo info);

  public Task<int> CountAsync(ApplyInfo? info = null);
}
