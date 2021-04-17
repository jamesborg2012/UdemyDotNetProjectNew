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

namespace UdemyDotNetProjectNew.Controllers
{
    public class AccountController : BaseAPIController
    {

        private readonly DataContext _context;
        public AccountController(DataContext context)
        {
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AppUser>> RegisterUser(RegisterDTO registerDTO)
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

            return user;
        }

        private async Task<bool> UserExists(string usernameToCheck)
        {
            return await _context.Users.AnyAsync(username => username.Username == usernameToCheck.ToLower());
        }
    }
}
