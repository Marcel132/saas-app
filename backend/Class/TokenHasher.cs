
using System.Security.Cryptography;
using System.Text;
public class TokenHasher
{
  public static string HashToken(string token)
  {
    using (SHA256 sha256 = SHA256.Create())
    {
        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(token));

        StringBuilder sb = new StringBuilder();
        foreach (var b in bytes)
        {
            sb.Append(b.ToString("x2"));
        }
        return sb.ToString();
    }
  }
}