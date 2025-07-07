using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MSPR_BLOC_4_USER.Models;
using System.Security.Claims;
using BCrypt.Net;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly UserDbContext _context;

    public UserController(UserDbContext context) => _context = context;

    // POST: api/User/register
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<User>> Register(User user)
    {
        if (await _context.Users.AnyAsync(u => u.Username == user.Username))
        {
            return Conflict("Username already exists.");
        }

        user.CreatedAt = DateTime.UtcNow;
        user.LastModifiedAt = DateTime.UtcNow;
        user.LastLoginAt = null;

        // Hash sécurisé
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
    }

    // PUT: api/User/update/{id}
    [Authorize]
    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateUser(int id, User updatedUser)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        var loggedInUsername = User.Identity?.Name;
        if (loggedInUsername != user.Username && !User.IsInRole("admin"))
        {
            return Forbid("You can only modify your own account unless you are admin.");
        }

        user.Username = updatedUser.Username ?? user.Username;
        user.AccountType = User.IsInRole("admin") ? updatedUser.AccountType ?? user.AccountType : user.AccountType;
        user.LastModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // PUT: api/User/update-password/{id}
    [Authorize]
    [HttpPut("update-password/{id}")]
    public async Task<IActionResult> UpdatePassword(int id, [FromBody] string newPassword)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        var loggedInUsername = User.Identity?.Name;
        if (loggedInUsername != user.Username && !User.IsInRole("admin"))
        {
            return Forbid("You can only change your own password unless you are admin.");
        }

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
        user.LastModifiedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE: api/User/delete/{id}
    [Authorize]
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        var loggedInUsername = User.Identity?.Name;
        if (loggedInUsername != user.Username && !User.IsInRole("admin"))
        {
            return Forbid("You can only delete your own account unless you are admin.");
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    // GET: api/User
    [Authorize(Roles = "admin")]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetAllUsers()
    {
        return await _context.Users.ToListAsync();
    }

    // GET: api/User/{id}
    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUserById(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user == null) return NotFound();

        var loggedInUsername = User.Identity?.Name;
        if (loggedInUsername != user.Username && !User.IsInRole("admin") && !User.IsInRole("moderator"))
        {
            return Forbid("You can only view your own account unless you are admin or moderator.");
        }

        return user;
    }
}
