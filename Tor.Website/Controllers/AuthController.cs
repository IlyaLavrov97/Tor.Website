using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Tor.Website.EF;
using Tor.Website.Models.DBO;
using Tor.Website.Models.Request;
using Tor.Website.Models.Response;

namespace TorWebApi.Controllers
{
    [Produces("application/json")]
    [Route("api/login")]
    [AllowAnonymous]
    public class AuthController : Controller
    {
        private readonly DataContext _context;
        private IConfiguration _config;

        public AuthController(DataContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // POST: api/login
        [HttpPost]
        public async Task<IActionResult> Login([FromBody]AuthRequest auth)
        {
            IActionResult response = Unauthorized();
            User user = await _context.Users.FirstOrDefaultAsync(u => u.Login == auth.Login && u.Password == auth.Password);

            if (user != null)
            {
                var tokenString = GenerateJSONWebToken(user);
                response = Ok(new AuthResponse { Token = tokenString });
            }

            return response;
        }

        private string GenerateJSONWebToken(User userInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[] {
                 new Claim(JwtRegisteredClaimNames.Sub, userInfo.Login),
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                 };

            var token = new JwtSecurityToken(_config["Jwt:Issuer"],
            _config["Jwt:Issuer"],
            claims,
            expires: DateTime.Now.AddMinutes(120),
            signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
