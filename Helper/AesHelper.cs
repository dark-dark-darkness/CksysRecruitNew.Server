using System.Security.Cryptography;
using System.Text;

using CksysRecruitNew.Server.Options;

using Microsoft.Extensions.Options;

namespace CksysRecruitNew.Server.Helper;

/// <summary>
/// aes加密帮助类
/// </summary>
public class AesHelper {

  private readonly AesOptions _options;

  /// <summary>
  /// 
  /// </summary>
  /// <param name="options">帮助类配置项</param>
  public AesHelper(IOptions<AesOptions> options) {
    _options = options.Value;
  }

  /// <summary>
  /// 加密
  /// </summary>
  /// <param name="input"></param>
  /// <returns></returns>
  public string Encrypt(string input) {
    var encryptKey = Encoding.UTF8.GetBytes(_options.EncryptKey);

    using var aesAlg = Aes.Create();

    using var encryptor = aesAlg.CreateEncryptor(encryptKey, aesAlg.IV);

    using var msEncrypt = new MemoryStream();

    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))

    using (var swEncrypt = new StreamWriter(csEncrypt)) {
      swEncrypt.Write(input);
    }
    var iv = aesAlg.IV;

    var decryptedContent = msEncrypt.ToArray();

    var result = new byte[iv.Length + decryptedContent.Length];

    Buffer.BlockCopy(iv, 0, result, 0, iv.Length);

    Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

    return Convert.ToBase64String(result);


  }

  /// <summary>
  /// 解密
  /// </summary>
  /// <param name="input"></param>
  /// <returns></returns>
  public string Decrypt(string input) {
    var fullCipher = Convert.FromBase64String(input);

    var iv = new byte[16];
    var cipher = new byte[fullCipher.Length - iv.Length];

    Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
    Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, fullCipher.Length - iv.Length);
    var decryptKey = Encoding.UTF8.GetBytes(_options.EncryptKey);

    using var aesAlg = Aes.Create();

    using var decrypted = aesAlg.CreateDecryptor(decryptKey, iv);

    using var msDecrypt = new MemoryStream(cipher);

    using var csDecrypt = new CryptoStream(msDecrypt, decrypted, CryptoStreamMode.Read);

    using var srDecrypt = new StreamReader(csDecrypt);

    var result = srDecrypt.ReadToEnd();


    return result;

  }
}
