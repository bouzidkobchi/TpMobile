﻿using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MobileAppTp
{
    public class UserService
    {
        private readonly IConfiguration configuration;

        public UserService(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public string GenerateJsonWebToken(AppUser user) // supposing the user is already stored
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(configuration["JWT:key"]);
            var tokenDiscriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id)
                }),
                Expires = DateTime.UtcNow.AddDays(5),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256),
            };
            var token = tokenHandler.CreateJwtSecurityToken(tokenDiscriptor);
            var ruserToken = tokenHandler.WriteToken(token);
            return ruserToken;
        }
    }
}
