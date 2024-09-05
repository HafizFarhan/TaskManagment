using Injazat.DataAccess.Models;
using EasyRepository.EFCore.Generic;
using Task = System.Threading.Tasks.Task;
using Microsoft.EntityFrameworkCore;
using Injazat.Presentation.Services.LogDBService;

namespace Injazat.Presentation.Services.TaskActivityService
{
    public class TaskActivityService : ITaskActivityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<TaskActivityService> _logger;
        private readonly ILogDbService _logDbService;
        public TaskActivityService(IUnitOfWork unitOfWork, ILogger<TaskActivityService> logger, ILogDbService logDbService)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _logDbService = logDbService;
        }
        public async Task LogActivity(TaskActivities activity)
        {
            if (activity == null) throw new ArgumentNullException(nameof(activity));

            activity.CreationDate = DateTime.UtcNow;
            activity.ModificationDate = DateTime.UtcNow;

            _unitOfWork.Repository.Add(activity);
            await _unitOfWork.Repository.CompleteAsync();

            var logEvent = new LogEvent
            {
                Action = "Log Activity",
                Description = $"Activity logged for {(activity.TaskId.HasValue ? "task " + activity.TaskId : "subtask " + activity.SubTaskId)} by user {activity.UserId}",
                UserId = activity.UserId
            };
            _logDbService.AddLogEvent(logEvent);

            _logger.LogInformation($"Activity logged for {(activity.TaskId.HasValue ? "task " + activity.TaskId : "subtask " + activity.SubTaskId)} by user {activity.UserId}");
        }

        public async Task<IEnumerable<TaskActivities>> GetActivitiesByTaskId(int taskId)
        {
            return await _unitOfWork.Repository.GetQueryable<TaskActivities>(a => a.TaskId == taskId).ToListAsync();
        }

        public async Task<IEnumerable<TaskActivities>> GetActivitiesBySubTaskId(int subTaskId)
        {
            return await _unitOfWork.Repository.GetQueryable<TaskActivities>(a => a.SubTaskId == subTaskId).ToListAsync();
        }

        public async Task AddAttachmentToActivity(int activityId, string fileName, byte[] fileData, string attachmentUrl)
        {
            var activity = await _unitOfWork.Repository.GetQueryable<TaskActivities>(ta => ta.Id == activityId).FirstOrDefaultAsync();
            if (activity == null)
            {
                throw new ArgumentException("Activity not found.");
            }

            activity.FileName = fileName;
            activity.FileData = fileData;
            activity.AttachmentUrl = attachmentUrl;
            activity.ModificationDate = DateTime.UtcNow;

            _unitOfWork.Repository.Update(activity);
            await _unitOfWork.Repository.CompleteAsync();

            var logEvent = new LogEvent
            {
                Action = "Add Attachment",
                Description = $"Attachment added to activity {activityId}, file name: {fileName}",
                UserId = activity.UserId  // Assuming the User ID is accessible here; you might need to pass it explicitly
            };
            _logDbService.AddLogEvent(logEvent);

            _logger.LogInformation($"Attachment added to activity {activityId}, file name: {fileName}");
        }

        public async Task<TaskActivities> DeleteActivity(int activityId)
        {
            var activity = await _unitOfWork.Repository.GetQueryable<TaskActivities>(a => a.Id == activityId).FirstOrDefaultAsync();
            if (activity == null)
            {
                throw new ArgumentException("Activity not found.");
            }

            activity.ModificationDate = DateTime.UtcNow;
            activity.IsDeleted = true;
            activity.DeletionDate = DateTime.UtcNow;

            _unitOfWork.Repository.Update(activity);
            await _unitOfWork.Repository.CompleteAsync();

            var logEvent = new LogEvent
            {
                Action = "Delete Activity",
                Description = $"Activity {activityId} deleted by user {activity.UserId}",
                UserId = activity.UserId
            };
            _logDbService.AddLogEvent(logEvent);

            _logger.LogInformation($"Activity {activityId} deleted by user {activity.UserId}");
            return activity;
        }
    }
}