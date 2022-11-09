namespace CksysRecruitNew.Server.Models;

public class Result<T> {
  public int Code { get; set; }

  public string Message { get; set; } = string.Empty;

  public T? Data { get; set; }


  public static Result<T> Ok(T? data = default)
    => new() {
      Code = 200,
      Message = "操作成功",
      Data = data,
    };

  public static Result<T> NotFound(string message = "未找到！", T? data = default)
    => new() {
      Code = 404,
      Message = message,
      Data = data,
    };

  public static Result<T> Error(string message = "未处理错误！", T? data = default)
    => new() {
      Code = 500,
      Message = message,
      Data = data,
    };

  public static Result<T> Created(string message = "成功创建！", T? data = default)
    => new() {
      Code = 201,
      Message = message,
      Data = data,
    };

  public static Result<T> BadRequest(string message = "请求错误", T? data = default)
    => new() {
      Code = 400,
      Message = message,
      Data = data,
    };


  public static implicit operator Result(Result<T> result)
    => new() {
      Code = result.Code,
      Message = result.Message,
      Data = result.Data,
    };
}

public sealed class Result : Result<object> {
}
