using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TicketSystemAPI.Interfaces;

namespace TicketSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IPeople _people;

        public UsersController(IPeople people)
        {
            _people = people;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            return Ok(await _people.GetApplicationUser());
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdUser(string id, ApplicationsUser user)
        {
            if(id != user.Id)
            {
                return BadRequest("IDs do not match");
            }

            await _people.UpdateUserAsync(id, user);
            return NoContent();

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(string id)
        {
            await _people.DeleteUserAsync(id);
            return NoContent();
        }
    }
}