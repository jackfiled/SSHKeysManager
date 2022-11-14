using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SSHKeysManager.Common;
using SSHKeysManager.Models;

namespace SSHKeysManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = "userAuthentication", Policy = "IsAdministratorOrOwner")]
    public class SSHKeysController : ControllerBase
    {
        private readonly SSHKeysContext sshKeysContext;
        private readonly UserContext userContext;

        public SSHKeysController(SSHKeysContext sshKeysContext, UserContext userContext)
        {
            this.sshKeysContext = sshKeysContext;
            this.userContext = userContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SSHKey>>> GetAllKeys()
        {
            var keys = await sshKeysContext.Keys.ToListAsync();

            return Ok(keys);
        }

        [HttpGet("{user}")]
        public async Task<ActionResult<IEnumerable<SSHKey>>> GetSingleUserKeys(long user)
        {
            var keys = await sshKeysContext.Keys
                .Where(k => k.UserId== user)
                .ToListAsync();

            // 这里没有校验请求的用户是否存在
            return Ok(keys);
        }

        [HttpGet("{user}/{id}")]
        public async Task<ActionResult<SSHKey>> GetSingleKey(long user, long id)
        {
            var key = await sshKeysContext.Keys.FindAsync(id);

            if (key == null)
            {
                return NotFound();
            }

            // 判断请求的用户id和数据库中记录的用户ID是否一致
            if (key.UserId == user)
            {
                return Ok(key);
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("{user}")]
        public async Task<IActionResult> CreateKey(long user, SSHKey key)
        {
            if (user != key.UserId)
            {
                return BadRequest();
            }

            var userItem = await userContext.Users.FindAsync(user);
            if (userItem == null)
            {
                // 虽然在理论上不会出现
                // 还是礼节性的处理一下
                return BadRequest();
            }

            if (string.IsNullOrEmpty(key.Key))
            {
                // 服务器生成公钥私钥对
                string[] keys = Utils.GenerateSSHKeys(userItem.EmailAddress);

                key.Key = keys[1];

                sshKeysContext.Keys.Add(key);
                await sshKeysContext.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetSingleKey),
                    new { key.Id },
                    new
                    {
                        privateKey = keys[0],
                        key.Id,
                        key.Key,
                        key.UserId
                    });
            }
            else
            {
                sshKeysContext.Keys.Add(key);
                await sshKeysContext.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetSingleKey),
                    new { key.Id },
                    key);
            }
        }

        [HttpDelete("{user}/{id}")]
        public async Task<IActionResult> DeleteKey(long user, long id)
        {
            var key = await sshKeysContext.Keys.FindAsync(id);

            if (key == null)
            {
                return NotFound();
            }

            // 判断请求的用户id和数据库中记录的用户ID是否一致
            if (key.UserId == user)
            {
                sshKeysContext.Keys.Remove(key);
                await sshKeysContext.SaveChangesAsync();

                return NoContent();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
