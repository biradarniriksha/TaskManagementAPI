using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using TaskManagementAPI.DataBase;
using TaskManagementAPI.Models;

namespace TaskManagementAPI.Controllers
{
    [Route("api")]
    [ApiController]
    
    public class TaskManagementController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<TaskManagementController> _logger;

        public TaskManagementController(AppDbContext context, ILogger<TaskManagementController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // POST: api/tasks
        [HttpPost("tasks")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> CreateTask([FromBody] TaskItem task)
        {

            if (string.IsNullOrEmpty(task.Title))
                return BadRequest("Title is required");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                _context.Tasks.Add(task);
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(GetTaskById), new { id = task.Id }, task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                throw new Exception("Failed to create task", ex);
            }
            
        }

        // GET: api/tasks/{id}
        [HttpGet("tasks/{id}")]
        [Authorize]
        public async Task<IActionResult> GetTaskById(int id)
        {
            try
            {
                var task = await _context.Tasks.FindAsync(id);

                if (task == null)
                    return NotFound();

                return Ok(task);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);

                throw new Exception("Failed to retrive task.", ex);
            }
            
        }

        // GET: api/tasks/user/{userId}
        [HttpGet("tasks/user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetTasksByUserId(int userId)
        {
            try
            {
                var tasks = await _context.Tasks
                    .Where(t => t.UserId == userId)
                    .ToListAsync();

                if (tasks == null)
                    return NotFound();

                return Ok(tasks);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);

                throw new Exception("Failed to retrive users.", ex);
            }  
        }
        [HttpGet("tasks/debug-auth")]
        [Authorize] 
        public IActionResult DebugAuth()
        {
            return Ok(new
            {
                Username = User.FindFirst(ClaimTypes.Name)?.Value,
                Role = User.FindFirst(ClaimTypes.Role)?.Value,
                AllClaims = User.Claims.Select(c => new { c.Type, c.Value })
            });
        }
    }
}
