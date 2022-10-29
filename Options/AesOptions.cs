namespace CksysRecruitNew.Server.Options;

public class AesOptions {

    public const string SectionKey = "AesOptions";
    public string EncryptKey { get; set; } = string.Empty;
}
