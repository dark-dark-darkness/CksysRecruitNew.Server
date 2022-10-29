namespace CksysRecruitNew.Server.Models;

public class Result
{
  public int Code { get; set; }

  public string Message { get; set; } = string.Empty;

  public object? Data { get; set; }


  public static Result Ok(object? data = null)
    => new()
    {
      Code = 200,
      Message = "操作成功",
      Data = data,
    };

  public static Result NotFound(string message = "未找到！")
    => new()
    {
      Code = 404,
      Message = message
    };

  public static Result Error(string message = "未处理错误！")
    => new()
    {
      Code = 500,
      Message = message
    };

  public static Result Created(string message = "成功创建！")
    => new()
    {
      Code = 201,
      Message = message
    };

  public static Result BadRequest(string message = "请求错误")
    => new()
    {
      Code = 400,
      Message = message
    };
}
