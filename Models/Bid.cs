using System.ComponentModel.DataAnnotations;
using EasyRepository.EFCore.Abstractions;

namespace Injazat.DataAccess.Models
{
    public class Bid : EasyBaseEntity<int>, IEasyCreateDateEntity, IEasyUpdateDateEntity, IEasySoftDeleteEntity
    {
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime SubmittedAt { get; set; }

        [Required]
        public int TaskId { get; set; }

        [Required]
        public Task Task { get; set; } // Reference to the task being bid on

        [Required]
        public int SupplierId { get; set; }

        [Required]
        public User Supplier { get; set; } // Reference to the User (supplier) who submitted the bid

        public bool IsApproved { get; set; } = false; // Flag to indicate if the bid has been approved
    }
}
