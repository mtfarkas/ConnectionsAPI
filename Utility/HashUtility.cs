using System.Security.Cryptography;
using System.Text;

namespace ConnectionsAPI.Utility;
public static class HashUtility
{
    public static string CalculateMD5(string input)
    {
        byte[] hash = MD5.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(hash).Replace("-", string.Empty).ToLower();
    }
}
