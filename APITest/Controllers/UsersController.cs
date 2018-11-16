using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APITest.Models;
using Microsoft.AspNetCore.Authorization;

namespace APITest.Controllers
{
    [Authorize(Roles = "admin")]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly TheCRMserviceContext _context;

        public UsersController(TheCRMserviceContext context)
        {
            _context = context;
        }

        // GET: api/users
        [HttpGet]
        public IEnumerable<Users> GetUsers()
        {
            return _context.Users;
        }

        // GET: api/users/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUsers([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var users = await _context.Users.FindAsync(id);

            if (users == null)
            {
                return NotFound();
            }

            return Ok(users);
        }

        // PUT: api/users/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsers([FromRoute] int id, [FromBody] Users users)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }  
            if((_context.Users.Where(c => c.Id != id && c.Username == users.Username)).Count() > 0)
                return BadRequest("Username Already Used");
            var currentUser = _context.Users.Where(c => c.Id == id);
            if (currentUser.First() == null)
                return BadRequest();
            
            if (users.Username == null && users.Password == null)
            {
                currentUser.First().Role = users.Role;
            }
            else if(users.Username == null && users.Role == null)
            {
                currentUser.First().Password = users.Password;
            }
            else if (users.Password == null && users.Role == null)
            {
                currentUser.First().Username = users.Username;
            }
            else if (users.Username == null){
                currentUser.First().Password = users.Password;
                currentUser.First().Role = users.Role;
            }
            else if(users.Password == null)
            {
                currentUser.First().Username = users.Username;
                currentUser.First().Role = users.Role;
            }
            else if(users.Role == null)
            {
                currentUser.First().Username = users.Username;
                currentUser.First().Password = users.Password;
            }
            else
            {
                currentUser.First().Username = users.Username;
                currentUser.First().Password = users.Password;
                currentUser.First().Role = users.Role;
            }

            //_context.Entry(currentUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsersExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
        
        // POST: api/users
        [HttpPost]
        public async Task<IActionResult> PostUsers([FromBody] Users users)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }  
            if((_context.Users.Where(c => c.Username == users.Username)).Count() > 0)
                return BadRequest("Username Already Used");
             _context.Users.Add(users);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUsers", new { id = users.Id }, users);
        }

        // DELETE: api/users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsers([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var users = await _context.Users.FindAsync(id);
            if (users == null)
            {
                return NotFound();
            }

            _context.Users.Remove(users);
            await _context.SaveChangesAsync();

            return Ok(users);
        }

        private bool UsersExists(int id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}