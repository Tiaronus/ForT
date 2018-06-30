using System.Security.Cryptography;
using System.Text;

namespace ExtensionMethods
{
    public static class ExtMethods
    {
        public static string MD5Hash(this string s)
        {
            MD5CryptoServiceProvider provider;
            provider = new MD5CryptoServiceProvider();
            byte[] bytes = Encoding.UTF8.GetBytes(s);
            StringBuilder builder = new StringBuilder();
            bytes = provider.ComputeHash(bytes);
            foreach (byte b in bytes) builder.Append(b.ToString("x2").ToLower());
            return builder.ToString();
        }        
    }
}

