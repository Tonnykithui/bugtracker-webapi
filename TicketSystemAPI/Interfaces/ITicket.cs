using Core2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketSystemAPI.Interfaces
{
    public interface ITicket
    {
        Task<Ticket> CreateTicketAsync(Ticket ticket);

        Task<List<Ticket>> GetTicketsAsync();

        Task<Object> GetTicketAsync(int? id);

        void UpdateTicket(int id, Ticket ticket);

        void DeleteTicket(int? id);

        Task<List<Ticket>> GetSingleUserTicketsAsync(string email);

        Task<Object> GetTicketTypesAsync();
    }
}
