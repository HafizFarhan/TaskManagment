using System.ComponentModel.DataAnnotations;
using EasyRepository.EFCore.Abstractions;

namespace Injazat.DataAccess.Models
{
    public class LogException 
    {
        public int Id { get; set; }
        [Required]
        public string ExceptionMessage { get; set; } // The message of the exception

        [Required]
        public string StackTrace { get; set; } // The stack trace of the exception
        public virtual DateTime CreationDate { get; set; }

        public int? UserId { get; set; } // Optional: ID of the user if applicable
        public User User { get; set; } // Optional: Navigation property to the User
    }
}
