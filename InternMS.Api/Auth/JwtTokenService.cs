using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InternMS.Api.Auth
{
    public class JwtTokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly string _key;
        private readonly int _expiryMinutes;

        public JwtTokenService(IConfiguration config)
        {
            _config = config;
            _issuer = _config["Jwt:Issuer"] ?? "internms";
            _audience = _config["Jwt:Audience"] ?? "internms-users";
            _key = _config["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key configuration is missing");
            int.TryParse(_config["Jwt:ExpiryMinutes"], out _expiryMinutes);
            if (_expiryMinutes <= 0) _expiryMinutes = 60 * 24;
        }

        public string CreateToken(Guid userId, string email, IEnumerable<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim("id", userId.ToString())
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_expiryMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}