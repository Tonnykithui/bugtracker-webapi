using Core2.Models;
using DataStore.EF;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TicketSystemAPI.Interfaces;

namespace TicketSystemAPI.Repository
{
    public class TicketRepository : ITicket
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<ApplicationsUser> _userManager;
        public TicketRepository
            (
            AppDbContext dbContext,
            UserManager<ApplicationsUser> userManager
            )
        {
            _dbContext = dbContext;
            _userManager = userManager;
        }

        public async Task<Ticket> CreateTicketAsync(Ticket ticket)
        {
            if(ticket != null)
            {

                ticket.ReportDate = DateTime.Now;
                ticket.DueDate = string.IsNullOrWhiteSpace(ticket.DueDate.ToString()) ? null : ticket.DueDate; 
                await _dbContext.Tickets.AddAsync(ticket);
                await _dbContext.SaveChangesAsync();
                //_dbContext.Entry(ticket).State = EntityState.Added;
                
                int ticketId = ticket.TicketId;


                if (ticket.AssignedUserId != null)
                {
                    foreach (var AuserId in ticket.AssignedUserId)
                    {
                        var newRecord = new TicketUser
                        {
                            UserId = AuserId,
                            TicketId = ticketId
                        };

                        await _dbContext.TicketUser.AddAsync(newRecord);
                        await _dbContext.SaveChangesAsync();
                    }
                    
                }
                
            }

            return ticket;
        }


        public void DeleteTicket(int? id)
        {
            if (id != null)
            {
                var ticket = _dbContext.Tickets.Find(id);
                _dbContext.Tickets.Remove(ticket);
                _dbContext.SaveChanges();
            }
        }

        public async Task<List<Ticket>> GetSingleUserTicketsAsync(string email)
        {
            var userExists = await _userManager.FindByEmailAsync(email);
            //if(userExists == null)
            //{
                
            //}
            var tickets = await _dbContext.Tickets.Where(t => t.TicketOwner == email).ToListAsync();
            return tickets;
        }

        public async Task<Object> GetTicketAsync(int? id)
        {
            var ticketExists = await _dbContext.Tickets.FindAsync(id);
            if (ticketExists == null)
                throw new NullReferenceException("Ticket not found");

            var tickets = await _dbContext.Tickets.ToListAsync();
            var comments = await _dbContext.Comments.ToListAsync();
            var ticketAssignee = await _dbContext.TicketUser.ToListAsync();

            var ticketDetails = from t in tickets
                                join c in comments
                                on t.TicketId equals c.TicketId
                                select new
                                {
                                    TicktId = t.TicketId,
                                    MessageId = c.CommentId,
                                    MessageDet = !string.IsNullOrEmpty(c.Message) ? c.Message : "",
                                    MessageTime = c.SubmitTime.ToLongDateString(),
                                    MessageOwner = !string.IsNullOrEmpty(c.Owner) ? c.Owner : ""                                
                                };

            var ticketUsers = from t in tickets
                              join c in ticketAssignee
                              on t.TicketId equals c.TicketId
                              select new
                              {
                                  Assigned = c.UserId,
                                  ID = c.TicketId
                              };

            var tickDet = ticketDetails.Where(tdet => tdet.TicktId == id);
            var tickAssigne = ticketUsers.Where(tas => tas.ID == id);

            var ts = tickAssigne as Object;
            var td = tickDet as Object;
            var tkt = ticketExists as Object;
            return new
            {
                Comments = td,
                Users = ts
            };
        }

        public async Task<List<Ticket>> GetTicketsAsync()
        {
            return await _dbContext.Tickets.ToListAsync();
        }

        public async Task<object> GetTicketTypesAsync()
        {
            var tickets = await _dbContext.Tickets.ToListAsync();

            var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
            int numOfAdmins = adminUsers.Count();

            var Developers = await _userManager.GetUsersInRoleAsync("Developer");
            int numOfDevs = Developers.Count();

            var openTickets = tickets.Where(ot => ot.Status == "Open").Count();
            var inprogressTickets = tickets.Where(ip => ip.Status == "Inprogress").Count();
            var closedTickets = tickets.Where(ip => ip.Status == "Closed").Count();

            var mediumPriority = tickets.Where(mp => mp.Priority == "Medium").Count();
            var lowPriority = tickets.Where(mp => mp.Priority == "Low").Count();
            var highPriority = tickets.Where(mp => mp.Priority == "High").Count();

            return new
            {
                Admins = numOfAdmins,
                Developers = numOfDevs,
                OpenTickets = openTickets,
                InProgressTickets = inprogressTickets,
                ClosedTickets = closedTickets,
                MediumPriority = mediumPriority,
                LowPriority = lowPriority,
                HighPriority = highPriority
            };
        }

        public void UpdateTicket(int id, Ticket ticket)
        {

            var ticketExists = _dbContext.Tickets.Find(id);

            if(ticketExists != null)
            {

                ticketExists.Description = string.IsNullOrWhiteSpace(ticket.Description) ? 
                                           ticketExists.Description : ticket.Description;

                ticketExists.Title = string.IsNullOrWhiteSpace(ticket.Title) ?
                                           ticketExists.Title : ticket.Title;

                ticketExists.Status = string.IsNullOrWhiteSpace(ticket.Status) ?
                                           ticketExists.Status : ticket.Status;

                ticketExists.Priority = string.IsNullOrWhiteSpace(ticket.Priority) ?
                                           ticketExists.Priority : ticket.Priority;

                ticketExists.DueDate = string.IsNullOrWhiteSpace(ticket.DueDate.ToString()) ?
                                           ticketExists.DueDate : ticket.DueDate;

                ticketExists.TicketOwner =  ticketExists.TicketOwner;

                ticketExists.EstimateTime = string.IsNullOrWhiteSpace(ticket.EstimateTime) ?
                                           ticketExists.EstimateTime : ticket.EstimateTime;


                if (ticket.AssignedUserId != null)
                {
                    var ticketsToDel = _dbContext.TicketUser.Where(t => t.TicketId == id);
                    _dbContext.TicketUser.RemoveRange(ticketsToDel);

                    foreach (var user in ticket.AssignedUserId)
                    {
                        var record = new TicketUser
                        {
                            UserId = user,
                            TicketId = id
                        };

                        //_dbContext.Entry(record).State = EntityState.Modified;
                        _dbContext.TicketUser.Add(record);
                    }
                }
                _dbContext.SaveChanges();
            }

           
        }

        
    }
}
