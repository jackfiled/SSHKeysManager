using System.Security.Cryptography;
using System.Text;

namespace SSHKeysManager.Common
{
    public class Utils
    {
        /// <summary>
        /// 利用指定的输入和密钥计算哈希值
        /// </summary>
        /// <param name="input">输入</param>
        /// <param name="keys">密钥</param>
        /// <returns>十六进制表示的哈希值</returns>
        public static string Hash256(string input, string keys)
        {
            var bytes = Encoding.UTF8.GetBytes(input);
            var keyBytes = Encoding.UTF8.GetBytes(keys);
            using (var hmacsha256 = new HMACSHA256(keyBytes))
            {
                byte[] result = hmacsha256.ComputeHash(bytes);
                return Convert.ToHexString(result);

            }
        }
    }
}
