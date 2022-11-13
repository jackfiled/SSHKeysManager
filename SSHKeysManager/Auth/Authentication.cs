using Microsoft.IdentityModel.Tokens;
using System.Text;
using SSHKeysManager.Common;

namespace SSHKeysManager.Auth
{
    /// <summary>
    /// 管理程序中的身份验证部分
    /// </summary>
    public class Authentication
    {
        /// <summary>
        /// 返回校验用户JWT Token的校验参数
        /// </summary>
        /// <returns>令牌校验参数</returns>
        public static TokenValidationParameters GenerateUserJWTTokenValidationParameters()
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,

                ValidIssuer = Const.Issuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Const.JwtSecret)),
            };
        }
    }
}
