using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using EasyRepository.EFCore.Abstractions;

namespace Injazat.DataAccess.Models
{
    public class TaskActivities : EasyBaseEntity<int>, IEasyCreateDateEntity, IEasyUpdateDateEntity, IEasySoftDeleteEntity
    {
        public int? TaskId { get; set; }

        [ForeignKey("TaskId")]
        public Task Task { get; set; } // Reference to the parent task

        public int? SubTaskId { get; set; }

        [ForeignKey("SubTaskId")]
        public Task SubTask { get; set; } // Reference to the parent SubTask

        [StringLength(1000, ErrorMessage = "Details cannot exceed 1000 characters.")]
        public string Details { get; set; } // For comments or status descriptions

        [Required]
        public eActivityType Type { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; }

        [Required]
        public User User { get; set; }

        public bool IsExternal { get; set; } = false; // Flag to indicate if the activity is external or internal

        // Fields specifically for attachments
        [Url(ErrorMessage = "Invalid URL format.")]
        public string? AttachmentUrl { get; set; } // For URLs if the type is Attachment

        [StringLength(255, ErrorMessage = "File name cannot exceed 255 characters.")]
        public string? FileName { get; set; } // If it's a file, store the filename here

        public byte[]? FileData { get; set; } // Store file data as binary for attachments
    }
}
