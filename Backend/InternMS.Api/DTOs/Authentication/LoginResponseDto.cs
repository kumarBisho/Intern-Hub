using InternMS.Api.DTOs.Users;

namespace InternMS.Api.DTOs.Authentication
{
    public class LoginResponseDto
    {
      public string Token { get; set; } = string.Empty;
      public UserDto User { get; set; } = default!;  
    }
}