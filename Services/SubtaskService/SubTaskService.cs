using EasyRepository.EFCore.Generic;
using Injazat.DataAccess.Models;
using Injazat.Presentation.Services.LogDBService;
using Injazat.Presentation.Services.SubtaskService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Task = Injazat.DataAccess.Models.Task;

public class SubTaskService : ISubTaskService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<SubTaskService> _logger;
    private readonly ILogDbService _logDbService;

    public SubTaskService(IUnitOfWork unitOfWork, UserManager<User> userManager, ILogger<SubTaskService> logger, ILogDbService logDbService)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _logger = logger;
        _logDbService = logDbService;
    }

    public async Task<IEnumerable<SubTask>> GetSubTasks(int taskId)
    {
        if (taskId <= 0)
        {
            throw new ArgumentException("Task ID is required and must be greater than zero.");
        }

        return await _unitOfWork.Repository.GetQueryable<SubTask>(st => st.TaskId == taskId).ToListAsync();
    }

    public async Task<IEnumerable<SubTask>> GetSubTasksAssignedTo(int userId)
    {
        if (userId <= 0)
        {
            throw new ArgumentException("User ID is required and must be greater than zero.");
        }

        var taskAssignments = _unitOfWork.Repository
            .GetQueryable<TaskAssignment>(ta => ta.AssignedToId == userId);

        var subTaskIds = taskAssignments.Select(ta => ta.SubTaskId).Distinct();

        return await _unitOfWork.Repository
            .GetQueryable<SubTask>(st => subTaskIds.Contains(st.Id))
            .ToListAsync();
    }

    public async Task<IEnumerable<SubTask>> GetSubTasksAssignedBy(int userId)
    {
        if (userId <= 0)
        {
            throw new ArgumentException("User ID is required and must be greater than zero.");
        }

        var taskAssignments = _unitOfWork.Repository
            .GetQueryable<TaskAssignment>(ta => ta.AssignedById == userId);

        var subTaskIds = taskAssignments.Select(ta => ta.SubTaskId).Distinct();

        return await _unitOfWork.Repository
            .GetQueryable<SubTask>(st => subTaskIds.Contains(st.Id))
            .ToListAsync();
    }

    public async Task<SubTask> CreateSubTask(SubTask subTask, int userId)
    {
        if (subTask == null) throw new ArgumentNullException(nameof(subTask));

        var task = await _unitOfWork.Repository.GetQueryable<Task>(t => t.Id == subTask.TaskId).FirstOrDefaultAsync();

        if (task == null)
        {
            throw new ArgumentException("Task not found");
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null || !(await _userManager.IsInRoleAsync(user, "Supplier") || await _userManager.IsInRoleAsync(user, "SupplierSupervisor")))
        {
            throw new UnauthorizedAccessException("User is not authorized to create subtasks");
        }
        
        subTask.CreationDate = DateTime.UtcNow;
        subTask.ModificationDate = DateTime.UtcNow;

        _unitOfWork.Repository.Add(subTask);
        await _unitOfWork.Repository.CompleteAsync();

        var logEvent = new LogEvent
        {
            Action = "SubTask Created",
            Description = $"SubTask {subTask.Id} created for task {task.Id} by user {userId}",
            UserId = userId
        };
        _logDbService.AddLogEvent(logEvent);

        _logger.LogInformation($"SubTask {subTask.Id} created for task {task.Id} by user {userId}");
        return subTask;
    }

    public async Task<SubTask> UpdateSubTask(SubTask subTask, int userId)
    {
        if (subTask == null)
        {
            throw new ArgumentNullException(nameof(subTask));
        }

        var user = await _userManager.FindByIdAsync(userId.ToString());

        if (user == null)
        {
            throw new ArgumentException("User not found");
        }

        subTask.ModificationDate = DateTime.UtcNow;

        _unitOfWork.Repository.Update(subTask);
        await _unitOfWork.Repository.CompleteAsync();
        _logger.LogInformation($"SubTask {subTask.Id} updated.");

        _logDbService.AddLogEvent(new LogEvent
        {
            Action = "SubTask Updated",
            Description = $"SubTask {subTask.Id} updated.",
            UserId = user.Id,
            UserName = user.UserName!,
        });

        return subTask;
    }

    public async Task<SubTask> DeleteSubTask(SubTask subTask)
    {
        if (subTask == null)
        {
            throw new ArgumentNullException(nameof(subTask));
        }

        subTask.ModificationDate = DateTime.UtcNow;
        subTask.DeletionDate = DateTime.UtcNow;
        subTask.IsDeleted = true;

        _unitOfWork.Repository.HardDelete(subTask);
        await _unitOfWork.Repository.CompleteAsync();
        _logger.LogInformation($"SubTask {subTask.Id} deleted.");
        return subTask;
    }

    public async Task<SubTask> AssignSubTask(SubTask subTask, int supervisorId, int employeeId)
    {
        if (subTask == null) throw new ArgumentNullException(nameof(subTask));

        var supervisor = await _userManager.FindByIdAsync(supervisorId.ToString());
        var employee = await _userManager.FindByIdAsync(employeeId.ToString());

        if (supervisor == null || !await _userManager.IsInRoleAsync(supervisor, "SupplierSupervisor"))
        {
            throw new ArgumentException("Supervisor not found or not authorized");
        }

        if (employee == null || !await _userManager.IsInRoleAsync(employee, "SupplierEmployee"))
        {
            throw new ArgumentException("Employee not found or not authorized");
        }

        var taskAssignment = new TaskAssignment
        {
            SubTaskId = subTask.Id,
            AssignedById = supervisorId,
            AssignedToId = employeeId
        };

        subTask.ModificationDate = DateTime.UtcNow;

        _unitOfWork.Repository.Add(taskAssignment);
        await _unitOfWork.Repository.CompleteAsync();

        var logEvent = new LogEvent
        {
            Action = "SubTask Assigned",
            Description = $"SubTask {subTask.Id} assigned to employee {employeeId} by supervisor {supervisorId}",
            UserId = supervisorId
        };
        _logDbService.AddLogEvent(logEvent);

        _logger.LogInformation($"SubTask {subTask.Id} assigned to employee {employeeId} by supervisor {supervisorId}");
        return subTask;
    }
}
