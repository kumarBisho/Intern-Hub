using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace InternMS.Api.Services.Token
{
    public interface ITokenService
    {
        string CreateToken(Guid userId, string email, IEnumerable<string> roles);
    }
}

