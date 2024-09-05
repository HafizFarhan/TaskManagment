using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Injazat.DataAccess.Models
{
    public class User : IdentityUser<int>
    {
        public virtual DateTime CreationDate { get; set; }
        public virtual DateTime? ModificationDate { get; set; }

        public virtual DateTime? DeletionDate { get; set; }

        public virtual bool IsDeleted { get; set; }
    }
}
