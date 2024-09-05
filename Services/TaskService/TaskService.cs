using EasyRepository.EFCore.Generic;
using Injazat.DataAccess.Models;
using Injazat.Presentation.Services.LogDBService;
using Injazat.Presentation.Services.TaskService;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Task = Injazat.DataAccess.Models.Task;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

public class TaskService : ITaskService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<TaskService> _logger;
    private readonly ILogDbService _logDbService;

    public TaskService(
        IUnitOfWork unitOfWork,
        UserManager<User> userManager,
        ILogger<TaskService> logger,
        ILogDbService logDbService)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _logger = logger;
        _logDbService = logDbService;
    }

    public async Task<IEnumerable<Task>> GetTasks()
    {
        return await _unitOfWork.Repository.GetQueryable<Task>().ToListAsync();
    }

    public async Task<IEnumerable<Task>> GetTasksBySupplierId(int supplierId)
    {
        return await _unitOfWork.Repository.GetQueryable<Task>(t => t.SupplierId == supplierId).ToListAsync();
    }

    public async Task<IEnumerable<Task>> GetTasksByVendorId(int vendorId)
    {
        return await _unitOfWork.Repository.GetQueryable<Task>(t => t.VendorId == vendorId).ToListAsync();
    }

    public async Task<Task> CreateTask(Task task, int userId)
    {
        task.CreationDate = DateTime.UtcNow;
        task.ModificationDate = DateTime.UtcNow;

        _unitOfWork.Repository.Add(task);
        await _unitOfWork.Repository.CompleteAsync();

        var logEvent = new LogEvent
        {
            Action = "Task Created",
            Description = $"Task {task.Id} created by user {userId}",
            UserId = userId
        };
        _logDbService.AddLogEvent(logEvent);

        _logger.LogInformation($"Task {task.Id} created by user {userId}");
        return task;
    }

    public async Task<Task> UpdateTask(Task task, int userId)
    {
        task.ModificationDate = DateTime.UtcNow;

        _unitOfWork.Repository.Update(task);
        await _unitOfWork.Repository.CompleteAsync();

        var logEvent = new LogEvent
        {
            Action = "Task Updated",
            Description = $"Task {task.Id} updated by user {userId}",
            UserId = userId
        };
        _logDbService.AddLogEvent(logEvent);

        _logger.LogInformation($"Task {task.Id} updated by user {userId}");
        return task;
    }

    public async Task<bool> DeleteTask(int taskId, int userId)
    {
        var task = await _unitOfWork.Repository.GetQueryable<Task>(t => t.Id == taskId).FirstOrDefaultAsync();

        if (task == null)
        {
            return false;
        }

        task.ModificationDate = DateTime.UtcNow;
        task.IsDeleted = true;
        task.DeletionDate = DateTime.UtcNow;

        _unitOfWork.Repository.HardDelete(task);
        await _unitOfWork.Repository.CompleteAsync();

        var logEvent = new LogEvent
        {
            Action = "Task Deleted",
            Description = $"Task {taskId} deleted by user {userId}",
            UserId = userId
        };
        _logDbService.AddLogEvent(logEvent);

        _logger.LogInformation($"Task {taskId} deleted by user {userId}");
        return true;
    }

    public async Task<Task> AssignTaskToSupplier(Task task, int supplierId)
    {
        if (task == null) throw new ArgumentNullException(nameof(task));

        var supplier = await _userManager.FindByIdAsync(supplierId.ToString());

        if (supplier == null)
        {
            throw new ArgumentException("Supplier not found");
        }

        if (!await _userManager.IsInRoleAsync(supplier, "Supplier"))
        {
            throw new ArgumentException("User is not a supplier");
        }

        // Assign the task to the Supplier
        task.SupplierId = supplierId;
        task.Supplier = supplier;
        task.OpenForBid = false; // Close the task for bidding
        task.ModificationDate = DateTime.UtcNow;

        _unitOfWork.Repository.Update(task);
        await _unitOfWork.Repository.CompleteAsync();
        _logger.LogInformation($"Task {task.Id} assigned to supplier {supplierId}");
        return task;
    }

    private void EnsureUserIsAuthorizedForTask(Task task, int userId)
    {
        if (task.VendorId != userId)
        {
            throw new UnauthorizedAccessException("You are not authorized to perform this operation on this task");
        }
    }
}
