using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SSHKeysManager.Models;

namespace SSHKeysManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(AuthenticationSchemes = "userAuthentication")]
    public class RelationController : ControllerBase
    {
        private readonly UserServerRelationContext userServerRelationContext;

        public RelationController(UserServerRelationContext userServerRelationContext)
        {
            this.userServerRelationContext = userServerRelationContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserServerRelation>>> GetAllRelations(int? user=null, int? server=null)
        {
            var relations = (IQueryable<UserServerRelation>)userServerRelationContext.Relations;

            if (user != null)
            {
                relations = relations.Where(r => r.UserId == user);
            }
            if (server != null)
            {
                relations = relations.Where(r => r.ServerId== server);
            }

            return Ok(await relations.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserServerRelation>> GetSingleRelation(long id)
        {
            var relation = await userServerRelationContext.Relations.FindAsync(id);

            if (relation == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(relation);
            }
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = "userAuthentication", Policy = "IsAdministrator")]
        public async Task<ActionResult<UserServerRelation>> CreateRelaion(UserServerRelation relation)
        {
            var oldRelation = await userServerRelationContext.Relations.SingleOrDefaultAsync(
                r => r.UserId== relation.UserId && r.ServerId== relation.ServerId);

            if (oldRelation != null)
            {
                return BadRequest();
            }
            else
            {
                userServerRelationContext.Relations.Add(relation);
                await userServerRelationContext.SaveChangesAsync();

                return CreatedAtAction(
                    nameof(GetSingleRelation),
                    new { relation.Id },
                    relation);
            }
        }

        [HttpDelete("{id}")]
        [Authorize(AuthenticationSchemes = "userAuthentication", Policy = "IsAdministrator")]
        public async Task<IActionResult> DeleteRelation(long id)
        {
            var relation = await userServerRelationContext.Relations.FindAsync(id);

            if (relation == null)
            {
                return NotFound();
            }
            else
            {
                userServerRelationContext.Relations.Remove(relation);
                await userServerRelationContext.SaveChangesAsync();

                return NoContent();
            }
        }
    }
}
