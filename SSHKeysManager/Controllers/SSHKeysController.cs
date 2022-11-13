using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SSHKeysManager.Models;

namespace SSHKeysManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = "userAuthentication", Policy = "IsAdministratorOrOwner")]
    public class SSHKeysController : ControllerBase
    {
        private readonly SSHKeysContext sshKeysContext;

        public SSHKeysController(SSHKeysContext sshKeysContext)
        {
            this.sshKeysContext = sshKeysContext;
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
        public async Task<ActionResult<Server>> GetSingleKey(long user, long id)
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
        public async Task<ActionResult<SSHKey>> CreateKey(long user, SSHKey key)
        {
            if (user != key.UserId)
            {
                return BadRequest();
            }

            if (string.IsNullOrEmpty(key.Key))
            {
                // 等待实现服务器生成公钥私钥对
            }

            sshKeysContext.Keys.Add(key);
            await sshKeysContext.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetSingleKey),
                new { key.Id },
                key);
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
