using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core2.Models
{
    public class Ticket
    {
        public int TicketId { get; set; }

        public int ProjectId { get; set; }

        [Required]
        public string Title { get; set; }

        public string Description { get; set; }

        public string Priority { get; set; }

        public string Status { get; set; }

        public DateTime? ReportDate { get; set; }

        public DateTime? DueDate { get; set; }

        public string TicketOwner { get; set; }

        public string EstimateTime { get; set; }

        public Project Project { get; set; }

        public List<Comment> Comment { get; set; }

        public List<string> AssignedUserId { get; set; }

        public IEnumerable<TicketUser> TicketUser { get; set; }
    }
}
