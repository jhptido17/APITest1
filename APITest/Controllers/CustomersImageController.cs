using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APITest.Models;
using Microsoft.AspNetCore.Authorization;
using System.IO;

namespace APITest.Controllers
{
    [Authorize(Roles = "admin, user")]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersImageController : ControllerBase
    {
        private readonly TheCRMserviceContext _context;

        public CustomersImageController(TheCRMserviceContext context)
        {
            _context = context;
        }
        //test
        [HttpGet]
        public IEnumerable<Customers> GetImage()
        {
            string path = Directory.GetCurrentDirectory();
            Console.WriteLine(path);
            return _context.Customers;
        }

        [HttpPost]
        //[Route("user/PostUserImage")]
        public async Task<IActionResult> PostUserImage([FromRoute] int id/*, [FromForm]  image*/)
        {
            string path = Directory.GetCurrentDirectory();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentCustomer = _context.Customers.Where(c => c.Id == id);
            if (currentCustomer.First() == null)
                return BadRequest();
            /*
            currentCustomer.First().Name = customers.Name;
            currentCustomer.First().Surname = customers.Surname;
            currentCustomer.First().UpdateBy = User.Identity.Name;
            */

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomersExists(id))
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

        private bool CustomersExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}