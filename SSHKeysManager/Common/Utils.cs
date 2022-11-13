using System.Security.Cryptography;
using System.Text;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using SSHKeysManager.Models;
using Microsoft.Data.Sqlite;

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

        /// <summary>
        /// 计算密码加盐哈希10000次的结果
        /// </summary>
        /// <param name="password">未加密的密码</param>
        /// <returns>已加密的密码</returns>
        public static string HashPassword10000(string password)
        {
            for(int i = 0; i < 10000; i++)
            {
                password = Hash256(password, Const.PasswordSalt);
            }

            return password;
        }

        /// <summary>
        /// 随机生成32位字符串令牌
        /// </summary>
        /// <returns></returns>
        public static string GenerateRandomToken()
        {
            Random random = new Random();
            byte[] bytes = new Byte[32];

            random.NextBytes(bytes);
            return Convert.ToBase64String(bytes);
        }

        public static void SetupDatabase()
        {
            // 建立数据库连接
            DbConnection connection = new SqliteConnection("Filename=example.db");
            connection.Open();
            var userContextOptions = new DbContextOptionsBuilder<UserContext>()
                .UseSqlite(connection)
                .Options;

            using var userContext = new UserContext(userContextOptions);

            // 检查数据库中是否存在默认管理员
            var defaultAdministrator = userContext.Users.SingleOrDefault(
                u => u.Name == "admin");

            if (defaultAdministrator == null)
            {
                User user = new User();
                user.Name = "admin";
                user.Permission = (int)UserPermission.Administrator;
                user.EmailAddress = "admin@admin.com";
                //string password = Hash256("admin", "");
                user.Password = HashPassword10000("admin");
                
                userContext.Users.Add(user);
                userContext.SaveChanges();
            }
        }
    }
}
