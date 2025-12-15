using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using InternMS.Api.Services;
using AutoMapper;
using InternMS.Api.DTOs;
using System.Security.Claims;

namespace InternMS.Api.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;
        public NotificationController(INotificationService notificationService, IMapper mapper)
        {
            _notificationService = notificationService;
            _mapper = mapper;
        }

        private Guid GetUserId() =>
            Guid.Parse(User.FindFirstValue("id"));

        [HttpGet]
        public async Task<IActionResult> GetMyNotifications()
        {
            var userId = GetUserId();
            var notifications = await _notificationService.GetUserNotificationsAsync(userId);
            return Ok(_mapper.Map<IEnumerable<NotificationDto>>(notifications));
        }

        [HttpPost("{id}/mark-read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var userId = GetUserId();
            await _notificationService.MarkAsReadAsync(id, userId);
            return Ok(new { message = "Notification marked as read" });
        }

        [HttpPost("mark-all-read")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = GetUserId();
            await _notificationService.MarkAllAsReadAsync(userId);
            return Ok(new { message = "All notifications marked as read" });
        }
    }
}