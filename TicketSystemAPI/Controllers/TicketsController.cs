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
    public class TicketsController : ControllerBase
    {
        private readonly ITicket _ticket;
        private readonly IComment _comment;

        public TicketsController(ITicket ticket, IComment comment)
        {
            _ticket = ticket;
            _comment = comment;
        }

        [HttpGet]
        public async Task<IActionResult> GetTickets()
        {
            return Ok(await _ticket.GetTicketsAsync());
        }

        [HttpPost]
        public async Task<IActionResult> CreateTicket(Ticket ticket)
        {
            if (ticket == null)
                return BadRequest();

            var cTicket = await _ticket.CreateTicketAsync(ticket);
            return NoContent();

            //return CreatedAtAction(
            //    nameof(GetTicketById),
            //    new { id = cTicket.TicketId },
            //    cTicket
            //        );
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTicketById(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }

            return Ok(await _ticket.GetTicketAsync(id));
        }

        [HttpPut("{id}")]
        public IActionResult ModifyTicket(int id, Ticket ticket)
        {
            //if (id != ticket.TicketId)
            //{
            //    return BadRequest(new
            //    {
            //        Message = "IDs for ticket do not match"
            //    });
            //}

            _ticket.UpdateTicket(id, ticket);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteTicket(int? id)
        {
            if (id == null)
                return BadRequest("Provide id for ticket to delete");

            _ticket.DeleteTicket(id);

            return NoContent();
        }

        [HttpPost]
        [Route("/api/tickets/{ticketId}/comments")]
        public async Task<IActionResult> PostComment(int ticketId, Comment comment)
        {
            var ticketExists = await _ticket.GetTicketAsync(ticketId);
            if (ticketExists == null)
                return BadRequest(new
                {
                    Message = "Ticket with provided id does not exists"
                });

            var result = await _comment.PostComment(ticketId, comment);

            return Ok(new
            {
                Message = "Comment added"
            });
            //return CreatedAtAction(
            //    nameof(GetComments),
            //    new { id = result.CommentId},
            //    result);
        }

        [HttpGet]
        [Route("/api/tickets/{ticketId}/comments")]
        public async Task<IActionResult> GetComments(int ticketId)
        {
            var comments = await _comment.GetComments(ticketId);
            return Ok(comments);
        }

        [HttpDelete]
        [Route("/api/tickets/{ticketId}/comments/{commentId}")]
        public IActionResult DeleteComments(int ticketId, int commentId)
        {
            _comment.DeleteComment(ticketId, commentId);
            return NoContent();
        }

        [HttpGet]
        [Route("/api/tickets/{email}")]
        public async Task<IActionResult> GetDevTickets(string email)
        {
            var userTickets = await _ticket.GetSingleUserTicketsAsync(email);
            return Ok(userTickets);
        }

        [HttpGet]
        [Route("/api/tickets/info")]
        public async Task<IActionResult> GetTicketInfo()
        {
            var ticketInfo = await _ticket.GetTicketTypesAsync();
            return Ok(ticketInfo);
        }
    }
}