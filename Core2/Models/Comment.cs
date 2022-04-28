using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core2.Models
{
    public class Comment
    {
        public int CommentId { get; set; }

        public int TicketId { get; set; }

        [Required]
        public string Message { get; set; }

        public string Owner { get; set; }

        public DateTime SubmitTime { get; set; }

        public Ticket Ticket { get; set; }
 
    }
}
