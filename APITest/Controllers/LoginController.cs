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
        //[HttpPost("authenticate")]
        [HttpPost]
        public async Task<IActionResult> Authenticate([FromForm] Users userParam)
        {
            Console.WriteLine("jdfskldjflskjl " + userParam.Username + " " + userParam.Password + " ldjflsdjflksjdlfjañls");
            var user = await _userService.Authenticate(userParam.Username, userParam.Password);
            if (user == null)
                return BadRequest(new { message = "Username or password is incorrect" });
            user.Password = null;
            return Ok(user);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAll();
            return Ok(users);
        }
    }
}
