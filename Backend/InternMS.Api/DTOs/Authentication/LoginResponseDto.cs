using InternMS.Api.DTOs.Users;

namespace InternMS.Api.DTOs.Authentication
{
    public class LoginResponseDto
    {
      public string AccessToken { get; set; } = string.Empty;
      public string RefreshToken { get; set; } = string.Empty;
      public UserDto User { get; set; } = default!;  
    }
}