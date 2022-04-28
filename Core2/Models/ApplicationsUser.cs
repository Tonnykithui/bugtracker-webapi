using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core2.Models
{
    public class ApplicationsUser : IdentityUser
    {

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string Role { get; set; }

        public IEnumerable<TicketUser> TicketUser { get; set; }
        public IEnumerable<ProjUser> ProjUser { get; set; }
    }
}
