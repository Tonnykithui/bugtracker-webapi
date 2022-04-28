using System;
using System.Collections.Generic;
using System.Text;

namespace Core2.Models
{
    public class NotFoundExc
    {
        public string Message { get; set; }

        public bool IsError { get; set; }

        public Ticket Ticket { get; set; }
    }
}
