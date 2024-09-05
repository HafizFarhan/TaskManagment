using EasyRepository.EFCore.Generic;
using Injazat.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Injazat.Presentation.Services.TaskAssignmentService
{
    public class TaskAssignmentService : ITaskAssignmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TaskAssignmentService> _logger;

        public TaskAssignmentService(IUnitOfWork unitOfWork, ILogger<TaskAssignmentService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async System.Threading.Tasks.Task AssignSubTaskToUser(int subTaskId, int userId, int assignedById)
        {
            var taskAssignment = new TaskAssignment
            {
                SubTaskId = subTaskId,
                AssignedToId = userId,
                AssignedById = assignedById
            };

            taskAssignment.CreationDate = DateTime.UtcNow;
            taskAssignment.ModificationDate = DateTime.UtcNow;

            _unitOfWork.Repository.Add(taskAssignment);
            await _unitOfWork.Repository.CompleteAsync();
            _logger.LogInformation($"SubTask {subTaskId} assigned to user {userId} by {assignedById}");
        }

        public async Task<IEnumerable<TaskAssignment>> GetAssignmentsForSubTask(int subTaskId)
        {
            return await _unitOfWork.Repository.GetQueryable<TaskAssignment>(ta => ta.SubTaskId == subTaskId).ToListAsync();
        }
    }
}