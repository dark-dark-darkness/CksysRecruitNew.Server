using CksysRecruitNew.Server.Entities;

namespace CksysRecruitNew.Server.Models;

public class SearchApplyInfoPageDto {
  public int Total { get; set; }

  public int PageNumber { get; set; } = 1;

  public int PageSize { get; set; }

  public IList<ApplyInfo>? Page { get; set; }
}
