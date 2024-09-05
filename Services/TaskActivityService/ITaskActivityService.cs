using Injazat.DataAccess.Models;

namespace Injazat.Presentation.Services.TaskActivityService
{
    public interface ITaskActivityService
    {
        System.Threading.Tasks.Task LogActivity(TaskActivities activity);
        Task<IEnumerable<TaskActivities>> GetActivitiesByTaskId(int taskId);
        Task<IEnumerable<TaskActivities>> GetActivitiesBySubTaskId(int subTaskId);
        System.Threading.Tasks.Task AddAttachmentToActivity(int activityId, string fileName, byte[] fileData, string attachmentUrl);
    }

}
