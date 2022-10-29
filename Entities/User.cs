using SqlSugar;

namespace CksysRecruitNew.Server.Entities;

[SugarTable("t_User")]
public class User {
    [SugarColumn(IsPrimaryKey = true)]
    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

}
