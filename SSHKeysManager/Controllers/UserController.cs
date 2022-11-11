using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using SSHKeysManager.Models;

namespace SSHKeysManager.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserContext _userContext;

        public UserController(UserContext userContext)
        {
            _userContext = userContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _userContext.Users.ToListAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetSingleUser(long id)
        {
            var user = await _userContext.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost]
        public async Task<ActionResult<User>> CreateUser(User user)
        {
            _userContext.Users.Add(user);
            await _userContext.SaveChangesAsync();

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

            var oldUser = await _userContext.Users.FindAsync(id);
            if (oldUser == null)
            {
                return NotFound();
            }

            oldUser.Name = user.Name;
            oldUser.EmailAddress= user.EmailAddress;
            
            await _userContext.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(long id)
        {
            var user = await _userContext.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            _userContext.Users.Remove(user);
            await _userContext.SaveChangesAsync();

            return NoContent();
        }
    }
}
