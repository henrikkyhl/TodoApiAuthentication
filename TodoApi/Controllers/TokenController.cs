using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using TodoApi.Data;
using TodoApi.Models;
using TodoApi.Helpers;
using System.Linq;
using System.Collections.Generic;

namespace TodoApi.Controllers
{
    [Route("/token")]
    public class TokenController : Controller
    {
        private readonly IRepository<User> repository;

        public TokenController(IRepository<User> repos)
        {
            repository = repos;
        }


        [HttpPost]
        public IActionResult Login([FromBody]LoginInputModel model)
        {
            var user = repository.GetAll().FirstOrDefault(u => u.Username == model.Username);

            // check if username exists
            if (user == null)
                return Unauthorized();

            // check if password is correct
            if (!model.Password.Equals(user.Password))
                return Unauthorized();

            // Authentication successful
            return Ok(new {
                username = user.Username,
                token = GenerateToken(user)
            });
        }

        // This method generates and returns a JWT token for a user.
        private string GenerateToken(User user)
        {            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username)
            };

            if (user.IsAdmin)
                claims.Add(new Claim(ClaimTypes.Role, "Administrator"));

            var token = new JwtSecurityToken(
                new JwtHeader(new SigningCredentials(
                    JwtSecurityKey.Key, 
                    SecurityAlgorithms.HmacSha256)),
                new JwtPayload(null, // issuer - not needed (ValidateIssuer = false)
                               null, // audience - not needed (ValidateAudience = false)
                               claims.ToArray(), 
                               DateTime.Now,               // notBefore
                               DateTime.Now.AddDays(1)));  // expires

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
