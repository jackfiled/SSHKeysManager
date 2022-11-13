using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using SSHKeysManager.Common;
using SSHKeysManager.Models;


namespace SSHKeysManager.Controllers
{
    /// <summary>
    /// 处理登录、注册、修改密码等的控制器
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserContext userContext;
        private readonly IConfiguration configuration;

        public AccountController(UserContext userContext, IConfiguration configuration)
        {
            this.userContext = userContext;
            this.configuration = configuration;
        }

        /// <summary>
        /// 登录请求处理函数
        /// </summary>
        /// <param name="loginModel">登录JSON模型</param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {
            string result = await _validateEmailAndPassword(loginModel.EmailAddress, loginModel.Password);

            if (string.IsNullOrEmpty(result))
            {
                return BadRequest("Emaill Address or Password is invaild");
            }
            else
            {
                loginModel.Password = Utils.HashPassword10000(loginModel.Password);
                var user = await userContext.Users.SingleAsync(u => u.Password == loginModel.Password);
                
                // 登录之后还是返回一些基本的信息
                return Ok(new
                {
                    user.Name,
                    user.EmailAddress,
                    user.Permission,
                    Token = result,
                });
            }
        }

        /// <summary>
        /// 注册请求处理函数
        /// </summary>
        /// <param name="registerModel">注册JSON模型</param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            // 判断数据库中是否存在相同的电子邮件地址
            // 不允许相同的电子邮件地址
            var user = await userContext.Users.SingleOrDefaultAsync(u => u.EmailAddress == registerModel.EmailAddress);
            
            if (user != null)
            {
                return BadRequest("Email Address is invalid");
            }

            User newUser = new User();
            newUser.EmailAddress = registerModel.EmailAddress;
            newUser.Name = registerModel.Name;
            newUser.Permission = (int)UserPermission.User;
            newUser.Password = Utils.HashPassword10000(registerModel.Password);

            userContext.Users.Add(newUser);
            await userContext.SaveChangesAsync();

            return Ok();
        }

        /// <summary>
        /// 修改密码请求处理函数
        /// </summary>
        /// <param name="passwordModel">修改密码JSON模型</param>
        /// <returns></returns>
        [HttpPost("password")]
        public async Task<IActionResult> ChangePassword(PasswordModel passwordModel)
        {
            string result = await _validateEmailAndPassword(passwordModel.EmailAddress, passwordModel.oldPassword);

            if (string.IsNullOrEmpty(result))
            {
                return BadRequest("EmaillAddress or password is invalid");
            }

            var user = await userContext.Users.SingleAsync(u => u.EmailAddress == passwordModel.EmailAddress);

            user.Password = Utils.HashPassword10000(passwordModel.newPassword);

            await userContext.SaveChangesAsync();
            return Ok();
        }

        /// <summary>
        /// 验证电子邮件地址和密码是否正确
        /// </summary>
        /// <param name="emailAddress">电子邮件地址</param>
        /// <param name="password">密码</param>
        /// <returns>验证成功为jwt字符串 验证失败为空字符串</returns>
        private async Task<string> _validateEmailAndPassword(string emailAddress, string password)
        {
            if (string.IsNullOrEmpty(emailAddress) || string.IsNullOrEmpty(password))
            {
                // 如果电子邮件地址或者密码为空
                // 返回空字符串
                return "";
            }

            var user = await userContext.Users.SingleOrDefaultAsync(u => u.EmailAddress == emailAddress); ;
            if (user == null)
            {
                return "";
            }

            // 密码加盐哈希10000次
            password = Utils.HashPassword10000(password);

            if (user.Password != password)
            {
                return "";
            }

            // 个人定制的信息
            var claims = new[]
            {
                new Claim("Name", user.Name),
                new Claim("Permission", user.Permission.ToString()),
            };

            string? jwtSecret = configuration["JwtSecret"];
            SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Const.JwtSecret));
            // 由于目前在验证身份中间件中无法访问依赖注入框架
            // 暂时无法自定义生成JWT令牌的密钥
            /*// 如果在配置文件中没有读取到JWT密钥配置
            // 采用程序中常量的默认值
            if (jwtSecret != null)
            {
                key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            }
            else
            {
                key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Const.JwtSecret));
            }*/

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(
                issuer: Const.Issuer,
                audience: user.EmailAddress,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddDays(3),
                claims: claims,
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

#nullable disable
    /// <summary>
    /// 登录时执行的JSON绑定模型
    /// </summary>
    public class LoginModel
    {
        public string EmailAddress { get; set; }
        public string Password { get; set; }
    }

    /// <summary>
    /// 注册时执行的JSON绑定模型
    /// </summary>
    public class RegisterModel
    {
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
    }

    /// <summary>
    /// 修改密码时执行的JSON绑定模型
    /// </summary>
    public class PasswordModel
    {
        public string EmailAddress { get; set; }
        public string oldPassword { get; set; }
        public string newPassword { get; set; }
    }
}
