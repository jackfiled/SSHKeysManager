using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SSHKeysManager.Models;
using SSHKeysManager.Common;

namespace SSHKeysManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = "userAuthentication", Policy = "IsAdministrator")]
    public class ServerController : ControllerBase
    {
        private readonly ServerContext serverContext;

        public ServerController(ServerContext serverContext)
        {
            this.serverContext = serverContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Server>>> GetServerList()
        {
            var servers = await serverContext.Servers.ToListAsync();

            return Ok(servers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Server>> GetSingleServer(long id)
        {
            var server = await serverContext.Servers.FindAsync(id);

            if (server == null)
            {
                return BadRequest("Id is invalid");
            }
            else
            {
                return Ok(server);
            }
        }

        [HttpPost]
        public async Task<ActionResult<Server>> CreateServer(Server s)
        {
            var sever = await serverContext.Servers.SingleOrDefaultAsync(s => s.Name== s.Name);

            if (sever == null)
            {
                Server newServer = new Server();
                newServer.Name = s.Name;
                if (string.IsNullOrEmpty(s.Token))
                {
                    // 如果创建时没有自定义令牌
                    // 随机生成一个令牌
                    newServer.Token = Utils.GenerateRandomToken();
                }
                else
                {
                    newServer.Token = s.Token;
                }
                serverContext.Servers.Add(newServer);
                await serverContext.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetSingleServer),
                    new { newServer.Id },
                    newServer);
            }
            else
            {
                return BadRequest("Invalid server name");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateServer(long id, Server server)
        {
            if (id != server.Id)
            {
                return BadRequest();
            }

            var oldServer = await serverContext.Servers.FindAsync(id);
            if (oldServer == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(server.Name))
            {
                oldServer.Name = server.Name;
            }
            if (!string.IsNullOrEmpty(server.Token))
            {
                oldServer.Token = server.Token;
            }
            await serverContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServer(long id)
        {
            var server = await serverContext.Servers.FindAsync(id);

            if (server == null)
            {
                return NotFound();
            }

            serverContext.Servers.Remove(server);
            await serverContext.SaveChangesAsync();

            return NoContent();
        }
        
    }
}
