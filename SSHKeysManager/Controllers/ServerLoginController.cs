﻿using Microsoft.EntityFrameworkCore;
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
            foreach (var relation in query)
            {
                var tokenQuery = from key in sshKeysContext.Keys
                                 where key.UserId == relation.UserId
                                 select key.Key;

                result.AppendLine(tokenQuery.ToString());
            }

            return Ok(result.ToString());
        }
    }
}