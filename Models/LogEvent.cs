using System.ComponentModel.DataAnnotations;

namespace Injazat.DataAccess.Models
{
    public class LogEvent 
    {
        [Required]
        public int Id { get; set; } // ID of the log event

        [Required]
        public DateTime CreationDate { get; set; } = DateTime.UtcNow; // Date and time of the log event creation

        [Required]
        [StringLength(50, ErrorMessage = "Action cannot exceed 50 characters.")]
        public string Action { get; set; } // e.g., "Task Created", "Bid Submitted", "Task Approved", "Task Assigned"

        [Required]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; } // Detailed description of the action

        [Required]
        public int UserId { get; set; } // ID of the user who performed the action

        [Required]
        public User User { get; set; } // Navigation property to the User

        [Required]
        [StringLength(30, ErrorMessage = "User name cannot exceed 30 characters.")]
        public string UserName { get; set; } // Name of the user
    }
}
