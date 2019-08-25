using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace DatingApp.API.Controllers
{
    // POST HTTP//localhost:5000/api/values/5
    [Route("api/[controller]")]
    [ApiController]

    public class AuthController : ControllerBase
    {
        private readonly IAuthRepo _repo;
        private readonly IConfiguration _config;
        public AuthController(IAuthRepo repo, IConfiguration config)
        {
            _config = config;
            _repo = repo;

        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserToRegisterDto userToRegisterDto)
        { 

            //validate username 
            if (!ModelState.IsValid)
                userToRegisterDto.username = userToRegisterDto.username.ToLower();

            if (await _repo.UserExists(userToRegisterDto.username))
                return BadRequest("Username already exists!");

            var userToCreate = new User
            {
                Username = userToRegisterDto.username
            };

            var createdUser = await _repo.Register(userToCreate, userToRegisterDto.password);
            return StatusCode(201);

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserToLoginDto userToLoginDto)
        {
            

            var repoUser = await _repo.Login(userToLoginDto.username.ToLower(), userToLoginDto.password);

            if (repoUser == null)
            {
                return Unauthorized();
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, repoUser.Id.ToString()),
                new Claim(ClaimTypes.Name, repoUser.Username)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDesctriptor = new SecurityTokenDescriptor  
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDesctriptor);

            return Ok(new {
                token = tokenHandler.WriteToken(token)
            });
        }

    }
}