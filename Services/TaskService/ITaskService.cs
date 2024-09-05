using Task = Injazat.DataAccess.Models.Task;
namespace Injazat.Presentation.Services.TaskService
{
    public interface ITaskService
    {
        Task<IEnumerable<Task>> GetTasks();
        Task<IEnumerable<Task>> GetTasksBySupplierId(int supplierId);
        Task<IEnumerable<Task>> GetTasksByVendorId(int vendorId);
        Task<Task> CreateTask(Task task, int userId);
        Task<Task> UpdateTask(Task task, int userId);
        Task<bool> DeleteTask(int taskId, int userId);
        Task<Task> AssignTaskToSupplier(Task task, int supplierId);
    }
}
