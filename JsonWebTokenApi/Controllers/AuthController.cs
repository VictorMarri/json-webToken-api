using JsonWebTokenApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace JsonWebTokenApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public static UsuarioModel usuario = new UsuarioModel();
        private readonly IConfiguration _configuration;
        public IUserService _userService { get; }
        public AuthController(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
        }

        [HttpGet, Authorize]
        public ActionResult<string> GetMe()
        {
            var userName = _userService.GetMyName();

            return Ok(userName);

            //var userName = User?.Identity?.Name;
            //var userName2 = User.FindFirstValue(ClaimTypes.Name);
            //var role = User.FindFirstValue(ClaimTypes.Role);
            //return Ok(new {userName, userName2, role});
        }

        [HttpPost("register")]
        public async Task<ActionResult<UsuarioModel>> Register(UsuarioDTO request)
        {
            CreatePasswordHash(request.Senha, out byte[] passwordHash, out byte[] passwordSalt);

            usuario.Username = request.NomeUsuario;
            usuario.PasswordHash = passwordHash;
            usuario.PasswordSalt = passwordSalt;

            return Ok(usuario);
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UsuarioDTO request)
        {
            if (usuario.Username != request.NomeUsuario) return BadRequest("Usuário não encontrado");

            if (!VerifyPasswordHash(request.Senha, usuario.PasswordHash, usuario.PasswordSalt)) return BadRequest("Senha errada");

            string token = CreateToken(usuario);

            return Ok(token);
        }

        private string CreateToken(UsuarioModel usuario)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Username),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("AppSettings:Token").Value));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials
                 );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using(var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }

    }
}
