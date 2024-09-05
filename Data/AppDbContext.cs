using Injazat.DataAccess.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Task = System.Threading.Tasks.Task;

namespace Injazat.DataAccess.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
    {
        public AppDbContext(DbContextOptions<AppDbContext> dbContextOptions) : base(dbContextOptions)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Bids entity relationships
            ConfigureBidsEntity(modelBuilder);

            // Call the base method
            base.OnModelCreating(modelBuilder);

            
        }
        public DbSet<Bid> Bids { get; set; }
        public DbSet<LogEvent> LogEvents { get; set; }
        public DbSet<LogException> LogExceptions { get; set; }
        public DbSet<SubTask> SubTasks { get; set; }
        public DbSet<Models.Task> Tasks { get; set; }
        public DbSet<TaskActivities> TaskActivities { get; set; }
        public DbSet<TaskAssignment> TaskAssignments { get; set; }

        private void ConfigureBidsEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bid>()
                .HasOne(b => b.Supplier) // Navigation property to Supplier
                .WithMany() // No navigation property on Supplier side
                .HasForeignKey(b => b.SupplierId) // Foreign key in Bids
                .OnDelete(DeleteBehavior.NoAction); // Prevent cascading deletes

            modelBuilder.Entity<Bid>()
                .HasOne(b => b.Task) // Navigation property to Task
                .WithMany() // No navigation property on Task side
                .HasForeignKey(b => b.TaskId) // Foreign key in Bids
                .OnDelete(DeleteBehavior.Cascade); // Enable cascading deletes
        }


        // Method to seed roles
        public static async Task SeedRolesAsync(RoleManager<IdentityRole<int>> roleManager)
        {
            foreach (var role in Enum.GetValues(typeof(eRole)).Cast<eRole>())
            {
                var roleName = role.ToString();

                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(roleName));
                }
            }
        }
    }
}
