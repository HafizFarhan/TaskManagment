using System.ComponentModel.DataAnnotations;
using EasyRepository.EFCore.Abstractions;

namespace Injazat.DataAccess.Models
{
    public class Task : EasyBaseEntity<int>, IEasyCreateDateEntity, IEasyUpdateDateEntity, IEasySoftDeleteEntity
    {
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string Title { get; set; }

        public bool OpenForBid { get; set; } // Flag to indicate if the task is open for bidding

        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Due date is required.")]
        public DateTime DueDate { get; set; }

        [Required(ErrorMessage = "Vendor ID is required.")]
        public int VendorId { get; set; }

        [Required]
        public User Vendor { get; set; } // Reference to the vendor who created the task

        public ICollection<SubTask> Subtasks { get; set; } = new List<SubTask>(); // List of subtasks

        public ICollection<Bid> Bids { get; set; } = new List<Bid>(); // Bids received for the task

        public int? SupplierId { get; set; } // ID of the supplier who was awarded the task

        public User Supplier { get; set; } // Reference to the supplier who submitted the bid
    }
}
