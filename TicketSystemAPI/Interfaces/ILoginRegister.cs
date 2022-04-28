using Core2.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Core2.Interfaces
{
    public interface ILoginRegister
    {
        Task<ClientValidation> RegisterUserAsync(Register register);

        Task<ClientValidation> LoginUserAsync(Login login);
    }
}
