using Microsoft.EntityFrameworkCore;
using InternMS.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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

    // UserRoleConfigration

    public class RoleConfiguration : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("roles");
            builder.HasKey(r => r.Id);
            builder.Property(r => r.Name).IsRequired().HasMaxLength(100);

            builder.HasData(
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "Mentor" },
                new Role { Id = 3, Name = "Intern" }
            );
        }

    }

    // UserCongigration

    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("users");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(255);
            builder.HasIndex(u => u.Email).IsUnique();
            builder.Property(u => u.PasswordHash).IsRequired();
            builder.Property(u => u.FirstName).HasMaxLength(100);
            builder.Property(u => u.LastName).HasMaxLength(100);
            builder.Property(u => u.IsActive).HasDefaultValue(true);
            builder.Property(u => u.CreatedAt).HasDefaultValueSql("now()");

            // Relationships 
            builder.HasMany(u => u.UserRoles).WithOne(ur => ur.User).HasForeignKey(ur => ur.UserId);
            builder.HasOne(u => u.Profile).WithOne(p => p.User).HasForeignKey<UserProfile>(p => p.UserId);
            builder.HasMany(u => u.CreateProjects).WithOne(p => p.CreatedBy).HasForeignKey(p => p.CreatedById);
        }
    }

    // UserRoleConfiguration

    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {
            builder.ToTable("user_roles");
            builder.HasKey(ur => new { ur.UserId, ur.RoleId });

            // Relationships
            builder.HasOne(ur => ur.User).WithMany(u => u.UserRoles).HasForeignKey(ur => ur.UserId);
            builder.HasOne(ur => ur.Role).WithMany(r => r.UserRoles).HasForeignKey(ur => ur.RoleId);
        }
    }

    // ProfileConfiguration

    public class ProfileConfiguration : IEntityTypeConfiguration<UserProfile>
    {
        public void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            builder.ToTable("profiles");
            builder.HasKey(p => p.UserId);

            builder.Property(p => p.Phone).HasMaxLength(50);
            builder.Property(p => p.Department).HasMaxLength(100);
            builder.Property(p => p.Position).HasMaxLength(100);
            builder.Property(p => p.Bio).HasMaxLength(1000);
        }
    }

    // ProjectConfiguration

    public class ProjectConfiguration : IEntityTypeConfiguration<Project>
    {
        public void Configure(EntityTypeBuilder<Project> builder)
        {
            builder.ToTable("projects");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Title).IsRequired().HasMaxLength(255);
            builder.Property(p => p.Description).HasMaxLength(4000);
            builder.Property(p => p.Status).HasConversion<string>().IsRequired();
            builder.Property(p => p.CreatedAt).HasDefaultValueSql("now()");

            // Relationships
            builder.HasMany(p => p.Assignments).WithOne(a => a.Project).HasForeignKey(a => a.ProjectId);
            builder.HasMany(p => p.Updates).WithOne(u => u.Project).HasForeignKey(u => u.ProjectId);

            builder.HasOne(p => p.CreatedBy).WithMany(u => u.CreateProjects).HasForeignKey(p => p.CreatedById).OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class ProjectTaskConfiguration : IEntityTypeConfiguration<ProjectTask>
    {
        public void Configure(EntityTypeBuilder<ProjectTask> builder)
        {
            builder.ToTable("project_tasks");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Title).IsRequired().HasMaxLength(255);
            builder.Property(p => p.Status).HasConversion<string>().IsRequired();
            builder.Property(p => p.Priority).HasConversion<string>().IsRequired();
            builder.HasOne(t => t.Project)
              .WithMany(p => p.Tasks)
              .HasForeignKey(t => t.ProjectId)
              .OnDelete(DeleteBehavior.Cascade);
        }
    }

    // ProjectAssignmentConfiguration

    public class ProjectAssignmentConfiguration : IEntityTypeConfiguration<ProjectAssignment>
    {
        public void Configure(EntityTypeBuilder<ProjectAssignment> builder)
        {
            builder.ToTable("project_assignments");
            builder.HasKey(pa => pa.Id);

            builder.Property(pa => pa.AssignedAt).HasDefaultValueSql("now()");

            builder.HasOne(pa => pa.Project).WithMany(p => p.Assignments).HasForeignKey(pa => pa.ProjectId);

            builder.HasOne(pa => pa.Intern).WithMany().HasForeignKey(pa => pa.InternId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(pa => pa.Mentor).WithMany().HasForeignKey(pa => pa.MentorId).OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(pa => new { pa.ProjectId, pa.InternId }).IsUnique();
        }
    }

    // ProjectUpdateConfiguration


    public class ProjectUpdateConfiguration : IEntityTypeConfiguration<ProjectUpdate>
    {
        public void Configure(EntityTypeBuilder<ProjectUpdate> builder)
        {
            builder.ToTable("project_updates");
            builder.HasKey(pu => pu.Id);

            builder.Property(pu => pu.Comment).HasMaxLength(4000);
            builder.Property(pu => pu.CreatedAt).HasDefaultValueSql("now()");
            builder.Property(pu => pu.Status).HasConversion<string>().IsRequired();

            builder.HasOne(pu => pu.Project).WithMany(p => p.Updates).HasForeignKey(pu => pu.ProjectId);
            builder.HasOne(pu => pu.Author).WithMany().HasForeignKey(pu => pu.AuthorId).OnDelete(DeleteBehavior.Restrict);
        }
    }

    // NotificationConfiguration
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("notifications");
            builder.HasKey(n => n.Id);

            builder.Property(n => n.Title).IsRequired().HasMaxLength(255);
            builder.Property(n => n.Message).IsRequired().HasMaxLength(2000);
            builder.Property(n => n.IsRead).HasDefaultValue(false);
            builder.Property(n => n.CreatedAt).HasDefaultValueSql("now()");

            builder.HasOne(n => n.User).WithMany().HasForeignKey(n => n.UserId).OnDelete(DeleteBehavior.Cascade);
        }
    }

}