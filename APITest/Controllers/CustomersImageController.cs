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
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;

namespace APITest.Controllers
{
    [Authorize(Roles = "admin, user")]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersImageController : ControllerBase
    {
        private readonly TheCRMserviceContext _context;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _hostingEnvironment;

        public CustomersImageController(TheCRMserviceContext context, IConfiguration configuration, IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        //api/CustomersImage/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetImage([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentCustomer = await _context.Customers.Where(c => c.Id == id).FirstOrDefaultAsync();

            if (currentCustomer == null)
                return BadRequest();

            string imagePath = currentCustomer.Image;

            if (imagePath == null || imagePath == "")
            {
                return NotFound();
            }
            return Ok(imagePath);
        }

        //api/CustomersImage/5
        [HttpPost("{id}")]
        public async Task<IActionResult> PostImage([FromRoute] int id, IFormFile file)
        {
            string [] validFileExtensions = { ".png", ".jpg", ".tiff", ".bmp"};
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            long size = file.Length;
            string fileExtension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];

            if (!validFileExtensions.Contains(fileExtension))
            {
                return NoContent();
            }

            var directory = _hostingEnvironment.WebRootPath;
            var directoryDB = _configuration.GetSection("ImagesDirectory").Value;

            if (!Directory.Exists(directory+directoryDB))
            {
                Directory.CreateDirectory(directory+directoryDB);
            }

            // full path to file, wwwroot and imagesDirectory
            var filePath = Path.Combine(directoryDB, Path.GetRandomFileName());
            filePath = filePath.Split('.')[filePath.Split('.').Length-2];
            filePath = filePath + fileExtension;
            var currentCustomer = await _context.Customers.Where(c => c.Id == id).FirstOrDefaultAsync();
            if (currentCustomer == null)
                return BadRequest();

            if (String.IsNullOrEmpty(currentCustomer.Image) && String.IsNullOrWhiteSpace(currentCustomer.Image))
            {
                DeleteFile(currentCustomer.Image);
            }

            currentCustomer.UpdateBy = User.Identity.Name;
            currentCustomer.Image = filePath;

            try
            {
                await _context.SaveChangesAsync();
                if (file.Length > 0)
                {
                    using (var stream = new FileStream(directory + filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
                return Ok(new { size, filePath });
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
        }

        [HttpPost]
        public async Task<IActionResult> PostImg( IFormFile file )
        {
            string [] validFileExtensions = { ".png", ".jpg", ".tiff", ".bmp"};
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            long size = file.Length;
            string fileExtension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];

            if (!validFileExtensions.Contains(fileExtension))
            {
                return NoContent();
            }

            var directory = _hostingEnvironment.WebRootPath;
            var directoryDB = _configuration.GetSection("ImagesDirectory").Value;

            if (!Directory.Exists(directory+directoryDB))
            {
                Directory.CreateDirectory(directory+directoryDB);
            }

            // full path to file, wwwroot and imagesDirectory
            var filePath = Path.Combine(directoryDB, Path.GetRandomFileName());
            filePath = filePath.Split('.')[filePath.Split('.').Length-2];
            filePath = filePath + fileExtension;
            try
            {
                if (file.Length > 0)
                {
                    using (var stream = new FileStream(directory + filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
                return Ok(filePath);
            }
            catch (Exception ex)
            {  
                throw ex;
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImage([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentCustomer = await _context.Customers.Where(c => c.Id == id).FirstOrDefaultAsync();
            if (currentCustomer == null)
                return BadRequest();

            try
            { 
                if (String.IsNullOrEmpty(currentCustomer.Image) && String.IsNullOrWhiteSpace(currentCustomer.Image))
                {
                    string imagePath = currentCustomer.Image;
                    currentCustomer.Image = null;
                    await _context.SaveChangesAsync();
                    DeleteFile(imagePath);
                    return Ok("Image removed");
                }
                return Ok("Customer without Image");
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