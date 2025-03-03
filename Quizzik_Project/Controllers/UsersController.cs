using AutoMapper;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Quizzik_Project.DTO;
using Quizzik_Project.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using BCrypt.Net;

namespace Quizzik_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly EFCoreDbContext _context;
        private readonly IConfiguration _configuration;
        //changes

        public UsersController(IMapper mapper, EFCoreDbContext context, IConfiguration configuration)
        {
            _mapper = mapper;
            _configuration = configuration;
            _context = context;
        }


        // GET: api/Users



        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAll()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                var userDTOs = _mapper.Map<List<UserDTO>>(users);
                return Ok(userDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UserDTO>> GetById(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null) 
                {
                    return NotFound();
                }
                var userDTO = _mapper.Map<UserDTO>(user);
                return Ok(userDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("Register")]
        public async Task<ActionResult<UserDTO>> Register(UserDTO userDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var user = _mapper.Map<User>(userDTO);
                user.Password = BCrypt.Net.BCrypt.HashPassword(userDTO.Password);
                user.CreatedAt = DateTime.Now;
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                userDTO.UserID = user.UserID;
                return CreatedAtAction(nameof(GetById), new { id = user.UserID }, userDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login(LoginDTO loginDTO)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Email == loginDTO.Email);
                if (user == null || !BCrypt.Net.BCrypt.Verify(loginDTO.Password, user.Password))
                {
                    return Unauthorized();
                }

                var token = GenerateJwtToken(user);
                return Ok(new { Token = token });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UserDTO userDTO)
        {
            try
            {
                if (id != userDTO.UserID)
                {
                    return BadRequest();
                }

                var user = _mapper.Map<User>(userDTO);
                user.Password = BCrypt.Net.BCrypt.HashPassword(userDTO.Password);
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        private string GenerateJwtToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
