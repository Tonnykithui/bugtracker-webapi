using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core2.Models;
using DataStore.EF;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TicketSystemAPI.Interfaces;

namespace TicketSystemAPI.Repository
{
    public class UserRepository : IPeople
    {
        private readonly AppDbContext _dbContext;
        private readonly UserManager<ApplicationsUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserRepository(
            AppDbContext dbContext, 
            UserManager<ApplicationsUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task DeleteUserAsync(string id)
        {
            //check user exists
            var userExists = await _userManager.FindByIdAsync(id);
            if (userExists != null)
            {
                await _userManager.DeleteAsync(userExists);
            }
        }


        public async Task<Object> GetApplicationUser()
        {
            var users = await _userManager.Users.ToListAsync();

            var roles = await _roleManager.Roles.ToListAsync();

            var userInRole = await _dbContext.UserRoles.ToListAsync();

            //Hashtable ht = new Hashtable();
            var userRole = from u in users
                           join usr in userInRole
                           on u.Id equals usr.UserId
                           join r in roles
                           on usr.RoleId equals r.Id
                           select new
                           {
                               id = u.Id,
                               firstName = u.FirstName,
                               lastName = u.LastName,
                               email = u.Email,
                               phone = u.PhoneNumber,
                               role = r.Name
                           };

            var userObj = userRole as Object;

            return userObj;
        }

        //public async Task<List<ApplicationsUser>> GetApplicationUser()
        //{
        //    var users = await _userManager.Users.ToListAsync();
        //    return users;
        //}

        public async Task UpdateUserAsync(string id, ApplicationsUser user)
        {
            var userExists = await _userManager.FindByIdAsync(id);
            if (userExists != null)
            {
                userExists.Email = user.Email ?? user.Email;
                userExists.FirstName = user.FirstName ?? user.FirstName;
                userExists.LastName = user.LastName ?? user.LastName;
                userExists.PhoneNumber = user.PhoneNumber ?? user.PhoneNumber;
                userExists.UserName = user.UserName ?? user.UserName;

                if (user.Role == "Admin")
                {
                    await _userManager.AddToRoleAsync(userExists, "Admin");
                    await _userManager.RemoveFromRoleAsync(userExists, "Developer");
                } else if(user.Role == "Developer")
                {
                    await _userManager.RemoveFromRoleAsync(userExists, "Admin");
                    await _userManager.AddToRoleAsync(userExists, "Developer");
                }
                await _dbContext.SaveChangesAsync();
            }
        }

        
    }
}
//public async Task DeleteUserAsync(string id)
//{
//    //check user exists
//    var userExists = await _userManager.FindByIdAsync(id);
//    if(userExists != null)
//    {
//       await _userManager.DeleteAsync(userExists);
//    }

//}

//public async Task UpdateUserAsync(string id, ApplicationsUser user)
//{
//    var userExists = await _userManager.FindByIdAsync(id);
//    if(userExists != null)
//    {
//        userExists.Email = user.Email ?? user.Email;
//        userExists.FirstName = user.FirstName ?? user.FirstName;
//        userExists.LastName = user.LastName ?? user.LastName;
//        userExists.PhoneNumber = user.PhoneNumber ?? user.PhoneNumber;
//        userExists.UserName = user.UserName ?? user.UserName;

//        if(user.Role == "Admin")
//        {
//            await _role
//        }
//    }
//}