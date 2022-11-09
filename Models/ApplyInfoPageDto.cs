using CksysRecruitNew.Server.Entities;

namespace CksysRecruitNew.Server.Models;

public sealed class ApplyInfoPageDto {
  public int Total { get; set; }

  public int PageNumber { get; set; } = 1;

  public int PageSize { get; set; }

  public IList<ApplyInfo>? Page { get; set; }
}
