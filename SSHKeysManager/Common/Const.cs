namespace SSHKeysManager.Common
{
    /// <summary>
    /// 代码中涉及的部分常量
    /// </summary>
    public class Const
    {
        /// <summary>
        /// Sqlite数据库文件的名称
        /// </summary>
        public const string DatabaseFileName = "SSHKeysManager.db";

        /// <summary>
        /// 给数据库中密码加盐
        /// </summary>
        public const string PasswordSalt = "dkvnksdfjvnmcvndfkvnlkcvvjfjkvhsdfmvnkdfvneajkvhsdfuvsdfkmvnsdjfvndfjbnef";

        /// <summary>
        /// Jwt密钥
        /// </summary>
        public const string JwtSecret = "cvhajwebefjkchvkjnasdkuvhcjnvuqwe";
        /// <summary>
        /// Jwt密钥签发者
        /// </summary>
        public const string Issuer = "SSHKeyManager";
    }

    /// <summary>
    /// 用户权限枚举
    /// </summary>
    public enum UserPermission
    {
        User,
        Administrator
    }
}
