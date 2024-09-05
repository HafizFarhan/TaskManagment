using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EasyRepository.EFCore.Abstractions;

namespace Injazat.DataAccess.Models
{
    public class TaskAssignment : EasyBaseEntity<int>, IEasyCreateDateEntity, IEasyUpdateDateEntity, IEasySoftDeleteEntity
    {
        [Required(ErrorMessage = "SubTask ID is required.")]
        public int SubTaskId { get; set; }

        [ForeignKey("SubTaskId")]
        public SubTask SubTask { get; set; } // Reference to the parent Sub Task

        public int? AssignedById { get; set; }

        [ForeignKey("AssignedById")]
        public User AssignedBy { get; set; } // Reference to the employee who assigned the task

        public int? AssignedToId { get; set; }

        [ForeignKey("AssignedToId")]
        public User AssignedTo { get; set; } // Reference to the employee assigned to this SubTask
    }
}
