using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UdemyDotNetProjectNew.Data;
using UdemyDotNetProjectNew.DTOs;
using UdemyDotNetProjectNew.Entities;
using UdemyDotNetProjectNew.Interfaces;

namespace UdemyDotNetProjectNew.Controllers
{
    public class AccountController : BaseAPIController
    {

        private readonly DataContext _context;
        private readonly ITokenService tokenService;

        public AccountController(DataContext context, ITokenService tokenService)
        {
            _context = context;
            this.tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserDTO>> RegisterUser(RegisterDTO registerDTO)
        {

            if(await UserExists(registerDTO.Username)) {
                return BadRequest("Username is taken");
            }

            using var hmac = new HMACSHA512();

            var byteArrayPassword = Encoding.UTF8.GetBytes(registerDTO.Password);
            var passwordSalt = hmac.Key;

            var user = new AppUser
            {
                Username = registerDTO.Username.ToLower(),
                PasswordHash = hmac.ComputeHash(byteArrayPassword),
                PasswordSalt = passwordSalt
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new UserDTO
            {
                Username = user.Username,
                Token = tokenService.CreateToken(user)
            };
        }

        private async Task<bool> UserExists(string usernameToCheck)
        {
            return await _context.Users.AnyAsync(username => username.Username == usernameToCheck.ToLower());
        }

        [HttpPost("login")]
        public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO)
        {
            var user = await _context.Users.SingleOrDefaultAsync(username => username.Username == loginDTO.Username);

            if(user == null) return Unauthorized("Invalid Username");

            using var hmac = new HMACSHA512(user.PasswordSalt);

            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

            for(int i = 0; i < computedHash.Length; i++)
            {
                if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
            }

            return new UserDTO {
                Username = user.Username,
                Token = tokenService.CreateToken(user)
            };
        }
    }
}
