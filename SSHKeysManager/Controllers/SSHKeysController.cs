using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SSHKeysManager.Models;

namespace SSHKeysManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SSHKeysController : ControllerBase
    {
        private readonly SSHKeysContext _sshKeysContext;

        public SSHKeysController(SSHKeysContext sshKeysContext)
        {
            _sshKeysContext = sshKeysContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SSHKey>>> GetAllKeys()
        {
            var keys = await _sshKeysContext.Keys.ToListAsync();

            return Ok(keys);
        }

        [HttpGet("{user}")]
        public async Task<ActionResult<IEnumerable<SSHKey>>> GetSingleUserKeys(long user)
        {
            var keys = await _sshKeysContext.Keys
                .Where(k => k.UserId== user)
                .ToListAsync();

            if (keys.Any())
            {
                return Ok(keys);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("{user}/{id}")]
        public async Task<ActionResult<Server>> GetSingleKey(long user, long id)
        {
            var key = await _sshKeysContext.Keys.FindAsync(id);

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

            _sshKeysContext.Keys.Add(key);
            await _sshKeysContext.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetSingleKey),
                new { key.Id },
                key);
        }

        [HttpDelete("{user}/{id}")]
        public async Task<IActionResult> DeleteKey(long user, long id)
        {
            var key = await _sshKeysContext.Keys.FindAsync(id);

            if (key == null)
            {
                return NotFound();
            }

            // 判断请求的用户id和数据库中记录的用户ID是否一致
            if (key.UserId == user)
            {
                _sshKeysContext.Keys.Remove(key);
                await _sshKeysContext.SaveChangesAsync();

                return NoContent();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
