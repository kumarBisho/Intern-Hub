using Microsoft.EntityFrameworkCore;
using InternMS.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InternMS.Infrastructure.Configrations
{
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