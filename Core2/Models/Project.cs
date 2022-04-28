using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Core2.Models
{
    public class Project
    {

        public int ProjectId { get; set; }

        //[Required]
        public string ProjectName { get; set; }

        //[Required]
        public string Description { get; set; }

        public DateTime CreationDate { get; set; }

        public string CreatedBy { get; set; }

        public List<string> AssignedUserId { get; set; }

        public List<Ticket> Ticket { get; set; }
        public IEnumerable<ProjUser> ProjUser { get; set; }
    }
}
