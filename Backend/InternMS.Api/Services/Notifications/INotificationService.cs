using InternMS.Api.DTOs;
using InternMS.Domain.Entities;

namespace InternMS.Api.Services
{
    public interface INotificationService
    {
        Task<IEnumerable<Notification>> GetUserNotificationsAsync(Guid userId);
        Task<Notification> CreateNotificationAsync(Guid userId, string title, string message);
        Task MarkAsReadAsync(int notificationId, Guid userId);
        Task MarkAllAsReadAsync(Guid userId);
    }
}