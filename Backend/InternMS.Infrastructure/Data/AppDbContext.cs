using Microsoft.EntityFrameworkCore;
using InternMS.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InternMS.Infrastructure.Configrations;

namespace InternMS.Infrastructure.Data
{
    // AppDbContext
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users => Set<User> ();
        public DbSet<BlacklistedToken> BlacklistedTokens => Set<BlacklistedToken>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<UserProfile> Profiles => Set<UserProfile>();
        public DbSet<Project> Projects => Set<Project>();
        public DbSet<ProjectTask> ProjectTasks => Set<ProjectTask>();
        public DbSet<ProjectAssignment> ProjectAssignments => Set<ProjectAssignment>();
        public DbSet<ProjectUpdate> ProjectUpdates => Set<ProjectUpdate>();
        public DbSet<Notification> Notifications => Set<Notification>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new ProfileConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectTaskConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectAssignmentConfiguration());
            modelBuilder.ApplyConfiguration(new ProjectUpdateConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationConfiguration());
        }
    }

}