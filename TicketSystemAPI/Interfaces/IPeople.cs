using Core2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TicketSystemAPI.Interfaces
{
    public interface IPeople
    {
        //List<ApplicationsUser>
        Task<Object> GetApplicationUser();

        Task DeleteUserAsync(string id);

        Task UpdateUserAsync(string id, ApplicationsUser user);
    }
}
