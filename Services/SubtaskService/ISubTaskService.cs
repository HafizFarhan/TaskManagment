using Injazat.DataAccess.Models;

namespace Injazat.Presentation.Services.SubtaskService
{
    public interface ISubTaskService
    {
        Task<IEnumerable<SubTask>> GetSubTasks(int taskId);
        Task<IEnumerable<SubTask>> GetSubTasksAssignedTo(int userId);
        Task<IEnumerable<SubTask>> GetSubTasksAssignedBy(int userId);
        Task<SubTask> CreateSubTask(SubTask subTask, int userId);
        Task<SubTask> UpdateSubTask(SubTask subTask, int userId);
        Task<SubTask> DeleteSubTask(SubTask subTask);
        Task<SubTask> AssignSubTask(SubTask subTask, int supervisorId, int employeeId);
    }
}
