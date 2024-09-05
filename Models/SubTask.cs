using System.ComponentModel.DataAnnotations;
using EasyRepository.EFCore.Abstractions;

namespace Injazat.DataAccess.Models
{
    public class SubTask : EasyBaseEntity<int>, IEasyCreateDateEntity, IEasyUpdateDateEntity, IEasySoftDeleteEntity
    {
        [Required]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string Title { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Due date is required.")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Required]
        public int TaskId { get; set; }

        [Required]
        public Task Task { get; set; } // Reference to parent task
    }
}
