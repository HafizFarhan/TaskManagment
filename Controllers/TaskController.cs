using Injazat.DataAccess.Models; // This is your Task model
using Injazat.Presentation.Services.TaskService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks; // Required for async Task
using TaskEntity = Injazat.DataAccess.Models.Task; // Alias to avoid ambiguity

namespace Injazat.Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly ILogger<TaskController> _logger;

        public TaskController(ITaskService taskService, ILogger<TaskController> logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        [HttpGet("GetTask")]
        public async Task<ActionResult<IEnumerable<TaskEntity>>> GetTasks()
        {
            var tasks = await _taskService.GetTasks();
            return Ok(tasks);
        }

        // GET: api/task/supplier/{supplierId}
        [HttpGet("supplier/{supplierId}")]
        public async Task<ActionResult<IEnumerable<TaskEntity>>> GetTasksBySupplierId(int supplierId)
        {
            var tasks = await _taskService.GetTasksBySupplierId(supplierId);
            return Ok(tasks);
        }

        [HttpGet("vendor/{vendorId}")]
        public async Task<ActionResult<IEnumerable<TaskEntity>>> GetTasksByVendorId(int vendorId)
        {
            var tasks = await _taskService.GetTasksByVendorId(vendorId);
            return Ok(tasks);
        }

        [HttpPost("CreateTask")]
        public async Task<ActionResult<TaskEntity>> CreateTask([FromBody] TaskEntity task, int userId)
        {
            if (task == null)
            {
                return BadRequest("Task data is required.");
            }

            var createdTask = await _taskService.CreateTask(task, userId);
            return CreatedAtAction(nameof(GetTasks), new { id = createdTask.Id }, createdTask);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<TaskEntity>> UpdateTask(int id, [FromBody] TaskEntity task, int userId)
        {
            if (id != task.Id)
            {
                return BadRequest("Task ID mismatch.");
            }

            var updatedTask = await _taskService.UpdateTask(task, userId);
            return Ok(updatedTask);
        }

     
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTask(int id, int userId)
        {
            var result = await _taskService.DeleteTask(id, userId);
            if (!result)
            {
                return NotFound($"Task with ID {id} not found.");
            }

            return NoContent();
        }

        // POST: api/task/assign/supplier
        [HttpPost("assign/supplier")]
        public async Task<ActionResult<TaskEntity>> AssignTaskToSupplier([FromBody] TaskEntity task, int supplierId)
        {
            if (task == null)
            {
                return BadRequest("Task data is required.");
            }

            var assignedTask = await _taskService.AssignTaskToSupplier(task, supplierId);
            return Ok(assignedTask);
        }
    }
}
