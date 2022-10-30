using System.Linq.Expressions;

using CksysRecruitNew.Server.Entities;

using SqlSugar;

namespace CksysRecruitNew.Server.Repositories;

public interface IApplyInfoRepository {

  public Task<bool> ExistsAsync(string phone);

  public Task<bool> ExistsAsync(Expression<Func<ApplyInfo, bool>> whereExpr);

  public Task<ApplyInfo?> GetAsync(string phone);

  public Task<ApplyInfo?> GetAsync(Expression<Func<ApplyInfo, bool>> whereExpr);

  public Task<List<ApplyInfo>> GetManyAsync(ApplyInfo? info = null, int pageNumber = 1, int pageSize = int.MaxValue, RefAsync<int>? total = null);

  public Task<List<ApplyInfo>> GetManyAsync(Expression<Func<ApplyInfo, bool>> whereExpr, int pageNumber = 1, int pageSize = int.MaxValue, RefAsync<int>? total = null);

  public Task<bool> DeleteAsync(string phone);

  public Task<bool> DeleteAsync(Expression<Func<ApplyInfo, bool>> whereExpr);

  public Task<bool> UpdateAsync(ApplyInfo info);

  public Task<bool> SaveAsync(ApplyInfo info);

  public Task<int> CountAsync(ApplyInfo? info = null);

  public Task<int> CountAsync(Expression<Func<ApplyInfo, bool>>? whereExpr = null);

}
