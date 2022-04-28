using System;
using System.Collections.Generic;
using System.Text;

namespace Core2.Models
{
    public class ClientValidation
    {
        public string Message { get; set; }

        public bool IsError { get; set; }

        public IEnumerable<string> Errors { get; set; }

        public DateTime? ExpireDate { get; set; }
    }
}
