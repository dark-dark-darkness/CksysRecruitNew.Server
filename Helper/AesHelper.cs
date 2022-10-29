using System.Security.Cryptography;
using System.Text;

namespace CksysRecruitNew.Server.Helper;

class AesHelper {

  public static string AESEncrypt(string EncryptString, string EncryptKey) {

    if (string.IsNullOrEmpty(EncryptString)) {
      throw (new Exception("密文不得为空"));
    }


    if (string.IsNullOrEmpty(EncryptKey)) {
      throw (new Exception("密钥不得为空"));
    }


    string m_strEncrypt = "";


    byte[] m_btIV = Convert.FromBase64String("Rkb4jvUy/ye7Cd7k89QQgQ==");


    Rijndael m_AESProvider = Rijndael.Create();


    try {

      byte[] m_btEncryptString = Encoding.Default.GetBytes(EncryptString);


      MemoryStream m_stream = new MemoryStream();


      CryptoStream m_csstream = new CryptoStream(m_stream, m_AESProvider.CreateEncryptor(Encoding.Default.GetBytes(EncryptKey), m_btIV), CryptoStreamMode.Write);


      m_csstream.Write(m_btEncryptString, 0, m_btEncryptString.Length);
      m_csstream.FlushFinalBlock();


      m_strEncrypt = Convert.ToBase64String(m_stream.ToArray());


      m_stream.Close();
      m_stream.Dispose();


      m_csstream.Close();
      m_csstream.Dispose();

    } catch (IOException ex) {
      throw ex;
    } catch (CryptographicException ex) {
      throw ex;
    } catch (ArgumentException ex) {
      throw ex;
    } catch (Exception ex) {
      throw ex;
    } finally {
      m_AESProvider.Clear();
    }


    return m_strEncrypt;

  }

  /// <summary>
  /// AES 解密(高级加密标准，是下一代的加密算法标准，速度快，安全级别高，目前 AES 标准的一个实现是 Rijndael 算法)
  /// </summary>
  /// <param name="DecryptString">待解密密文</param>
  /// <param name="DecryptKey">解密密钥</param>
  /// <returns></returns>
  public static string AESDecrypt(string DecryptString, string DecryptKey) {

    if (string.IsNullOrEmpty(DecryptString)) {
      throw (new Exception("密文不得为空"));
    }


    if (string.IsNullOrEmpty(DecryptKey)) {
      throw (new Exception("密钥不得为空"));
    }


    string m_strDecrypt = "";


    byte[] m_btIV = Convert.FromBase64String("Rkb4jvUy/ye7Cd7k89QQgQ==");


    Rijndael m_AESProvider = Rijndael.Create();


    try {

      byte[] m_btDecryptString = Convert.FromBase64String(DecryptString);


      MemoryStream m_stream = new MemoryStream();


      CryptoStream m_csstream = new CryptoStream(m_stream, m_AESProvider.CreateDecryptor(Encoding.Default.GetBytes(DecryptKey), m_btIV), CryptoStreamMode.Write);


      m_csstream.Write(m_btDecryptString, 0, m_btDecryptString.Length);
      m_csstream.FlushFinalBlock();


      m_strDecrypt = Encoding.Default.GetString(m_stream.ToArray());


      m_stream.Close();
      m_stream.Dispose();


      m_csstream.Close();
      m_csstream.Dispose();

    } catch (IOException ex) {
      throw ex;
    } catch (CryptographicException ex) {
      throw ex;
    } catch (ArgumentException ex) {
      throw ex;
    } catch (Exception ex) {
      throw ex;
    } finally {
      m_AESProvider.Clear();
    }


    return m_strDecrypt;

  }
}
