using InternMS.Api.Services.Dashboard;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InternMS.Api.Controllers.Dashboard
{
    [Authorize]
    [ApiController]
    [Route("api/dashboard")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard()
        {
            var userId = Guid.Parse(User.FindFirstValue("UserId"));
            var role = User.FindFirstValue(ClaimTypes.Role)!;

            var dashboard = await _dashboardService.GetDashboardAsync(userId, role);
            return Ok(dashboard);
        }
    }
}