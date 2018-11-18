using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APITest.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace APITest.Controllers
{
    //[Authorize]
    [Authorize(Roles = "admin, user")]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly TheCRMserviceContext _context;
         private readonly IHostingEnvironment _hostingEnvironment;

        public CustomersController(TheCRMserviceContext context,  IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        // GET: api/Customers
        [HttpGet]
        public IEnumerable<Customers> GetCustomers(int page = 1, int rows = 10)
        {
            //return _context.Customers.Skip((page-1) * rows).Take(rows).ToList();
            return _context.Customers;
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCustomers([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customers = await _context.Customers.FindAsync(id);

            if (customers == null)
            {
                return NotFound();
            }

            return Ok(customers);
        }

        // PUT: api/Customers/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCustomers([FromRoute] int id, [FromBody] Customers customers)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            /*if(_context.Customers.Any(c => c.Id == id))
            {
                return NotFound();
            }

            _context.Customers.Update(customers);
            await _context.SaveChangesAsync();*/

            var currentCustomer = await _context.Customers.Where(c => c.Id == id).FirstOrDefaultAsync();
            if (currentCustomer == null)
                return BadRequest();

            if (customers.Name == null && customers.Surname == null)
            {
                currentCustomer.UpdateBy = User.Identity.Name;
            }
            else if(customers.Name == null && customers.Surname != null)
            {
                currentCustomer.Surname = customers.Surname;
                currentCustomer.UpdateBy = User.Identity.Name;
            }
            else if(customers.Surname == null && customers.Name != null)
            {
                currentCustomer.Name = customers.Name;
                currentCustomer.UpdateBy = User.Identity.Name;
            }
            else
            {
                currentCustomer.Name = customers.Name;
                currentCustomer.Surname = customers.Surname;
                currentCustomer.UpdateBy = User.Identity.Name;
            }
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

        // POST: api/Customers
        [HttpPost]
        public async Task<IActionResult> PostCustomers([FromBody] Customers customers)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            customers.CreatedBy = User.Identity.Name;
            customers.UpdateBy = User.Identity.Name;
            _context.Customers.Add(customers);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCustomers", new { id = customers.Id }, customers);
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomers([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var customers = await _context.Customers.FindAsync(id);
            if (customers == null)
            {
                return NotFound();
            }
            if (String.IsNullOrEmpty(customers.Image) && String.IsNullOrWhiteSpace(customers.Image))
            {
                DeleteFile(customers.Image);
            }
            
            _context.Customers.Remove(customers);
            await _context.SaveChangesAsync();

            return Ok(customers);
        }

        public void DeleteFile(string imagePath)
        {
            imagePath = _hostingEnvironment.WebRootPath + imagePath;
            string rFileName = imagePath.Split("\\")[imagePath.Split("\\").Length - 1];
            string rFilePath = ""; //= imagePath.Split("\\")[imagePath.Split("\\").Length - 2];
            for (int i = 0; i < imagePath.Split("\\").Length - 2; i++)
            {
                rFilePath = rFilePath + imagePath.Split("\\")[i] + "\\";
            }
            string [] dir = Directory.GetFiles(rFilePath, rFileName + "*.*", SearchOption.AllDirectories);
            Console.WriteLine(dir.First());
            System.IO.File.Delete(dir.First());
        }

        private bool CustomersExists(int id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}