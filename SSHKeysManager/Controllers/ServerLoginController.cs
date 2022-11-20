using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using SSHKeysManager.Models;
using System.Text;

namespace SSHKeysManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServerLoginController : ControllerBase
    {
        private readonly ServerContext serverContext;
        private readonly UserServerRelationContext userServerRelationContext;
        private readonly SSHKeysContext sshKeysContext;

        public ServerLoginController(ServerContext serverContext, UserServerRelationContext userServerRelationContext, SSHKeysContext sshKeysContext)
        {
            this.serverContext = serverContext;
            this.userServerRelationContext = userServerRelationContext;
            this.sshKeysContext = sshKeysContext;
        }

        [HttpGet]
        public async Task<ActionResult<string>> ServerLogin(string token)
        {
            var server = await serverContext.Servers.SingleOrDefaultAsync(x => x.Token == token);

            if (server == null)
            {
                return NotFound();
            }

            var query = from relation in userServerRelationContext.Relations
                        where relation.ServerId == server.Id
                        select relation;

            StringBuilder result = new StringBuilder();
            var relations = query.ToArray();
            foreach (var relation in relations)
            {
                var tokenQuery = from key in sshKeysContext.Keys
                                 where key.UserId == relation.UserId
                                 select key.Key;

                string[] tokens = tokenQuery.ToArray();
                foreach(string t in tokens)
                {
                    result.AppendLine(t);
                }
            }

            return Ok(result.ToString());
        }
    }
}
