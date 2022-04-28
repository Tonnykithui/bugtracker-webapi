using System;
using System.Collections.Generic;
using System.Text;

namespace Core2.Models
{
    public class TicketUser
    {
        public int TicketId { get; set; }
        public Ticket Ticket { get; set; }

        public string UserId { get; set; }
        public ApplicationsUser ApplicationsUser { get; set; }
    }
}
