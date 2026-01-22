using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StaffServiceAPI.Data;
using StaffServiceAPI.Models;

namespace StaffServiceAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly ILogger<UsersController> _logger;

    public UsersController(AppDbContext context, ILogger<UsersController> logger)
    {
        _context = context;
        _logger = logger;
    }

    // GET: api/users
    [HttpGet]
    public async Task<ActionResult<IEnumerable<User>>> GetUsers()
    {
        try
        {
            var users = await _context.Users.ToListAsync();
            _logger.LogInformation("Retrieved {Count} users from database", users.Count);
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            return StatusCode(500, new { error = "An error occurred while retrieving users" });
        }
    }

    // GET: api/users/5
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetUser(int id)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                _logger.LogWarning("User with ID {Id} not found", id);
                return NotFound(new { error = $"User with ID {id} not found" });
            }

            _logger.LogInformation("Retrieved user with ID {Id}", id);
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user with ID {Id}", id);
            return StatusCode(500, new { error = "An error occurred while retrieving the user" });
        }
    }

    // GET: api/users/immune-status/Immune
    [HttpGet("immune-status/{status}")]
    public async Task<ActionResult<IEnumerable<User>>> GetUsersByImmuneStatus(string status)
    {
        try
        {
            var validStatuses = new[] { "Immune", "Non-Immune", "Unknown" };
            if (!validStatuses.Contains(status))
            {
                return BadRequest(new { error = "Invalid immune status. Must be 'Immune', 'Non-Immune', or 'Unknown'" });
            }

            var users = await _context.Users
                .Where(u => u.ImmuneStatus == status)
                .ToListAsync();

            _logger.LogInformation("Retrieved {Count} users with immune status '{Status}'", users.Count, status);
            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users by immune status");
            return StatusCode(500, new { error = "An error occurred while filtering users" });
        }
    }

    // POST: api/users
    [HttpPost]
    public async Task<ActionResult<User>> CreateUser(User user)
    {
        try
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Created new user with ID {Id}", user.Id);
            return CreatedAtAction(nameof(GetUser), new { id = user.Id }, user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return StatusCode(500, new { error = "An error occurred while creating the user" });
        }
    }

    // PUT: api/users/5
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, User user)
    {
        if (id != user.Id)
        {
            return BadRequest(new { error = "ID mismatch" });
        }

        _context.Entry(user).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
            _logger.LogInformation("Updated user with ID {Id}", id);
            return NoContent();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            if (!await UserExists(id))
            {
                _logger.LogWarning("User with ID {Id} not found during update", id);
                return NotFound(new { error = $"User with ID {id} not found" });
            }
            else
            {
                _logger.LogError(ex, "Concurrency error updating user with ID {Id}", id);
                throw;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user with ID {Id}", id);
            return StatusCode(500, new { error = "An error occurred while updating the user" });
        }
    }

    // DELETE: api/users/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                _logger.LogWarning("User with ID {Id} not found during deletion", id);
                return NotFound(new { error = $"User with ID {id} not found" });
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted user with ID {Id}", id);
            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user with ID {Id}", id);
            return StatusCode(500, new { error = "An error occurred while deleting the user" });
        }
    }

    private async Task<bool> UserExists(int id)
    {
        return await _context.Users.AnyAsync(e => e.Id == id);
    }
}
