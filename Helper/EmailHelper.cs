namespace CksysRecruitNew.Server.Helper;

public class EmailHelper {

  private static readonly string _tamplate = File.ReadAllText("EmailTemplate.html");

  public static string GetEmail(string name) => _tamplate.Replace("{{name}}", name);


}
