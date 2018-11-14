﻿using System;
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

            var currentCustomer = _context.Customers.Where(c => c.Id == id);

            if (currentCustomer.First() == null)
                return BadRequest();

            string imagePath = currentCustomer.First().Image;

            if (imagePath == null || imagePath == "")
            {
                return Content("No image");
            }
            return Ok(imagePath);
        }

        //api/CustomersImage/5
        /*[HttpGet("{id}")]
        public async Task<IActionResult> GetImage([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentCustomer = _context.Customers.Where(c => c.Id == id);

            if (currentCustomer.First() == null)
                return BadRequest();

            string imagePath = currentCustomer.First().Image; 

            if (imagePath == null || imagePath == "")
            {
                return Content("No image");
            }
      
            Console.WriteLine(Directory.GetCurrentDirectory());
            string fileName = imagePath.Split("\\")[imagePath.Split("\\").Length - 1];
            string filePath = ""; //= imagePath.Split("\\")[imagePath.Split("\\").Length - 2];
            for (int i = 0; i < imagePath.Split("\\").Length - 2; i++)
            {
                filePath = filePath + imagePath.Split("\\")[i] + "\\";
            }

            string [] dir = Directory.GetFiles(filePath, fileName + "*.*", SearchOption.AllDirectories);

            if (dir == null)
            {
                return NoContent();
            }

            //string [] dir = Directory.GetFiles(imagePath, "*png");

            var memory = new MemoryStream();
            using (var stream = new FileStream(dir.First(), FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;
            return File(memory, GetContentType(dir.First()), Path.GetFileName(dir.First()));

            //return _context.Customers;
        }

        private string GetContentType(string file)
        {
            var fileExtension = "." +file.Split('.')[file.Split('.').Length - 1];
            Console.WriteLine("image/" + fileExtension.TrimStart('.'));
            return "image/" + fileExtension.TrimStart('.');
        }*/

        //api/CustomersImage/5
        [HttpPost("{id}")]
        public async Task<IActionResult> PostImage([FromRoute] int id, IFormFile file /*[FromBody] List<IFormFile> files*/)
        {
            Console.WriteLine("." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1]);
            string [] validFileExtensions = { ".png", ".jpg", ".tiff", ".bmp"};
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            //long size = files.Sum(f => f.Length);
            long size = file.Length;
            string fileExtension = "." + file.FileName.Split('.')[file.FileName.Split('.').Length - 1];

            if (!validFileExtensions.Contains(fileExtension))
            {
                return NoContent();
            }

            var directory = _hostingEnvironment.WebRootPath;
            var directoryDB = _configuration.GetSection("ImagesDirectory").Value; ;

            if (!Directory.Exists(directory+directoryDB))
            {
                Directory.CreateDirectory(directory+directoryDB);
            }

            // full path to file in temp location
            var filePath = Path.Combine(directoryDB, Path.GetRandomFileName());
            filePath = filePath.Split('.')[filePath.Split('.').Length-2];
            filePath = filePath + fileExtension;
            var currentCustomer = _context.Customers.Where(c => c.Id == id);
            if (currentCustomer.First() == null)
                return BadRequest();

            if (currentCustomer.First().Image != "" && currentCustomer.First().Image != " " && currentCustomer.First().Image != null)
            {
                DeleteFile(currentCustomer.First().Image);
            }

            currentCustomer.First().UpdateBy = User.Identity.Name;
            currentCustomer.First().Image = filePath;

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
                /*foreach (var formFile in files)
                {
                    if (formFile.Length > 0)
                    {
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await formFile.CopyToAsync(stream);
                        }
                    }
                }*/

                // process uploaded files
                // Don't rely on or trust the FileName property without validation.

                return Ok(new { size, filePath });
                /*return Ok(new { count = files.Count, size, filePath});*/
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteImage([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var currentCustomer = _context.Customers.Where(c => c.Id == id);
            if (currentCustomer.First() == null)
                return BadRequest();

            try
            { 
                if (currentCustomer.First().Image != "" && currentCustomer.First().Image != " " && currentCustomer.First().Image != null)
                {
                    string imagePath = currentCustomer.First().Image;
                    currentCustomer.First().Image = null;
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