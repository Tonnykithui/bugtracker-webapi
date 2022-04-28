using System;
using System.Collections.Generic;
using System.Text;

namespace Core2.Models
{
    public class ProjUser
    {
        public int ProjectId { get; set; }
        public Project Project { get; set; }

        public string ApplicationUserId { get; set; }
        public ApplicationsUser ApplicationsUser { get; set; }

        public List<string> ApplicationUserIds { get; set; }
    }
}
