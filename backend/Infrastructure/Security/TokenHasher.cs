
using System.Security.Cryptography;
using System.Text;
public class TokenHasher
{
public static string HashToken(string token)
{
  var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(token));
  return Convert.ToHexString(bytes).ToLower();
}
}