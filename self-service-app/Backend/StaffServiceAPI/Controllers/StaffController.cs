using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StaffServiceAPI.Data;
using StaffServiceAPI.Models;

namespace StaffServiceAPI.Controllers
{
    /// <summary>
    /// Staff Management Controller
    /// Handles all CRUD operations for staff data
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class StaffController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<StaffController> _logger;

        public StaffController(AppDbContext context, ILogger<StaffController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// GET: api/staff
        /// Get all staff members
        /// DEBUGGING: Set breakpoint on line 31 to see all staff being loaded
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Staff>>> GetStaff()
        {
            _logger.LogInformation("[DEBUG] Getting all staff");

            try
            {
                // SET BREAKPOINT HERE ↓
                var staff = await _context.Staff.ToListAsync();
                
                _logger.LogInformation($"[DEBUG] Retrieved {staff.Count} staff members");
                
                // In WATCH panel, add:
                // - staff.Count
                // - staff.Count(s => s.ImmuneStatus == "Immune")
                // - staff.GroupBy(s => s.Department).Select(g => new { Dept = g.Key, Count = g.Count() })
                
                return Ok(staff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving staff");
                return StatusCode(500, new { error = "Failed to retrieve staff", details = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/staff/5
        /// Get a specific staff member by ID
        /// DEBUGGING: Test with both valid (1-100) and invalid (999) IDs
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<Staff>> GetStaffById(int id)
        {
            _logger.LogInformation($"[DEBUG] Getting staff with ID: {id}");

            // SET BREAKPOINT HERE ↓
            var staff = await _context.Staff.FindAsync(id);

            if (staff == null)
            {
                _logger.LogWarning($"[DEBUG] Staff with ID {id} not found");
                return NotFound(new { error = $"Staff with ID {id} not found" });
            }

            _logger.LogInformation($"[DEBUG] Found staff: {staff.Name}");
            return Ok(staff);
        }

        /// <summary>
        /// GET: api/staff/immune-status/Immune
        /// Filter staff by immune status
        /// DEBUGGING: Try with "Immune", "Non-Immune", and invalid values
        /// </summary>
        [HttpGet("immune-status/{status}")]
        public async Task<ActionResult<IEnumerable<Staff>>> GetStaffByImmuneStatus(string status)
        {
            _logger.LogInformation($"[DEBUG] Filtering by immune status: '{status}'");

            try
            {
                // Debug: Check what statuses exist in database
                var allStatuses = await _context.Staff
                    .Select(s => s.ImmuneStatus)
                    .Distinct()
                    .ToListAsync();
                _logger.LogDebug($"[DEBUG] Available statuses in DB: {string.Join(", ", allStatuses)}");

                // SET BREAKPOINT HERE ↓
                var staff = await _context.Staff
                    .Where(s => s.ImmuneStatus == status)
                    .ToListAsync();

                _logger.LogInformation($"[DEBUG] Found {staff.Count} staff with status '{status}'");

                if (staff.Count == 0)
                {
                    _logger.LogWarning($"[DEBUG] No staff found with status '{status}'. Check for typos or case sensitivity.");
                }

                return Ok(staff);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error filtering by immune status: {status}");
                return StatusCode(500, new { error = "Failed to filter staff", details = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/staff/department/Cardiology
        /// Get all staff in a specific department
        /// </summary>
        [HttpGet("department/{department}")]
        public async Task<ActionResult<IEnumerable<Staff>>> GetStaffByDepartment(string department)
        {
            _logger.LogInformation($"[DEBUG] Getting staff in department: {department}");

            var staff = await _context.Staff
                .Where(s => s.Department == department)
                .OrderBy(s => s.Name)
                .ToListAsync();

            if (staff.Count == 0)
            {
                return NotFound(new { error = $"No staff found in department: {department}" });
            }

            return Ok(staff);
        }

        /// <summary>
        /// POST: api/staff
        /// Create a new staff member
        /// DEBUGGING: Check ModelState validation and database insertion
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<Staff>> CreateStaff([FromBody] Staff staff)
        {
            _logger.LogInformation("[DEBUG] Creating new staff member");

            // Debug: Log incoming data
            _logger.LogDebug($"[DEBUG] Received data:");
            _logger.LogDebug($"  Name: {staff.Name}");
            _logger.LogDebug($"  DOB: {staff.DateOfBirth}");
            _logger.LogDebug($"  Gender: {staff.Gender}");
            _logger.LogDebug($"  Status: {staff.ImmuneStatus}");
            _logger.LogDebug($"  Department: {staff.Department}");

            // SET BREAKPOINT HERE ↓ - Check ModelState
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("[DEBUG] Invalid model state:");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogWarning($"  - {error.ErrorMessage}");
                }
                return BadRequest(ModelState);
            }

            try
            {
                // SET BREAKPOINT HERE ↓ - Before adding to database
                _context.Staff.Add(staff);
                
                // SET BREAKPOINT HERE ↓ - Before saving
                await _context.SaveChangesAsync();

                _logger.LogInformation($"[DEBUG] Staff created with ID: {staff.Id}");

                // Return 201 Created with location header
                return CreatedAtAction(
                    nameof(GetStaffById), 
                    new { id = staff.Id }, 
                    staff
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating staff");
                return StatusCode(500, new { error = "Failed to create staff", details = ex.Message });
            }
        }

        /// <summary>
        /// PUT: api/staff/5
        /// Update an existing staff member
        /// DEBUGGING: Watch the entity state changes and validation
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStaff(int id, [FromBody] Staff staff)
        {
            _logger.LogInformation($"[DEBUG] Updating staff with ID: {id}");

            // Validate ID matches
            if (id != staff.Id)
            {
                _logger.LogWarning($"[DEBUG] ID mismatch: URL={id}, Body={staff.Id}");
                return BadRequest(new { error = "ID in URL does not match ID in request body" });
            }

            // Check if staff exists
            // SET BREAKPOINT HERE ↓
            var existingStaff = await _context.Staff.FindAsync(id);
            if (existingStaff == null)
            {
                _logger.LogWarning($"[DEBUG] Staff with ID {id} not found");
                return NotFound(new { error = $"Staff with ID {id} not found" });
            }

            // Debug: Log what's changing
            _logger.LogDebug($"[DEBUG] Updating:");
            _logger.LogDebug($"  Old Name: {existingStaff.Name} → New: {staff.Name}");
            _logger.LogDebug($"  Old Status: {existingStaff.ImmuneStatus} → New: {staff.ImmuneStatus}");

            try
            {
                // Update properties
                existingStaff.Name = staff.Name;
                existingStaff.DateOfBirth = staff.DateOfBirth;
                existingStaff.Gender = staff.Gender;
                existingStaff.ImmuneStatus = staff.ImmuneStatus;
                existingStaff.Department = staff.Department;

                // SET BREAKPOINT HERE ↓ - Before saving
                await _context.SaveChangesAsync();

                _logger.LogInformation($"[DEBUG] Staff {id} updated successfully");
                return NoContent(); // 204 No Content (standard for successful PUT)
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"Concurrency error updating staff {id}");
                return StatusCode(409, new { error = "The staff record was modified by another user" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating staff {id}");
                return StatusCode(500, new { error = "Failed to update staff", details = ex.Message });
            }
        }

        /// <summary>
        /// DELETE: api/staff/5
        /// Delete a staff member
        /// DEBUGGING: Verify cascade deletes and referential integrity
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStaff(int id)
        {
            _logger.LogInformation($"[DEBUG] Deleting staff with ID: {id}");

            // SET BREAKPOINT HERE ↓
            var staff = await _context.Staff.FindAsync(id);
            
            if (staff == null)
            {
                _logger.LogWarning($"[DEBUG] Staff with ID {id} not found");
                return NotFound(new { error = $"Staff with ID {id} not found" });
            }

            _logger.LogDebug($"[DEBUG] Deleting staff: {staff.Name}");

            try
            {
                // SET BREAKPOINT HERE ↓ - Before removing
                _context.Staff.Remove(staff);
                
                // SET BREAKPOINT HERE ↓ - Before saving
                await _context.SaveChangesAsync();

                _logger.LogInformation($"[DEBUG] Staff {id} deleted successfully");
                
                return Ok(new { 
                    message = "Staff deleted successfully",
                    deletedStaff = new { 
                        id = staff.Id, 
                        name = staff.Name 
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting staff {id}");
                return StatusCode(500, new { error = "Failed to delete staff", details = ex.Message });
            }
        }

        /// <summary>
        /// GET: api/staff/search?name=john
        /// Search staff by name (case-insensitive)
        /// DEBUGGING: Test LINQ queries and string comparisons
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Staff>>> SearchStaff([FromQuery] string name)
        {
            _logger.LogInformation($"[DEBUG] Searching for staff with name containing: '{name}'");

            if (string.IsNullOrWhiteSpace(name))
            {
                return BadRequest(new { error = "Search term 'name' is required" });
            }

            // SET BREAKPOINT HERE ↓
            var staff = await _context.Staff
                .Where(s => s.Name.ToLower().Contains(name.ToLower()))
                .OrderBy(s => s.Name)
                .ToListAsync();

            _logger.LogInformation($"[DEBUG] Found {staff.Count} staff members matching '{name}'");

            return Ok(staff);
        }

        /// <summary>
        /// GET: api/staff/stats
        /// Get summary statistics
        /// DEBUGGING: Practice with aggregation functions
        /// </summary>
        [HttpGet("stats")]
        public async Task<ActionResult<object>> GetStatistics()
        {
            _logger.LogInformation("[DEBUG] Calculating staff statistics");

            try
            {
                // SET BREAKPOINT HERE ↓
                var total = await _context.Staff.CountAsync();
                var immune = await _context.Staff.CountAsync(s => s.ImmuneStatus == "Immune");
                var nonImmune = await _context.Staff.CountAsync(s => s.ImmuneStatus == "Non-Immune");

                // Group by department
                var byDepartment = await _context.Staff
                    .GroupBy(s => s.Department)
                    .Select(g => new
                    {
                        department = g.Key,
                        count = g.Count(),
                        immune = g.Count(s => s.ImmuneStatus == "Immune"),
                        nonImmune = g.Count(s => s.ImmuneStatus == "Non-Immune")
                    })
                    .OrderByDescending(x => x.count)
                    .ToListAsync();

                var stats = new
                {
                    totalStaff = total,
                    immuneCount = immune,
                    nonImmuneCount = nonImmune,
                    immunePercentage = total > 0 ? Math.Round((double)immune / total * 100, 2) : 0,
                    departmentBreakdown = byDepartment
                };

                _logger.LogInformation($"[DEBUG] Stats calculated: {total} total, {immune} immune, {nonImmune} non-immune");

                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating statistics");
                return StatusCode(500, new { error = "Failed to calculate statistics", details = ex.Message });
            }
        }

        /// <summary>
        /// POST: api/staff/bulk
        /// Create multiple staff members at once
        /// DEBUGGING: Watch transaction handling and bulk operations
        /// </summary>
        [HttpPost("bulk")]
        public async Task<ActionResult<object>> CreateBulkStaff([FromBody] List<Staff> staffList)
        {
            _logger.LogInformation($"[DEBUG] Creating {staffList.Count} staff members in bulk");

            if (staffList == null || staffList.Count == 0)
            {
                return BadRequest(new { error = "Staff list cannot be empty" });
            }

            try
            {
                // Validate all staff
                for (int i = 0; i < staffList.Count; i++)
                {
                    if (string.IsNullOrWhiteSpace(staffList[i].Name))
                    {
                        return BadRequest(new { error = $"Staff at index {i} has invalid name" });
                    }
                }

                // SET BREAKPOINT HERE ↓ - Before bulk insert
                await _context.Staff.AddRangeAsync(staffList);
                
                // SET BREAKPOINT HERE ↓ - Before saving
                await _context.SaveChangesAsync();

                _logger.LogInformation($"[DEBUG] Successfully created {staffList.Count} staff members");

                return Ok(new
                {
                    message = "Staff created successfully",
                    count = staffList.Count,
                    ids = staffList.Select(s => s.Id).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating bulk staff");
                return StatusCode(500, new { error = "Failed to create staff", details = ex.Message });
            }
        }
    }
}