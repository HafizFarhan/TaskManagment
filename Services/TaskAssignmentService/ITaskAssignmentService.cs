using Injazat.DataAccess.Models;

namespace Injazat.Presentation.Services.TaskAssignmentService
{
    public interface ITaskAssignmentService
    {
        System.Threading.Tasks.Task AssignSubTaskToUser(int subTaskId, int userId, int assignedById);
        Task<IEnumerable<TaskAssignment>> GetAssignmentsForSubTask(int subTaskId);
    }
}
