using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UdemyDotNetProjectNew.Data;
using UdemyDotNetProjectNew.Entities;

namespace UdemyDotNetProjectNew.Controllers
{
    public class UsersController : BaseAPIController
    {
        private readonly DataContext _context;

        public UsersController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<AppUser>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<AppUser>> GetUserById(int id)
        {
            return await _context.Users.FindAsync(id);
        }
    }
}
