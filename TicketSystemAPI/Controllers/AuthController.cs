using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core2.Interfaces;
using Core2.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace TicketSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ILoginRegister _loginRegister;

        public AuthController(ILoginRegister loginRegister)
        {
            _loginRegister = loginRegister;
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterAsync(Register register)
        {
            var createdUser = await _loginRegister.RegisterUserAsync(register);
            if (createdUser.IsError)
            {
                return BadRequest(createdUser.Message);
            }

            return Ok(createdUser.Message);
        }


        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> LoginAsync(Login login)
        {
            var loginUser = await _loginRegister.LoginUserAsync(login);
            if (loginUser.IsError)
            {
                return BadRequest(loginUser.Message);
            }

            return Ok(loginUser);
        }
    }
}