using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SSHKeysManager.Models;

namespace SSHKeysManager.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = "userAuthentication", Policy = "IsAdministrator")]
    public class UserController : ControllerBase
    {
        private readonly UserContext userContext;
        private readonly IConfiguration configuration;

        public UserController(UserContext userContext, IConfiguration configuration)
        {
            this.userContext = userContext;
            this.configuration = configuration;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await userContext.Users.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetSingleUser(long id)
        {
            var user = await userContext.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            userContext.Users.Add(user);
            await userContext.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetSingleUser),
                new { id = user.Id },
                user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(long id, User user)
        {
            if (id != user.Id)
            {
                return BadRequest();
            }

            var oldUser = await userContext.Users.FindAsync(id);
            if (oldUser == null)
            {
                return NotFound();
            }

            oldUser.Name = user.Name;
            oldUser.EmailAddress= user.EmailAddress;
            
            await userContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            var user = await userContext.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            userContext.Users.Remove(user);
            await userContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
