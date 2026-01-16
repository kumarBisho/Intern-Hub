using InternMS.Infrastructure.Data;
using InternMS.Domain.Entities;
using InternMS.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace InternMS.Api.Services
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _db;
        public NotificationService(AppDbContext db)
        {
            _db = db;
        }
        public async Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId)
        {
            return await _db.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<Notification> CreateNotificationAsync(Guid userId, string title, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };
            _db.Notifications.Add(notification);
            await _db.SaveChangesAsync();

            return notification;
        }

        public async Task MarkAsReadAsync(int notificationId, Guid userId)
        {
            var notification = await _db.Notifications
                .FirstOrDefaultAsync(n => n.Id == notificationId && n.UserId == userId);

            if (notification == null)
                throw new Exception("Notification not found");

            notification.IsRead = true;
            await _db.SaveChangesAsync();
        }

        public async Task MarkAllAsReadAsync(Guid userId)
        {
            var notifications = await _db.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var n in notifications)
                n.IsRead = true;

            await _db.SaveChangesAsync();
        }

        public async Task ClearReadNotificationsAsync(Guid userId)
        {
            var readNotifications = await _db.Notifications
                .Where(n => n.UserId == userId && n.IsRead)
                .ToListAsync();

            _db.Notifications.RemoveRange(readNotifications);
            await _db.SaveChangesAsync();
        }
    }
}