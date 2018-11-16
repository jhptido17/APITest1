using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using APITest.Models;
using APITest.Services;

namespace APITest.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private IUserService _userService;

        public LoginController(IUserService userService)
        {
            Console.WriteLine("djslfjalsdjfljsljjlsdjlfjlsjafl");
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Authenticate([FromBody] Users userParam)
        {
            var user = await _userService.Authenticate(userParam.Username, userParam.Password);
            if (user == null)
                return BadRequest("Username or password is incorrect");
            user.Password = null;
            return Ok("Access Granted");
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAll();
            return Ok(users);
        }
    }
}
