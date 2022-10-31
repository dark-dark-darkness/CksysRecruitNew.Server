using SqlSugar;

namespace CksysRecruitNew.Server.Entities;

[SugarTable(EntitiesConsts.TableName.User)]
public class User {
  [SugarColumn(IsPrimaryKey = true)]
  public string Username { get; set; } = string.Empty;

  public string Password { get; set; } = string.Empty;

}
